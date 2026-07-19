namespace MediSync.BuildingBlocks.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    protected BaseEntity() { }

    protected BaseEntity(Guid id) { }
}
