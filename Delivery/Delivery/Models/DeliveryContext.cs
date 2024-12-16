using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Models;

public class DeliveryContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketDish> BasketDishes { get; set; }
    
    public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Dish>()
            .HasOne(d => d.Establishments)
            .WithMany(e => e.Dishes)
            .HasForeignKey(d => d.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}