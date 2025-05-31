namespace Naveasy.Samples.Models;

public class ClientModel(int? id, string? name)
{
    public int? Id { get; } = id;
    public string? Name { get; } = name;
}