using MySqlConnector;

Console.WriteLine("Đang kết nối database...");

var connectionString = "Server=localhost;Database=StudentManagementDB;User=root;Password=12345;";

try
{
    using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("✓ Kết nối thành công!");
    
    // Bước 1: Chuyển đổi dữ liệu text sang số
    Console.WriteLine("Đang chuyển đổi dữ liệu Major...");
    using var cmd1 = new MySqlCommand(@"
        UPDATE Students 
        SET Major = CASE 
            WHEN Major IS NULL OR Major = '' THEN '0'
            WHEN Major REGEXP '^[0-9]+$' THEN Major
            ELSE '0'
        END;
    ", connection);
    await cmd1.ExecuteNonQueryAsync();
    Console.WriteLine("✓ Đã chuyển đổi dữ liệu.");
    
    // Bước 2: Thay đổi kiểu cột
    Console.WriteLine("Đang thay đổi kiểu dữ liệu cột Major...");
    using var cmd2 = new MySqlCommand(@"
        ALTER TABLE Students 
        MODIFY COLUMN Major int NOT NULL;
    ", connection);
    await cmd2.ExecuteNonQueryAsync();
    
    Console.WriteLine("✓ Hoàn thành! Cột Major đã được chuyển từ text sang int.");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Lỗi: {ex.Message}");
}
