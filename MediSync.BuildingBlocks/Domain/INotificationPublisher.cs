namespace MediSync.BuildingBlocks.Domain;

public interface INotificationPublisher
{
    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
