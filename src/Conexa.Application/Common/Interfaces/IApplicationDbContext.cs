using Conexa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Movie> Movies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
