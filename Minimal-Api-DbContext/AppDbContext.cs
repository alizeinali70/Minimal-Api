using Microsoft.EntityFrameworkCore;
using MinimalApiDataClasse;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalApiDbContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

  public DbSet<Product> Products { get; set; }
}
