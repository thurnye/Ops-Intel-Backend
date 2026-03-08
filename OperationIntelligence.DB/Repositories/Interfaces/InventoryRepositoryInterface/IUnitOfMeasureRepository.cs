namespace OperationIntelligence.DB;

public interface IUnitOfMeasureRepository : IBaseRepository<UnitOfMeasure>
{
    Task<UnitOfMeasure?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
