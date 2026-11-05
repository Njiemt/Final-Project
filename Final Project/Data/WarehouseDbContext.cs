using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WarehouseManagementSystem.Data;

public sealed class WarehouseDbContext : DbContext
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryComment> InventoryComments => Set<InventoryComment>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        string connectionString = configuration.GetConnectionString("Warehouse")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=Warehouse;Trusted_Connection=True;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(x => x.LocationId);
            entity.Property(x => x.LocationCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Aisle).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Rack).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Shelf).HasMaxLength(10).IsRequired();
            entity.Property(x => x.LocationDescription).HasMaxLength(150);
            entity.HasIndex(x => x.LocationCode).IsUnique();
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("InventoryItems");
            entity.HasCheckConstraint("CK_InventoryItems_ItemNumber_Positive", "[ItemNumber] > 0");
            entity.HasCheckConstraint("CK_InventoryItems_Quantity", "[Quantity] >= 0");
            entity.HasCheckConstraint("CK_InventoryItems_ReorderLevel", "[ReorderLevel] >= 0");
            entity.HasKey(x => x.ItemId);
            entity.Property(x => x.ItemNumber).IsRequired();
            entity.Property(x => x.ItemName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ItemDescription).HasMaxLength(500);
            entity.Property(x => x.ConditionName).HasMaxLength(30).IsRequired();
            entity.Property(x => x.DateAdded).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(x => x.LastUpdated).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(x => x.ItemNumber).IsUnique();
            entity.HasOne(x => x.Location)
                .WithMany(x => x.InventoryItems)
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryComment>(entity =>
        {
            entity.ToTable("InventoryComments");
            entity.HasKey(x => x.CommentId);
            entity.Property(x => x.CommentText).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.CommentedBy).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CommentDate).HasDefaultValueSql("SYSDATETIME()");
            entity.HasOne(x => x.Item)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.ToTable("InventoryTransactions");
            entity.HasCheckConstraint("CK_InventoryTransactions_Type", "[TransactionType] IN ('INSERT', 'UPDATE')");
            entity.HasKey(x => x.TransactionId);
            entity.Property(x => x.TransactionType).HasMaxLength(20).IsRequired();
            entity.Property(x => x.ConditionBefore).HasMaxLength(30);
            entity.Property(x => x.ConditionAfter).HasMaxLength(30);
            entity.Property(x => x.ChangedBy).HasMaxLength(128).IsRequired();
            entity.Property(x => x.TransactionDate).HasDefaultValueSql("SYSDATETIME()");
            entity.HasOne(x => x.Item)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.ToTable("TeamMembers");
            entity.HasCheckConstraint("CK_TeamMembers_YearInProgram", "[YearInProgram] >= 1");
            entity.HasKey(x => x.MemberId);
            entity.Property(x => x.EmployeeId).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.CollegeProgram).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.HasIndex(x => x.EmployeeId).IsUnique();
        });

        modelBuilder.Entity<Location>().HasData(
            new Location { LocationId = 1, LocationCode = "A-01-01", Aisle = "A", Rack = "01", Shelf = "01", LocationDescription = "Front storage rack", IsActive = true },
            new Location { LocationId = 2, LocationCode = "A-01-02", Aisle = "A", Rack = "01", Shelf = "02", LocationDescription = "Second shelf", IsActive = true },
            new Location { LocationId = 3, LocationCode = "B-02-01", Aisle = "B", Rack = "02", Shelf = "01", LocationDescription = "Electronics storage", IsActive = true },
            new Location { LocationId = 4, LocationCode = "C-01-01", Aisle = "C", Rack = "01", Shelf = "01", LocationDescription = "Repair area", IsActive = true });

        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { ItemId = 1, ItemNumber = 1, ItemName = "Business Laptop", ItemDescription = "14-inch business laptop", LocationId = 3, ConditionName = "Good", Quantity = 8, ReorderLevel = 3, DateAdded = new DateTime(2024, 12, 1), LastUpdated = new DateTime(2024, 12, 1) },
            new InventoryItem { ItemId = 2, ItemNumber = 2, ItemName = "USB-C Cable", ItemDescription = "1-meter charging cable", LocationId = 1, ConditionName = "New", Quantity = 25, ReorderLevel = 10, DateAdded = new DateTime(2024, 12, 1), LastUpdated = new DateTime(2024, 12, 1) },
            new InventoryItem { ItemId = 3, ItemNumber = 3, ItemName = "LED Monitor", ItemDescription = "24-inch LED monitor", LocationId = 2, ConditionName = "Fair", Quantity = 4, ReorderLevel = 5, DateAdded = new DateTime(2024, 12, 1), LastUpdated = new DateTime(2024, 12, 1) },
            new InventoryItem { ItemId = 4, ItemNumber = 4, ItemName = "Inspection Laptop", ItemDescription = "Awaiting diagnostic testing", LocationId = 4, ConditionName = "Damaged", Quantity = 1, ReorderLevel = 1, DateAdded = new DateTime(2024, 12, 1), LastUpdated = new DateTime(2024, 12, 1) });

        modelBuilder.Entity<InventoryComment>().HasData(
            new InventoryComment { CommentId = 1, ItemId = 1, CommentText = "Tested and ready for deployment.", CommentedBy = "Warehouse Admin", CommentDate = new DateTime(2024, 12, 1) },
            new InventoryComment { CommentId = 2, ItemId = 3, CommentText = "Low stock; check condition before issuing.", CommentedBy = "Warehouse Admin", CommentDate = new DateTime(2024, 12, 1) },
            new InventoryComment { CommentId = 3, ItemId = 4, CommentText = "Hold for inspection.", CommentedBy = "Warehouse Admin", CommentDate = new DateTime(2024, 12, 1) });

        modelBuilder.Entity<TeamMember>().HasData(
            new TeamMember
            {
                MemberId = 1,
                EmployeeId = 1,
                FullName = "Mohamed Njie",
                Birthdate = new DateTime(2007, 7, 2),
                CollegeProgram = "IT Software Development",
                YearInProgram = 2,
                Email = "njiemt@mail.uc.edu"
            });
    }
}

public sealed class Location
{
    public int LocationId { get; set; }
    public string LocationCode { get; set; } = "";
    public string Aisle { get; set; } = "";
    public string Rack { get; set; } = "";
    public string Shelf { get; set; } = "";
    public string? LocationDescription { get; set; }
    public bool IsActive { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}

public sealed class InventoryItem
{
    public int ItemId { get; set; }
    public int ItemNumber { get; set; }
    public string ItemName { get; set; } = "";
    public string? ItemDescription { get; set; }
    public int LocationId { get; set; }
    public string ConditionName { get; set; } = "";
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime LastUpdated { get; set; }
    public Location Location { get; set; } = null!;
    public ICollection<InventoryComment> Comments { get; set; } = new List<InventoryComment>();
    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}

public sealed class InventoryComment
{
    public int CommentId { get; set; }
    public int ItemId { get; set; }
    public string CommentText { get; set; } = "";
    public DateTime CommentDate { get; set; }
    public string CommentedBy { get; set; } = "";
    public InventoryItem Item { get; set; } = null!;
}

public sealed class InventoryTransaction
{
    public int TransactionId { get; set; }
    public int ItemId { get; set; }
    public string TransactionType { get; set; } = "";
    public int? QuantityBefore { get; set; }
    public int? QuantityAfter { get; set; }
    public string? ConditionBefore { get; set; }
    public string? ConditionAfter { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ChangedBy { get; set; } = "";
    public InventoryItem Item { get; set; } = null!;
}

public sealed class TeamMember
{
    public int MemberId { get; set; }
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = "";
    public DateTime Birthdate { get; set; }
    public string CollegeProgram { get; set; } = "";
    public int YearInProgram { get; set; }
    public string? Email { get; set; }
}
