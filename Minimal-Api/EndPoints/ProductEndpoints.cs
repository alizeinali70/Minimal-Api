using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiDataClasse;
using MinimalApiDbContext;
namespace MinimalApi.EndPoints;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var products = new List<Product>
{
    new Product { Id = Guid.NewGuid(), Name = "Wireless Headphones", Description = "Noise-cancelling Bluetooth headphones with 20-hour battery life", Price = 79.99d },
    new Product { Id = Guid.NewGuid(), Name = "Smart Watch", Description = "Fitness tracker with heart rate monitor and GPS", Price = 199.99d },
    new Product { Id = Guid.NewGuid(), Name = "Laptop Backpack", Description = "Water-resistant backpack with USB charging port", Price = 34.99d },
    new Product { Id = Guid.NewGuid(), Name = "Coffee Maker", Description = "Programmable 12-cup coffee maker with built-in grinder", Price = 89.99d },
    new Product { Id = Guid.NewGuid(), Name = "Yoga Mat", Description = "Non-slip exercise mat with carrying strap", Price = 24.99d }
};

        var group = routes.MapGroup("/api/Product").WithTags(nameof(Product));

        group.MapGet("/", async (AppDbContext context) =>
        {
            return await context.Products.ToListAsync().ConfigureAwait(true);
        })
        .WithName("GetAllProducts")
        .WithSummary("Retrieve all Products").
        WithDescription("Returns a paginated list of all products in the catalog");
        

        group.MapGet("/{id}", (AppDbContext context,Guid id) =>
        {
            return context.Products.FirstOrDefault(x => x.Id == id);
        })
        .WithName("GetProductById");

        group.MapPut("/{id}", (AppDbContext context, Guid id, Product input) =>
        {
            var p = context.Products.FirstOrDefault(x => x.Id == id);
            if(p != null)
                {
                p.Name = input.Name.IsNullOrEmpty() ? p.Name : input.Name;
                p.Description = input.Description.IsNullOrEmpty() ? p.Description : input.Description;
                p.Price = input.Price ?? p.Price;
                context.Products.Update(p);
                context.SaveChanges();
                return Results.Ok(p);
            }
            return Results.NotFound(id);
        })
        .WithName("UpdateProduct");

        group.MapPost("/", async (AppDbContext context,Product model) =>
        {
            var p = products.FirstOrDefault(x => x.Id == model.Id);

            if (p is not null)
            {
                return TypedResults.BadRequest("Product with the same ID already exists.");
            }
            model.Id = Guid.NewGuid();
            products.Add(model);
            context.Products.Add(model);
            await context.SaveChangesAsync().ConfigureAwait(true);

            return Results.Ok(model);

        })
        .WithName("CreateProduct");

        group.MapDelete("/{id}", (AppDbContext context, Guid id) =>
        {
            var p = context.Products.FirstOrDefault(x => x.Id == id);
            if (p != null)
            {
                context.Products.Remove(p);
                context.SaveChanges();
                return Results.Ok(p);
            }
            return Results.Ok(id);
        })
        .WithName("DeleteProduct");
    }
}