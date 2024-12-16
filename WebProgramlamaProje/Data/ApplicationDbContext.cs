using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Salon> Salons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee ve Service arasındaki Many-to-Many ilişkiyi tanımla
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Services)
            .WithMany(s => s.Employees)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeService",
                j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"),
                j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId")
            );
    }
}