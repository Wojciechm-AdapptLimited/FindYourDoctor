using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InvalidOperationException = System.InvalidOperationException;

namespace FindYourDoctor.Data;

public class FindYourDoctorUserStore : IUserPasswordStore<User>, IUserEmailStore<User>, IUserRoleStore<User>, IQueryableUserStore<User>
{
    private bool _isDisposed;
    private readonly FindYourDoctorDbContext _context;
    private readonly IDbContextFactory<FindYourDoctorDbContext> _contextFactory;

    public IQueryable<User> Users => _context.Set<User>();
    
    public FindYourDoctorUserStore(FindYourDoctorDbContext context, IDbContextFactory<FindYourDoctorDbContext> contextFactory)
    {
        _context = context;
        _contextFactory = contextFactory;
    }
    
    #region DML

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }
    
    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        context.Attach(user);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        context.Update(user);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Could not update user {user.Email}." });
        }
        return IdentityResult.Success;
            
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        context.Remove(user);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Could not delete user {user.Email}." });
        }
        return IdentityResult.Success;
    }

    #endregion

    #region Setters

    public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
        
        user.UserName = userName;

        return Task.CompletedTask;
    }
    
    public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));
        
        user.NormalizedUserName = normalizedName;

        return Task.CompletedTask;
    }
    
    public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentNullException(nameof(passwordHash));
        
        user.PasswordHash = passwordHash;

        return Task.CompletedTask;
    }
    
    public Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        
        user.Email = email;

        return Task.CompletedTask;
    }
    
    public Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedEmail)) throw new ArgumentNullException(nameof(normalizedEmail));
        
        user.NormalizedEmail = normalizedEmail;

        return Task.CompletedTask;
    }
    
    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var account = await context.Accounts.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);

        if (account == null)
        {
            throw new InvalidOperationException($"Role {roleName} not found");
        }

        user.AccountType = account.Id;

        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var account = await context.Accounts.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);

        if (account == null)
        {
            throw new InvalidOperationException($"Role {roleName} not found");
        }

        user.AccountType = 1;

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Getters

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Id.ToString());
    }
    
    public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.UserName);
    }
    
    public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedUserName);
    }
    
    public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PasswordHash);
    }
    
    public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedEmail);
    }
    
    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var name = await context.Accounts
            .SingleOrDefaultAsync(x => x.Id == user.AccountType, cancellationToken: cancellationToken);

        return new List<string> {name?.Name ?? string.Empty};
    }

    #endregion

    #region Queries

    public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        if (!int.TryParse(userId, out var idInt)) throw new ArgumentException("Not valid id", nameof(userId));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Users.FindAsync(new object?[] {idInt}, cancellationToken);
    }
    
    public async Task<User?> FindByNameAsync(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (userName == null) throw new ArgumentNullException(nameof(userName));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Users.SingleOrDefaultAsync(x => x.NormalizedUserName == userName, cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (email == null) throw new ArgumentNullException(nameof(email));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == email, cancellationToken);
    }

    public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Users.AnyAsync(x => x.Id == user.Id && !string.IsNullOrEmpty(x.PasswordHash), cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await _context.Users
            .Include(x => x.AccountTypeNavigation)
            .AnyAsync(x => x.Id == user.Id && x.AccountTypeNavigation.Name == roleName, cancellationToken);
    }
    
    public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));
        
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var role = await context.Accounts.SingleOrDefaultAsync(x => x.Name == roleName, cancellationToken);

        if (role == null)
        {
            return new List<User>();
        }

        return await context.Accounts
            .Include(x => x.Users)
            .Where(x => x.Name == roleName)
            .SelectMany(x => x.Users)
            .ToListAsync(cancellationToken);
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