using IIS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IIS.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Borrow> Reservations => Set<Borrow>();
    public DbSet<Studio> Studios => Set<Studio>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<RentalDayInterval> RentalDayIntervals => Set<RentalDayInterval>();
    public DbSet<User> Users => Set<User>();


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Borrow>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Borrow>()
            .HasOne(b => b.User)
            .WithMany(u => u.BorrowedEquipment)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict); // depends if we want to keep the user's borrow history

        modelBuilder.Entity<Borrow>()
            .HasOne(b => b.Equipment)
            .WithMany(e => e.Borrows)
            .HasForeignKey(b => b.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Equipment>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Studio)
            .WithMany(s => s.OwnedEquipment)
            .HasForeignKey(e => e.StudioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.EquipmentType)
            .WithMany(et => et.EquipmentItems)
            .HasForeignKey(e => e.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Equipment>()
            .HasMany(e => e.UsersForbiddenToBorrow)
            .WithMany(u => u.AssignedStudio.OwnedEquipment);

        modelBuilder.Entity<RentalDayInterval>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<RentalDayInterval>()
            .HasOne(r => r.Equipment)
            .WithMany(e => e.RentalDayIntervals)
            .HasForeignKey(r => r.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EquipmentType>()
            .HasKey(et => et.Id);

        modelBuilder.Entity<Studio>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Studio>()
            .HasMany(s => s.UsersAssigned)
            .WithOne(u => u.AssignedStudio)
            .OnDelete(DeleteBehavior.SetNull);
    }
}