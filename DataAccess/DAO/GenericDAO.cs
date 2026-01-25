using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cơ sở - Xử lý truy xuất dữ liệu chung
    /// </summary>
    /// <typeparam name="T">Kiểu Entity</typeparam>
    public class GenericDAO<T> where T : class
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        public GenericDAO(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả records
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy record theo ID
        /// </summary>
        public async Task<T?> GetByIdAsync(object id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy {typeof(T).Name} theo ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách theo điều kiện
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _context.Set<T>().Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy 1 record theo điều kiện
        /// </summary>
        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _context.Set<T>().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm mới record
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật record
        /// </summary>
        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa record
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _context.Set<T>().AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tồn tại {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đếm số lượng record
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                if (predicate == null)
                    return await _context.Set<T>().CountAsync();
                return await _context.Set<T>().CountAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy DbSet để query nâng cao
        /// </summary>
        public DbSet<T> GetDbSet()
        {
            return _context.Set<T>();
        }
    }
}
