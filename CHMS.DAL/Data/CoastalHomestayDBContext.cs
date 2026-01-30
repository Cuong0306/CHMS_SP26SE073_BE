using CHMS.DAL.Entities;
using CHMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHMS.DAL.Data;

public class CoastalHomestayDBContext : DbContext
{
    public CoastalHomestayDBContext(DbContextOptions<CoastalHomestayDBContext> options)
        : base(options)
    {
    }

    #region DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<VerificationDocument> VerificationDocuments => Set<VerificationDocument>();

    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<Homestay> Homestays => Set<Homestay>();
    public DbSet<HomestayImage> HomestayImages => Set<HomestayImage>();
    public DbSet<HomestayAmenity> HomestayAmenities => Set<HomestayAmenity>();

    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<SeasonalPricing> SeasonalPricings => Set<SeasonalPricing>();

    public DbSet<Promotion> Promotions => Set<Promotion>();

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Cancellation> Cancellations => Set<Cancellation>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewImage> ReviewImages => Set<ReviewImage>();

    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketReply> TicketReplies => Set<TicketReply>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== GLOBAL SETTINGS ==========
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        // ========== GLOBAL QUERY FILTERS (Soft Delete) ==========
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Homestay>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Booking>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Promotion>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Amenity>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<SupportTicket>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Permission>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<VerificationDocument>().HasQueryFilter(x => !x.IsDeleted);

        // ========== USER & AUTH ==========
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("ACTIVE");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.DeletedByNavigation)
                .WithMany(p => p.InverseDeletedByNavigation)
                .HasForeignKey(d => d.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.DeviceInfo).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VerificationDocument>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DocumentUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
            entity.Property(e => e.RejectReason).HasMaxLength(500);
            entity.Property(e => e.SubmissionDate).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.VerificationDocumentUsers)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.VerificationDocumentReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // ========== LOCATION ==========
        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)");

            entity.HasOne(d => d.District).WithMany(p => p.Locations)
                .HasForeignKey(d => d.DistrictId).OnDelete(DeleteBehavior.Restrict);
        });

        // ========== HOMESTAY ==========
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IconUrl).HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Homestay>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Area).HasColumnType("decimal(8,2)");
            entity.Property(e => e.MaxGuests).HasDefaultValue(2);
            entity.Property(e => e.Bedrooms).HasDefaultValue(1);
            entity.Property(e => e.Bathrooms).HasDefaultValue(1);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Owner).WithMany(p => p.HomestayOwners)
                .HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Location).WithMany(p => p.Homestays)
                .HasForeignKey(d => d.LocationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.HomestayDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HomestayImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Homestay).WithMany(p => p.HomestayImages)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HomestayAmenity>(entity =>
        {
            entity.HasKey(e => new { e.HomestayId, e.AmenityId });

            entity.HasOne(d => d.Homestay).WithMany(p => p.HomestayAmenities)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Amenity).WithMany(p => p.HomestayAmenities)
                .HasForeignKey(d => d.AmenityId).OnDelete(DeleteBehavior.Cascade);
        });

        // ========== AVAILABILITY & PRICING ==========
        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.HomestayId, e.Date }).IsUnique();

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.PriceOverride).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Homestay).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SeasonalPricing>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(12,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Homestay).WithMany(p => p.SeasonalPricings)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Cascade);
        });

        // ========== PROMOTION ==========
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.MinBookingAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CurrentUsage).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        // ========== BOOKING ==========
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.GuestsCount).HasDefaultValue(1);
            entity.Property(e => e.PricePerNightAtBooking).HasColumnType("decimal(12,2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(12,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(12,2)");
            entity.Property(e => e.SpecialRequests).HasMaxLength(500);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingCustomers)
                .HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Homestay).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Promotion).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PromotionId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.BookingDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BookingStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OldStatus).HasMaxLength(20);
            entity.Property(e => e.NewStatus).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingStatusHistories)
                .HasForeignKey(d => d.BookingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.BookingStatusHistories)
                .HasForeignKey(d => d.ChangedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Method).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Amount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cancellation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.RefundStatus).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Booking).WithMany(p => p.Cancellations)
                .HasForeignKey(d => d.BookingId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.Cancellations)
                .HasForeignKey(d => d.CancelledBy).OnDelete(DeleteBehavior.Restrict);
        });

        // ========== REVIEW ==========
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BookingId).IsUnique();

            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.IsVerified).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Booking).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.BookingId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Customer).WithMany(p => p.ReviewCustomers)
                .HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Homestay).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.ReviewDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages)
                .HasForeignKey(d => d.ReviewId).OnDelete(DeleteBehavior.Cascade);
        });

        // ========== WISHLIST ==========
        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.HomestayId });
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Homestay).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Cascade);
        });

        // ========== SUPPORT ==========
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(20).HasDefaultValue("NORMAL");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("OPEN");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTicketUsers)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Staff).WithMany(p => p.SupportTicketStaffs)
                .HasForeignKey(d => d.StaffId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Booking).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.BookingId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.TicketId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.User).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ========== NOTIFICATION & CHAT ==========
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Homestay).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.HomestayId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Guest).WithMany(p => p.ConversationGuests)
                .HasForeignKey(d => d.GuestId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Host).WithMany(p => p.ConversationHosts)
                .HasForeignKey(d => d.HostId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Booking).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.BookingId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SessionId).HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Sender).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.ChatHistories)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        // ========== AUDIT LOG ==========
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Entity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    // ========== OVERRIDE SAVECHANGES FOR AUDIT ==========
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified); // Chỉ lấy các dòng đang sửa

        foreach (var entry in entries)
        {
            // 👇 SỬA DÒNG NÀY: Dùng Metadata.FindProperty để kiểm tra an toàn
            var hasUpdatedAt = entry.Metadata.FindProperty("UpdatedAt");

            if (hasUpdatedAt != null)
            {
                // Nếu có cột UpdatedAt thì mới gán giá trị
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}