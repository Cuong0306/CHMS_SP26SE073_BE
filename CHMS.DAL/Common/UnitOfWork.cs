using CHMS.DAL.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CHMS.DAL.Common;

/// <summary>
/// Implementation cho Unit of Work pattern
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CoastalHomestayDBContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    public UnitOfWork(CoastalHomestayDBContext context)
    {
        _context = context;
    }

    #region Repositories

    // Lazy loading repositories - chỉ tạo khi cần
    // private IUserRepository? _userRepository;
    // private IHomestayRepository? _homestayRepository;
    // private IBookingRepository? _bookingRepository;

    // public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    // public IHomestayRepository Homestays => _homestayRepository ??= new HomestayRepository(_context);
    // public IBookingRepository Bookings => _bookingRepository ??= new BookingRepository(_context);

    #endregion

    #region Methods

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #endregion

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}