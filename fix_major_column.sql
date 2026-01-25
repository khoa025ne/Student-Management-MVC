-- Script để chuyển đổi cột Major từ text sang int

USE StudentManagementDB;

-- Bước 1: Chuyển đổi dữ liệu hiện có từ text sang số
UPDATE Students 
SET Major = CASE 
    WHEN Major IS NULL OR Major = '' THEN '0'
    WHEN Major REGEXP '^[0-9]+$' THEN Major
    ELSE '0'
END;

-- Bước 2: Thay đổi kiểu dữ liệu của cột Major từ longtext sang int
ALTER TABLE Students 
MODIFY COLUMN Major int NOT NULL;

-- Kiểm tra kết quả
SELECT 'Column Major has been converted to int successfully' AS Result;
DESCRIBE Students;
