using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Data;

public class FindYourDoctorRoleStore : IQueryableRoleStore<Account>
{
    private bool _isDisposed;
    private readonly FindYourDoctorDbContext _context;
    
    public IQueryable<Account> Roles => _context.Set<Account>();

    public FindYourDoctorRoleStore(FindYourDoctorDbContext context)
    {
        _context = context;
    }
    
    #region DML

    public async Task<IdentityResult> CreateAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));

        _context.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        _context.Attach(role);
        role.ConcurrencyStamp = Guid.NewGuid().ToString();
        _context.Update(role);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Could not update role {role.Name}." });
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));
        
        _context.Remove(role);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Could not delete role {role.Name}." });
        }
        
        return IdentityResult.Success;
    }

    #endregion

    #region Setters

    public Task SetRoleNameAsync(Account role, string? roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        role.Name = roleName;

        return Task.CompletedTask;
    }
    
    public Task SetNormalizedRoleNameAsync(Account role, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (string.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));

        role.NormalizedName = normalizedName;

        return Task.CompletedTask;
    }

    #endregion

    #region Getters

    public Task<string> GetRoleIdAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.Name);
    }
    
    public Task<string?> GetNormalizedRoleNameAsync(Account role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null) throw new ArgumentNullException(nameof(role));

        return Task.FromResult(role.Name);
    }

    #endregion

    #region Queries

    public Task<Account?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (roleId == null) throw new ArgumentNullException(nameof(roleId));
        if (!int.TryParse(roleId, out var idInt)) throw new ArgumentException("Not valid id", nameof(roleId));
        
        return _context.Accounts.SingleOrDefaultAsync(x => x.Id == idInt, cancellationToken);
    }

    public Task<Account?> FindByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (roleName == null) throw new ArgumentNullException(nameof(roleName));
        
        return _context.Accounts.SingleOrDefaultAsync(x => x.NormalizedName == roleName, cancellationToken);
    }
    
    #endregion
    
    public void Dispose()
    {
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

}