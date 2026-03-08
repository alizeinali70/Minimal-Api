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
            var p = context.Products.FirstOrDefault(x => x.Id == model.Id);

            if (p is not null)
            {
                return TypedResults.BadRequest("Product with the same ID already exists.");
            }
            model.Id = Guid.NewGuid();            
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