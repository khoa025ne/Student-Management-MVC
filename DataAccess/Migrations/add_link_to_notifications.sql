-- Migration: Thêm cột Link vào bảng Notifications
USE StudentManagementDB;

ALTER TABLE `Notifications` 
ADD COLUMN `Link` VARCHAR(200) NULL AFTER `CreatedAt`;

SELECT 'Migration completed: Added Link column to Notifications' AS Status;
