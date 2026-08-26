using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class FinancialProfileRepository : IFinancialProfileRepository
{
    private readonly KatameDbContext _context;

    public FinancialProfileRepository(KatameDbContext context)
    {
        _context = context;
    }

    // El filtro global por usuario ya deja ver solo la fila del usuario actual,
    // así que basta con traer la primera (y única) que exista.
    public Task<FinancialProfile?> GetAsync() =>
        _context.FinancialProfiles.FirstOrDefaultAsync();

    // Upsert seguro ante condiciones de carrera: si dos peticiones casi
    // simultáneas llegan a la vez a "actualizar mi ingreso" cuando el perfil
    // todavía no existe, ambas pueden ver "no existe" y tratar de insertarlo. El
    // índice único en UserId (KatameDbContext.cs) deja pasar solo un INSERT; en
    // vez de dejar que la segunda petición reviente con un error 500, detectamos
    // ese conflicto y lo convertimos en un UPDATE sobre la fila que sí se guardó.
    public async Task<FinancialProfile> UpsertAsync(decimal monthlyIncome)
    {
        var profile = await GetAsync();

        if (profile is not null)
        {
            profile.MonthlyIncome = monthlyIncome;
            await _context.SaveChangesAsync();
            return profile;
        }

        var newProfile = new FinancialProfile { MonthlyIncome = monthlyIncome };
        await _context.FinancialProfiles.AddAsync(newProfile);

        try
        {
            await _context.SaveChangesAsync();
            return newProfile;
        }
        catch (DbUpdateException)
        {
            // Perdimos la carrera: soltamos el intento fallido para que no se
            // vuelva a insertar en el próximo SaveChanges, y actualizamos la fila
            // que sí ganó.
            _context.Entry(newProfile).State = EntityState.Detached;

            var winner = await GetAsync();
            if (winner is null)
            {
                throw;
            }

            winner.MonthlyIncome = monthlyIncome;
            await _context.SaveChangesAsync();
            return winner;
        }
    }
}
