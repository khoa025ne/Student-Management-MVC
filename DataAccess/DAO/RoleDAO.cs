using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Role
    /// </summary>
    public class RoleDAO
    {
        private readonly AppDbContext _context;

        public RoleDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách vai trò: {ex.Message}", ex);
            }
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Roles.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy vai trò theo ID: {ex.Message}", ex);
            }
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            try
            {
                return await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy vai trò theo tên: {ex.Message}", ex);
            }
        }

        public async Task<Role> AddAsync(Role role)
        {
            try
            {
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm vai trò: {ex.Message}", ex);
            }
        }
    }
}
