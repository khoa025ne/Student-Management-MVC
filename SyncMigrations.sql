-- Script to sync migration history with existing database
-- Run this in MySQL Workbench or phpMyAdmin against StudentManagementDB

-- First, ensure the migration history table exists
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

-- Insert existing migrations that have already been applied (tables exist)
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20260117021725_InitialCreate', '8.0.0'),
('20260121050439_ChangeMajorToEnum', '8.0.0'),
('20260124070435_ConfigureNotificationCascadeDelete', '8.0.0'),
('20260125_AddEnrollmentComment', '8.0.0');

-- After running this, run: dotnet ef database update --project DataAccess --startup-project StudentManagementMVC
-- This will apply only the TailAdminMigration
