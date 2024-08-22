using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace SafeEscape.Models
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomEdge> RoomEdges { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Exit> Exits { get; set; }
        public DbSet<Stair> Stairs { get; set; }
        public DbSet<GeoJsonFloorData> GeoJsonFloorData { get; set; }
        public DbSet<UserBuilding> UserBuildings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoomEdge>()
                .HasOne(re => re.Room1)
                .WithMany(r => r.RoomEdges1)
                .HasForeignKey(re => re.Room1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomEdge>()
                .HasOne(re => re.Room2)
                .WithMany(r => r.RoomEdges2)
                .HasForeignKey(re => re.Room2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBuilding>()
               .HasKey(ub => new { ub.UserId, ub.BuildingId });

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<Building>()
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Floor>()
                .HasMany(f => f.Rooms)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Sensors)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.RoomEdges1)
                .WithOne(re => re.Room1)
                .HasForeignKey(re => re.Room1Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.RoomEdges2)
                .WithOne(re => re.Room2)
                .HasForeignKey(re => re.Room2Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Building>()
                .HasMany(b => b.UserBuildings)
                .WithOne(ub => ub.Building)
                .HasForeignKey(ub => ub.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Building>()
                .HasMany(b => b.Exits)
                .WithOne(e => e.Building)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Building>()
                .HasMany(b => b.Stairs)
                .WithOne(s => s.Building)
                .HasForeignKey(s => s.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Building>()
                .HasMany(b => b.GeoJsonFloorData)
                .WithOne(g => g.Building)
                .HasForeignKey(g => g.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
