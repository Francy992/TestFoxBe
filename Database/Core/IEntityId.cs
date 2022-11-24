namespace Database.Core;
public interface IEntityId<T>
{
    T Id { get; set; }
}