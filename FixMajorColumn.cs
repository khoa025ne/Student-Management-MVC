using Microsoft.EntityFrameworkCore;
using DataAccess;

// Script để fix cột Major từ text sang int
var connectionString = "Server=localhost;Database=StudentManagementDB;User=root;Password=12345;";

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33)));

using (var context = new AppDbContext(optionsBuilder.Options))
{
    Console.WriteLine("Đang chuyển đổi cột Major...");
    
    try
    {
        // Bước 1: Chuyển đổi dữ liệu text sang số
        await context.Database.ExecuteSqlRawAsync(@"
            UPDATE Students 
            SET Major = CASE 
                WHEN Major IS NULL OR Major = '' THEN '0'
                WHEN Major REGEXP '^[0-9]+$' THEN Major
                ELSE '0'
            END;
        ");
        
        Console.WriteLine("Đã chuyển đổi dữ liệu Major.");
        
        // Bước 2: Thay đổi kiểu cột
        await context.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE Students 
            MODIFY COLUMN Major int NOT NULL;
        ");
        
        Console.WriteLine("✓ Đã chuyển đổi cột Major từ text sang int thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Lỗi: {ex.Message}");
    }
}
