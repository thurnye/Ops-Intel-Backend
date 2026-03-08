using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UnitOfMeasureService : IUnitOfMeasureService
{
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

    public UnitOfMeasureService(IUnitOfMeasureRepository unitOfMeasureRepository)
    {
        _unitOfMeasureRepository = unitOfMeasureRepository;
    }

    public async Task<UnitOfMeasureResponse> CreateAsync(CreateUnitOfMeasureRequest request, CancellationToken cancellationToken = default)
    {
        var nameExists = await _unitOfMeasureRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null)
            throw new InvalidOperationException(InventoryErrorMessages.UnitOfMeasureAlreadyExists(request.Name));

        var symbolExists = await _unitOfMeasureRepository.ExistsAsync(x => x.Symbol == request.Symbol, cancellationToken);
        if (symbolExists)
            throw new InvalidOperationException(InventoryErrorMessages.UnitOfMeasureSymbolAlreadyExists(request.Symbol));

        var unit = new UnitOfMeasure
        {
            Name = request.Name,
            Symbol = request.Symbol
        };

        await _unitOfMeasureRepository.AddAsync(unit, cancellationToken);
        await _unitOfMeasureRepository.SaveChangesAsync(cancellationToken);

        return Map(unit);
    }

    public async Task<UnitOfMeasureResponse?> UpdateAsync(UpdateUnitOfMeasureRequest request, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfMeasureRepository.GetByIdAsync(request.Id, cancellationToken);
        if (unit == null)
            return null;

        var nameExists = await _unitOfMeasureRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null && nameExists.Id != request.Id)
            throw new InvalidOperationException(InventoryErrorMessages.UnitOfMeasureAlreadyExists(request.Name));

        var symbolExists = await _unitOfMeasureRepository.ExistsAsync(x => x.Symbol == request.Symbol && x.Id != request.Id, cancellationToken);
        if (symbolExists)
            throw new InvalidOperationException(InventoryErrorMessages.UnitOfMeasureSymbolAlreadyExists(request.Symbol));

        unit.Name = request.Name;
        unit.Symbol = request.Symbol;
        unit.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfMeasureRepository.Update(unit);
        await _unitOfMeasureRepository.SaveChangesAsync(cancellationToken);

        return Map(unit);
    }

    public async Task<UnitOfMeasureResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfMeasureRepository.GetByIdAsync(id, cancellationToken);
        return unit == null ? null : Map(unit);
    }

    public async Task<IReadOnlyList<UnitOfMeasureResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var units = await _unitOfMeasureRepository.GetAllAsync(cancellationToken);
        return units.Select(Map).ToList();
    }

    private static UnitOfMeasureResponse Map(UnitOfMeasure unit) => new()
    {
        Id = unit.Id,
        Name = unit.Name,
        Symbol = unit.Symbol
    };
}
