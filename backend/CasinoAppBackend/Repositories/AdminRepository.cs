using System.Linq.Expressions;
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<List<User>> GetAllUsersAdminsAsync()
        {
            return await context.Users
                .Where(u => u.UserRole == UserRole.Admin)
                .Include(u => u.Admin)
                .OrderByDescending(u => u.InsertedAt)
                .ToListAsync();
        }

        public async Task<List<Admin>> GetAdminsFilteredAsync(
            string? search, bool? isActive)
        {
            var predicates = new List<Expression<Func<Admin, bool>>>();

            IQueryable<Admin> query = context.Admins
                .Include(a => a.User);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                predicates.Add(a =>
                    EF.Functions.Like(a.User.Username, pattern) ||
                    EF.Functions.Like(a.User.Email, pattern));
            }

            if (isActive.HasValue)
                predicates.Add(a => a.User.IsActive == isActive.Value); 

            predicates.ForEach(predicate => query = query.Where(predicate));

            return await query
                .OrderByDescending(u => u.InsertedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<Admin>> GetPaginatedAdminsFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive)
        {
            int skip = (pageNumber - 1) * pageSize;
            var predicates = new List<Expression<Func<Admin, bool>>>();
            
            IQueryable<Admin> query = context.Admins
                .Include(a => a.User);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                predicates.Add(a =>
                    EF.Functions.Like(a.User.Username, pattern) ||
                    EF.Functions.Like(a.User.Email, pattern));
            }

            if (isActive.HasValue)
                predicates.Add(a => a.User.IsActive == isActive.Value);

            predicates.ForEach(predicate => query = query.Where(predicate));

            var totalRecords = await query.CountAsync();

            var adminsFiltered = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Admin>
            {
                Data = adminsFiltered,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Admin?> GetByIdAsync(Guid id) =>
            await context.Admins
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
