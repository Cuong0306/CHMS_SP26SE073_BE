

namespace CHMS.DAL.Common;

using CHMS.DAL.Entities;
using CHMS.DAL.Repositories.Interfaces;

/// <summary>
/// Interface cho Unit of Work pattern
/// Quản lý tất cả repositories và đảm bảo save changes trong 1 transaction
/// </summary>
public interface IUnitOfWork : IDisposable
{
    #region Repositories

    // Thêm các repository interface ở đây khi tạo
    IUserRepository Users { get; }
    // IHomestayRepository Homestays { get; }
    // IBookingRepository Bookings { get; }
    // IReviewRepository Reviews { get; }
    // IPaymentRepository Payments { get; }
    // ... các repositories khác
    IGenericRepository<UserRole> UserRoles { get; }

    IGenericRepository<Role> Roles { get; }

    IGenericRepository<RefreshToken> RefreshTokens { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Lưu tất cả thay đổi vào database (sync)
    /// </summary>
    int SaveChanges();

    /// <summary>
    /// Bắt đầu transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit transaction
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rollback transaction
    /// </summary>
    Task RollbackTransactionAsync();

    #endregion
}