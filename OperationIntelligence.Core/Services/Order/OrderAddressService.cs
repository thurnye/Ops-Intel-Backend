using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class OrderAddressService : IOrderAddressService
{
    private readonly IOrderAddressRepository _orderAddressRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderAddressService(
        IOrderAddressRepository orderAddressRepository,
        IOrderRepository orderRepository)
    {
        _orderAddressRepository = orderAddressRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderAddressResponse> AddAsync(CreateOrderAddressRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (request.AddressType == AddressType.Billing)
        {
            var existing = await _orderAddressRepository.GetBillingAddressAsync(request.OrderId, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException(OrderErrorMessages.BillingAddressAlreadyExists);
        }

        if (request.AddressType == AddressType.Shipping)
        {
            var existing = await _orderAddressRepository.GetShippingAddressAsync(request.OrderId, cancellationToken);
            if (existing != null )
                throw new InvalidOperationException(OrderErrorMessages.ShippingAddressAlreadyExists);
        }

        var entity = new OrderAddress
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            AddressType = request.AddressType,
            ContactName = request.ContactName,
            CompanyName = request.CompanyName,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            // IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _orderAddressRepository.AddAsync(entity, cancellationToken);
        await _orderAddressRepository.SaveChangesAsync(cancellationToken);

        return new OrderAddressResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            AddressType = entity.AddressType,
            ContactName = entity.ContactName,
            CompanyName = entity.CompanyName,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            StateOrProvince = entity.StateOrProvince,
            PostalCode = entity.PostalCode,
            Country = entity.Country
        };
    }

    public async Task<OrderAddressResponse> UpdateAsync(Guid id, UpdateOrderAddressRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _orderAddressRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null )
            throw new KeyNotFoundException(OrderErrorMessages.AddressNotFound);

        entity.ContactName = request.ContactName;
        entity.CompanyName = request.CompanyName;
        entity.AddressLine1 = request.AddressLine1;
        entity.AddressLine2 = request.AddressLine2;
        entity.City = request.City;
        entity.StateOrProvince = request.StateOrProvince;
        entity.PostalCode = request.PostalCode;
        entity.Country = request.Country;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Email = request.Email;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        _orderAddressRepository.Update(entity);
        await _orderAddressRepository.SaveChangesAsync(cancellationToken);

        return new OrderAddressResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            AddressType = entity.AddressType,
            ContactName = entity.ContactName,
            CompanyName = entity.CompanyName,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            StateOrProvince = entity.StateOrProvince,
            PostalCode = entity.PostalCode,
            Country = entity.Country
        };
    }

    public async Task<IReadOnlyList<OrderAddressResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _orderAddressRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return rows.Select(address => new OrderAddressResponse
        {
            Id = address.Id,
            OrderId = address.OrderId,
            AddressType = address.AddressType,
            ContactName = address.ContactName,
            CompanyName = address.CompanyName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            StateOrProvince = address.StateOrProvince,
            PostalCode = address.PostalCode,
            Country = address.Country
        }).ToList();
    }
}