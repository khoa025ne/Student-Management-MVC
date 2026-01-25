-- Fix Foreign Key Constraint cho Notifications table
USE studentmanagementdb;

-- Xóa constraint cũ
ALTER TABLE notifications DROP FOREIGN KEY FK_Notifications_Students_StudentId;

-- Thêm lại constraint với ON DELETE CASCADE
ALTER TABLE notifications 
ADD CONSTRAINT FK_Notifications_Students_StudentId 
FOREIGN KEY (StudentId) 
REFERENCES students(StudentId) 
ON DELETE CASCADE;

-- Kiểm tra lại constraint
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    REFERENCED_TABLE_NAME,
    DELETE_RULE
FROM 
    information_schema.REFERENTIAL_CONSTRAINTS
WHERE 
    CONSTRAINT_SCHEMA = 'studentmanagementdb'
    AND TABLE_NAME = 'notifications';
