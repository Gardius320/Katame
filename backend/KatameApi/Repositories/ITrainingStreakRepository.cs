namespace KatameApi.Repositories;

public interface ITrainingStreakRepository
{
    Task<int> GetLongestAsync();

    /// <summary>
    /// Sube el récord guardado a <paramref name="candidate"/> si es mayor que el
    /// actual, y devuelve el récord vigente después de esa comparación. Mismo
    /// patrón "upsert seguro ante condiciones de carrera" que
    /// FinancialProfileRepository.UpsertAsync.
    /// </summary>
    Task<int> UpdateLongestIfHigherAsync(int candidate);
}
