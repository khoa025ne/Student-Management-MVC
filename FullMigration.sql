CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Courses` (
        `CourseId` int NOT NULL AUTO_INCREMENT,
        `CourseName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `CourseCode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Credits` int NOT NULL,
        `Major` longtext CHARACTER SET utf8mb4 NULL,
        `PrerequisiteCourseId` int NULL,
        CONSTRAINT `PK_Courses` PRIMARY KEY (`CourseId`),
        CONSTRAINT `FK_Courses_Courses_PrerequisiteCourseId` FOREIGN KEY (`PrerequisiteCourseId`) REFERENCES `Courses` (`CourseId`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `RoleId` int NOT NULL AUTO_INCREMENT,
        `RoleName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Description` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`RoleId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Semesters` (
        `SemesterId` int NOT NULL AUTO_INCREMENT,
        `SemesterName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `SemesterCode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Semesters` PRIMARY KEY (`SemesterId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Users` (
        `UserId` int NOT NULL AUTO_INCREMENT,
        `Email` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `PasswordHash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `FullName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `LastLogin` datetime(6) NULL,
        `IsActive` tinyint(1) NOT NULL,
        `MustChangePassword` tinyint(1) NOT NULL,
        `GoogleId` longtext CHARACTER SET utf8mb4 NULL,
        `PasswordChangedAt` datetime(6) NULL,
        `RefreshToken` longtext CHARACTER SET utf8mb4 NULL,
        `RefreshTokenExpiryTime` datetime(6) NULL,
        `AvatarUrl` longtext CHARACTER SET utf8mb4 NULL,
        `RoleId` int NOT NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`UserId`),
        CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`RoleId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Classes` (
        `ClassId` int NOT NULL AUTO_INCREMENT,
        `ClassName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ClassCode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `MaxCapacity` int NOT NULL,
        `CurrentEnrollment` int NOT NULL,
        `Room` longtext CHARACTER SET utf8mb4 NULL,
        `Schedule` longtext CHARACTER SET utf8mb4 NULL,
        `DayOfWeekPair` int NOT NULL,
        `TimeSlot` int NOT NULL,
        `CourseId` int NOT NULL,
        `SemesterId` int NOT NULL,
        `TeacherId` int NULL,
        CONSTRAINT `PK_Classes` PRIMARY KEY (`ClassId`),
        CONSTRAINT `FK_Classes_Courses_CourseId` FOREIGN KEY (`CourseId`) REFERENCES `Courses` (`CourseId`) ON DELETE CASCADE,
        CONSTRAINT `FK_Classes_Semesters_SemesterId` FOREIGN KEY (`SemesterId`) REFERENCES `Semesters` (`SemesterId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Students` (
        `StudentId` int NOT NULL AUTO_INCREMENT,
        `StudentCode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `FullName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `DateOfBirth` datetime(6) NOT NULL,
        `ClassCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `OverallGPA` double NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `Major` longtext CHARACTER SET utf8mb4 NOT NULL,
        `AvatarUrl` longtext CHARACTER SET utf8mb4 NULL,
        `CurrentTermNo` int NULL,
        `IsFirstLogin` tinyint(1) NOT NULL,
        `UserId` int NOT NULL,
        CONSTRAINT `PK_Students` PRIMARY KEY (`StudentId`),
        CONSTRAINT `FK_Students_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `AcademicAnalyses` (
        `AnalysisId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NOT NULL,
        `AnalysisDate` datetime(6) NOT NULL,
        `OverallGPA` double NOT NULL,
        `StrongSubjectsJson` longtext CHARACTER SET utf8mb4 NULL,
        `WeakSubjectsJson` longtext CHARACTER SET utf8mb4 NULL,
        `Recommendations` longtext CHARACTER SET utf8mb4 NULL,
        `AiModelUsed` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AcademicAnalyses` PRIMARY KEY (`AnalysisId`),
        CONSTRAINT `FK_AcademicAnalyses_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Enrollments` (
        `EnrollmentId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NOT NULL,
        `ClassId` int NOT NULL,
        `EnrollmentDate` datetime(6) NOT NULL,
        `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
        `MidtermScore` double NULL,
        `FinalScore` double NULL,
        `TotalScore` double NULL,
        `Grade` longtext CHARACTER SET utf8mb4 NULL,
        `IsPassed` tinyint(1) NOT NULL,
        `AttemptNumber` int NOT NULL,
        CONSTRAINT `PK_Enrollments` PRIMARY KEY (`EnrollmentId`),
        CONSTRAINT `FK_Enrollments_Classes_ClassId` FOREIGN KEY (`ClassId`) REFERENCES `Classes` (`ClassId`) ON DELETE CASCADE,
        CONSTRAINT `FK_Enrollments_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `LearningPathRecommendations` (
        `RecommendationId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NOT NULL,
        `SemesterId` int NOT NULL,
        `RecommendationDate` datetime(6) NOT NULL,
        `RecommendedCoursesJson` longtext CHARACTER SET utf8mb4 NULL,
        `OverallStrategy` longtext CHARACTER SET utf8mb4 NULL,
        `WarningsJson` longtext CHARACTER SET utf8mb4 NULL,
        `AiModelUsed` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsViewed` tinyint(1) NOT NULL,
        CONSTRAINT `PK_LearningPathRecommendations` PRIMARY KEY (`RecommendationId`),
        CONSTRAINT `FK_LearningPathRecommendations_Semesters_SemesterId` FOREIGN KEY (`SemesterId`) REFERENCES `Semesters` (`SemesterId`) ON DELETE CASCADE,
        CONSTRAINT `FK_LearningPathRecommendations_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Notifications` (
        `NotificationId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NULL,
        `Title` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Message` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsRead` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_Notifications` PRIMARY KEY (`NotificationId`),
        CONSTRAINT `FK_Notifications_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE TABLE `Scores` (
        `ScoreId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NOT NULL,
        `CourseId` int NOT NULL,
        `ScoreValue` double NOT NULL,
        CONSTRAINT `PK_Scores` PRIMARY KEY (`ScoreId`),
        CONSTRAINT `FK_Scores_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_AcademicAnalyses_StudentId` ON `AcademicAnalyses` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Classes_CourseId` ON `Classes` (`CourseId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Classes_SemesterId` ON `Classes` (`SemesterId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Courses_CourseCode` ON `Courses` (`CourseCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Courses_PrerequisiteCourseId` ON `Courses` (`PrerequisiteCourseId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Enrollments_ClassId` ON `Enrollments` (`ClassId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Enrollments_StudentId` ON `Enrollments` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_LearningPathRecommendations_SemesterId` ON `LearningPathRecommendations` (`SemesterId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_LearningPathRecommendations_StudentId` ON `LearningPathRecommendations` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Notifications_StudentId` ON `Notifications` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Scores_StudentId` ON `Scores` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Semesters_SemesterCode` ON `Semesters` (`SemesterCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Students_StudentCode` ON `Students` (`StudentCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Students_UserId` ON `Students` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    CREATE INDEX `IX_Users_RoleId` ON `Users` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260117021725_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260117021725_InitialCreate', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260121050439_ChangeMajorToEnum') THEN


                    UPDATE Students 
                    SET Major = CASE 
                        WHEN Major IS NULL OR Major = '' THEN '0'
                        WHEN Major REGEXP '^[0-9]+$' THEN Major
                        ELSE '0'
                    END;
                

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260121050439_ChangeMajorToEnum') THEN

    ALTER TABLE `Students` MODIFY COLUMN `Major` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260121050439_ChangeMajorToEnum') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260121050439_ChangeMajorToEnum', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260124070435_ConfigureNotificationCascadeDelete') THEN

    ALTER TABLE `Notifications` DROP FOREIGN KEY `FK_Notifications_Students_StudentId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260124070435_ConfigureNotificationCascadeDelete') THEN

    ALTER TABLE `Notifications` MODIFY COLUMN `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260124070435_ConfigureNotificationCascadeDelete') THEN

    ALTER TABLE `Notifications` MODIFY COLUMN `Message` varchar(1000) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260124070435_ConfigureNotificationCascadeDelete') THEN

    ALTER TABLE `Notifications` ADD CONSTRAINT `FK_Notifications_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260124070435_ConfigureNotificationCascadeDelete') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260124070435_ConfigureNotificationCascadeDelete', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260125_AddEnrollmentComment') THEN

    ALTER TABLE `Enrollments` ADD `Comment` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260125_AddEnrollmentComment') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260125_AddEnrollmentComment', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` MODIFY COLUMN `CourseId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `Assignment1Score` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `Assignment2Score` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `AttendanceScore` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `ClassId` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `Comment` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `CreatedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `EnrollmentId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `FinalExamScore` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `Grade` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `MidtermScore` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `ModifiedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD `TotalScore` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Notifications` ADD `CreatedBy` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Notifications` ADD `Link` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Notifications` ADD `Priority` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Notifications` ADD `ReadAt` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Notifications` ADD `TeacherId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Classes` ADD `CreatedAt` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Classes` ADD `MaxStudents` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE TABLE `AIConversationLogs` (
        `LogId` int NOT NULL AUTO_INCREMENT,
        `StudentId` int NULL,
        `UserId` int NULL,
        `RequestType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Prompt` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Response` longtext CHARACTER SET utf8mb4 NULL,
        `UsedKnowledgeIds` longtext CHARACTER SET utf8mb4 NULL,
        `ModelUsed` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `TokensUsed` int NOT NULL,
        `ProcessingTimeMs` int NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `ErrorMessage` longtext CHARACTER SET utf8mb4 NULL,
        `UserRating` int NULL,
        `UserFeedback` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_AIConversationLogs` PRIMARY KEY (`LogId`),
        CONSTRAINT `FK_AIConversationLogs_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE SET NULL,
        CONSTRAINT `FK_AIConversationLogs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE TABLE `AIKnowledgeBases` (
        `KnowledgeId` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Content` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Category` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `SubCategory` varchar(100) CHARACTER SET utf8mb4 NULL,
        `Tags` varchar(500) CHARACTER SET utf8mb4 NULL,
        `Priority` int NOT NULL,
        `UsageCount` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `Language` varchar(10) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'vi',
        `MetadataJson` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AIKnowledgeBases` PRIMARY KEY (`KnowledgeId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE TABLE `DashboardMetrics` (
        `MetricId` int NOT NULL AUTO_INCREMENT,
        `MetricName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `CurrentValue` double NOT NULL,
        `PreviousValue` double NULL,
        `ChangePercentage` double NULL,
        `Trend` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Unit` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Category` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `IconClass` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ColorClass` varchar(20) CHARACTER SET utf8mb4 NULL,
        `SemesterId` int NULL,
        `LastUpdated` datetime(6) NOT NULL,
        `DisplayOrder` int NOT NULL,
        `IsVisible` tinyint(1) NOT NULL,
        CONSTRAINT `PK_DashboardMetrics` PRIMARY KEY (`MetricId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE TABLE `Teachers` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `TeacherCode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `FullName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(100) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `Department` varchar(100) CHARACTER SET utf8mb4 NULL,
        `Specialization` longtext CHARACTER SET utf8mb4 NULL,
        `Degree` longtext CHARACTER SET utf8mb4 NULL,
        `DateOfBirth` datetime(6) NULL,
        `Gender` longtext CHARACTER SET utf8mb4 NULL,
        `Address` longtext CHARACTER SET utf8mb4 NULL,
        `AvatarUrl` longtext CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `UserId` int NULL,
        CONSTRAINT `PK_Teachers` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Teachers_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_Scores_ClassId` ON `Scores` (`ClassId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_Scores_CourseId` ON `Scores` (`CourseId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_Scores_EnrollmentId` ON `Scores` (`EnrollmentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_Classes_TeacherId` ON `Classes` (`TeacherId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIConversationLogs_CreatedAt` ON `AIConversationLogs` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIConversationLogs_RequestType` ON `AIConversationLogs` (`RequestType`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIConversationLogs_StudentId` ON `AIConversationLogs` (`StudentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIConversationLogs_UserId` ON `AIConversationLogs` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIKnowledgeBases_Category` ON `AIKnowledgeBases` (`Category`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIKnowledgeBases_IsActive` ON `AIKnowledgeBases` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_AIKnowledgeBases_Priority` ON `AIKnowledgeBases` (`Priority`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE INDEX `IX_DashboardMetrics_Category` ON `DashboardMetrics` (`Category`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE UNIQUE INDEX `IX_DashboardMetrics_MetricName` ON `DashboardMetrics` (`MetricName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE UNIQUE INDEX `IX_Teachers_TeacherCode` ON `Teachers` (`TeacherCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    CREATE UNIQUE INDEX `IX_Teachers_UserId` ON `Teachers` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Classes` ADD CONSTRAINT `FK_Classes_Teachers_TeacherId` FOREIGN KEY (`TeacherId`) REFERENCES `Teachers` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD CONSTRAINT `FK_Scores_Classes_ClassId` FOREIGN KEY (`ClassId`) REFERENCES `Classes` (`ClassId`) ON DELETE RESTRICT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD CONSTRAINT `FK_Scores_Courses_CourseId` FOREIGN KEY (`CourseId`) REFERENCES `Courses` (`CourseId`) ON DELETE RESTRICT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    ALTER TABLE `Scores` ADD CONSTRAINT `FK_Scores_Enrollments_EnrollmentId` FOREIGN KEY (`EnrollmentId`) REFERENCES `Enrollments` (`EnrollmentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260128095941_TailAdminMigration') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260128095941_TailAdminMigration', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

