-- Migration: Tạo bảng LearningPathRecommendations
-- Mục đích: Lưu trữ gợi ý lộ trình học tập từ AI cho sinh viên

USE StudentManagementDB;

-- Tạo bảng LearningPathRecommendations
CREATE TABLE IF NOT EXISTS `LearningPathRecommendations` (
    `RecommendationId` INT AUTO_INCREMENT PRIMARY KEY,
    `StudentId` INT NOT NULL,
    `SemesterId` INT NOT NULL,
    `RecommendationDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `AiModelUsed` VARCHAR(50) DEFAULT 'Gemini-1.5-Pro',
    `RecommendedCoursesJson` JSON NOT NULL,
    `OverallStrategy` TEXT,
    `WarningsJson` JSON,
    `IsViewed` BOOLEAN DEFAULT FALSE,
    `ViewedAt` DATETIME NULL,
    `ViewCount` INT DEFAULT 0,
    `IsFollowed` BOOLEAN NULL,
    
    -- Foreign Keys
    CONSTRAINT `FK_LearningPath_Student` 
        FOREIGN KEY (`StudentId`) REFERENCES `Students`(`StudentId`) ON DELETE CASCADE,
    CONSTRAINT `FK_LearningPath_Semester` 
        FOREIGN KEY (`SemesterId`) REFERENCES `Semesters`(`SemesterId`) ON DELETE CASCADE,
    
    -- Indexes
    INDEX `IDX_LearningPath_Student` (`StudentId`),
    INDEX `IDX_LearningPath_Semester` (`SemesterId`),
    INDEX `IDX_LearningPath_Date` (`RecommendationDate` DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Thêm comment cho bảng
ALTER TABLE `LearningPathRecommendations` 
    COMMENT = 'Lưu trữ gợi ý lộ trình học tập từ AI cho sinh viên';

SELECT 'Migration completed: LearningPathRecommendations table created' AS Status;
