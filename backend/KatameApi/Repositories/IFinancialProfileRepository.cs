using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IFinancialProfileRepository
{
    Task<FinancialProfile?> GetAsync();

    /// <summary>
    /// Crea el perfil si todavía no existe, o actualiza el ingreso si ya existe.
    /// Se expone como una sola operación (en vez de "GetAsync + AddAsync a mano
    /// desde el servicio") para poder resolver de forma segura el caso en que dos
    /// peticiones casi simultáneas intenten crear el perfil por primera vez.
    /// </summary>
    Task<FinancialProfile> UpsertAsync(decimal monthlyIncome);
}
