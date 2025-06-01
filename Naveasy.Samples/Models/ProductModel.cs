namespace Naveasy.Samples.Models;

public class ProductModel(int? id, string? name)
{
    public int? Id { get; } = id;
    public string? Name { get; } = name;
}