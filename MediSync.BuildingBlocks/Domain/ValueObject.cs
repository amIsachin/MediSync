namespace MediSync.BuildingBlocks.Domain;

/// <summary>
/// Represents a Domain-Driven Design (DDD) Value Object.
/// A value object is identified by the values of its properties rather than by an identity.
/// Two value objects are considered equal when all of their equality components are equal.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Returns the collection of property values that participate in equality comparison.
    /// Derived classes must return the properties that uniquely define the value object.
    /// </summary>
    /// <returns>
    /// A sequence of values used to determine equality and generate the hash code.
    /// </returns>
    protected abstract IEnumerable<object> GetEqualityComponents();


    /// <summary>
    /// Determines whether the current value object is equal to another object.
    /// Two value objects are equal when they are of the same type and all of
    /// their equality components match in the same order.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified object is an equivalent value object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code based on the value object's equality components.
    /// Equal value objects always produce the same hash code.
    /// </summary>
    /// <returns>A hash code representing the value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }


    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns><c>true</c> if both value objects are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
        => Equals(left, right);


    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns><c>true</c> if the value objects are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !Equals(left, right);
}
