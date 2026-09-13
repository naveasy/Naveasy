namespace Naveasy.Shell.Samples.Models;

public record OrderModel(int Id, string Customer, decimal Total);

public record OrderReviewModel(int OrderId, bool Approved);
