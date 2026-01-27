-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: studentmanagementdb
-- ------------------------------------------------------
-- Server version	9.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260109110104_InitialCreate','8.0.2'),('20260109112614_AddSemesterEntity','8.0.2'),('20260109113856_AddSemesterTable','8.0.2'),('20260109115106_AddMajorToStudent','8.0.2'),('20260109115635_AddMajorToStudentTable','8.0.2'),('20260109123943_UpdateClassEntityForCreditSystem','8.0.2'),('20260109133146_Ten_Hanh_Dong','8.0.2'),('20260109135353_AddEnrollmentDetailsAndStudentAvatar','8.0.2'),('20260109140805_AddAvatarUrlToStudent','8.0.2'),('20260109150728_MakeAvatarUrlNullable','8.0.2'),('20260110054814_EditGrade','8.0.2'),('20260110060243_AddFlow2_GradingAndAI','8.0.2'),('20260110070501_AddClassCodeColumn','8.0.2'),('20260110085634_Edit','8.0.2'),('20260111140209_eee','8.0.2'),('20260113155358_AddAuthenticationFeatures','8.0.2'),('20260114075316_AddAvatarUrlToUser1','8.0.2'),('20260115062552_AddMajorToCourse','8.0.2'),('20260116142045_AddLearningPathRecommendation','8.0.2');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `academicanalyses`
--

DROP TABLE IF EXISTS `academicanalyses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `academicanalyses` (
  `AnalysisId` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `AnalysisDate` datetime(6) NOT NULL,
  `OverallGPA` double NOT NULL,
  `StrongSubjectsJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WeakSubjectsJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Recommendations` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AiModelUsed` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`AnalysisId`),
  KEY `IX_AcademicAnalyses_StudentId` (`StudentId`),
  CONSTRAINT `FK_AcademicAnalyses_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `academicanalyses`
--

LOCK TABLES `academicanalyses` WRITE;
/*!40000 ALTER TABLE `academicanalyses` DISABLE KEYS */;
INSERT INTO `academicanalyses` VALUES (11,11,'2026-01-21 10:20:53.000000',9,'[\"Database\", \"Network\"]','[]','Xuất sắc','Gemini-Pro'),(12,12,'2026-01-21 10:20:53.000000',6.5,'[]','[\"Math\"]','Học thêm Math','Gemini-Pro'),(13,13,'2026-01-21 10:20:53.000000',7.5,'[\"Math\"]','[]','Tốt','Gemini-Pro'),(14,14,'2026-01-21 10:20:53.000000',8,'[\"Statistics\"]','[]','Rất tốt','Gemini-Pro'),(15,15,'2026-01-21 10:20:53.000000',6,'[]','[\"Physics\"]','Ôn Physics','Gemini-Pro'),(16,16,'2026-01-21 10:20:53.000000',7.5,'[\"English\"]','[]','Tiếp tục','Gemini-Pro'),(17,17,'2026-01-21 10:20:53.000000',8.5,'[\"AI\", \"ML\"]','[]','Xuất sắc về AI','Gemini-Pro'),(18,18,'2026-01-21 10:20:53.000000',5.5,'[]','[\"AI\"]','Học thêm AI','Gemini-Pro'),(19,19,'2026-01-21 10:20:53.000000',9,'[\"English\", \"PE\"]','[]','Tốt lắm','Gemini-Pro'),(20,20,'2026-01-21 10:20:53.000000',8,'[\"English\"]','[]','Duy trì','Gemini-Pro'),(21,21,'2026-01-21 10:20:53.000000',7.5,'[\"ML\"]','[]','Khá tốt','Gemini-Pro'),(22,22,'2026-01-21 10:20:53.000000',6.5,'[]','[\"Statistics\"]','Cải thiện Stats','Gemini-Pro'),(23,23,'2026-01-21 10:20:53.000000',9.5,'[\"PE\", \"English\"]','[]','Hoàn hảo','Gemini-Pro'),(24,24,'2026-01-21 10:20:53.000000',8.5,'[\"AI\"]','[]','Rất tốt','Gemini-Pro'),(25,25,'2026-01-21 10:20:53.000000',7,'[]','[\"Database\"]','Ôn Database','Gemini-Pro'),(26,26,'2026-01-21 10:20:53.000000',8.5,'[\"ML\", \"AI\"]','[]','Xuất sắc AI/ML','Gemini-Pro'),(27,27,'2026-01-21 10:20:53.000000',9,'[\"DL\", \"CV\"]','[]','Rất giỏi DL','Gemini-Pro'),(28,28,'2026-01-21 10:20:53.000000',6,'[]','[\"CV\"]','Học thêm CV','Gemini-Pro'),(29,29,'2026-01-21 10:20:53.000000',8,'[\"NLP\"]','[]','Tốt về NLP','Gemini-Pro'),(30,30,'2026-01-21 10:20:53.000000',7.5,'[\"OS\"]','[]','Khá','Gemini-Pro'),(31,16,'2026-01-27 22:02:15.236745',5.6,'[]','[]','Bạn đang học tốt! Hãy duy trì và phát huy thêm.','Gemini-AI'),(32,31,'2026-01-27 22:11:18.497748',0.4,'[]','[\"Lập trình C\"]','Tập trung ôn tập các môn yếu. Tham gia học bổ trợ nếu cần. Sắp xếp thời gian học tập hợp lý hơn.','Gemini-AI'),(33,32,'2026-01-27 22:19:50.991602',4.2,'[]','[\"Lập trình C\"]','Tập trung ôn tập các môn yếu. Tham gia học bổ trợ nếu cần. Sắp xếp thời gian học tập hợp lý hơn.','Gemini-AI'),(34,32,'2026-01-27 22:32:20.087530',4.6,'[]','[\"Lập trình C\",\"Lập trình C\"]','Tập trung ôn tập các môn yếu. Tham gia học bổ trợ nếu cần. Sắp xếp thời gian học tập hợp lý hơn.','Gemini-AI');
/*!40000 ALTER TABLE `academicanalyses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `classes`
--

DROP TABLE IF EXISTS `classes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `classes` (
  `ClassId` int NOT NULL AUTO_INCREMENT,
  `ClassName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CourseId` int NOT NULL DEFAULT '0',
  `CurrentEnrollment` int NOT NULL DEFAULT '0',
  `MaxCapacity` int NOT NULL DEFAULT '0',
  `MaxStudents` int NOT NULL DEFAULT '30',
  `Room` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Schedule` varchar(255) DEFAULT NULL,
  `SemesterId` int NOT NULL DEFAULT '0',
  `TeacherId` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ClassCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DayOfWeekPair` int NOT NULL DEFAULT '0',
  `TimeSlot` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ClassId`),
  KEY `IX_Classes_CourseId` (`CourseId`),
  KEY `IX_Classes_SemesterId` (`SemesterId`),
  CONSTRAINT `FK_Classes_Courses_CourseId` FOREIGN KEY (`CourseId`) REFERENCES `courses` (`CourseId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Classes_Semesters_SemesterId` FOREIGN KEY (`SemesterId`) REFERENCES `semesters` (`SemesterId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `classes`
--

LOCK TABLES `classes` WRITE;
/*!40000 ALTER TABLE `classes` DISABLE KEYS */;
INSERT INTO `classes` VALUES (1,'Lập trình C - Lớp 1',1,8,40,40,'P.304',NULL,32,NULL,'2026-01-27 21:52:42','PRN101-01',1,1),(2,'Lập trình C - Lớp 2',1,1,40,40,'P.305',NULL,32,NULL,'2026-01-27 21:52:50','PRN101-02',2,2),(3,'Cấu trúc dữ liệu - Lớp 1',2,0,35,35,'P.306','Wed-Sat (Slot 3)',32,NULL,'2026-01-27 22:31:32','DSA101-01',3,2),(4,'Lập trình Java - Lớp 1',3,0,1,1,'P.307','Mon-Thu (Slot 1)',32,NULL,'2026-01-27 22:23:11','PRN211-01',1,4),(5,'Lập trình Java - Lớp 2',3,0,40,30,'P.308','Tue-Fri (Slot 2)',16,2,'2026-01-27 21:40:40','PRN211-02',2,2),(6,'Lập trình C# - Lớp 1',4,0,35,30,'P.309','Wed-Sat (Slot 4)',16,3,'2026-01-27 21:40:40','PRN212-01',3,4),(7,'Cơ sở dữ liệu - Lớp 1',5,0,40,30,'P.310','Mon-Thu (Slot 2)',16,4,'2026-01-27 21:40:40','DBI202-01',1,2),(8,'Cơ sở dữ liệu - Lớp 2',5,0,40,30,'P.311','Tue-Fri (Slot 3)',16,5,'2026-01-27 21:40:40','DBI202-02',2,3),(9,'Đồ án 1 - Lớp 1',6,0,30,30,'P.312','Wed-Sat (Slot 1)',16,6,'2026-01-27 21:40:40','PRJ301-01',3,1),(10,'Toán rời rạc - Lớp 1',7,0,40,30,'P.313','Mon-Thu (Slot 3)',16,7,'2026-01-27 21:40:40','MAD101-01',1,3),(11,'Xác suất thống kê - Lớp 1',8,0,40,30,'P.314','Tue-Fri (Slot 4)',16,8,'2026-01-27 21:40:40','STA101-01',2,4),(12,'Vật lý đại cương - Lớp 1',9,1,45,30,'P.315','Wed-Sat (Slot 2)',16,9,'2026-01-27 21:40:40','PHY101-01',3,2),(13,'Anh văn 1 - Lớp 1',10,0,30,30,'P.316','Mon-Thu (Slot 4)',16,10,'2026-01-27 21:40:40','ENG101-01',1,4),(14,'Anh văn 1 - Lớp 2',10,0,30,30,'P.317','Tue-Fri (Slot 1)',16,2,'2026-01-27 21:40:40','ENG101-02',2,1),(15,'Anh văn 2 - Lớp 1',11,0,30,30,'P.318','Wed-Sat (Slot 3)',16,3,'2026-01-27 21:40:40','ENG102-01',3,3),(16,'Giáo dục thể chất - Lớp 1',13,0,50,30,'Sân','Mon-Thu (Slot 2)',16,4,'2026-01-27 21:40:40','PED101-01',1,2),(17,'AI cơ bản - Lớp 1',14,0,35,30,'P.401','Tue-Fri (Slot 2)',16,5,'2026-01-27 21:40:40','AIF101-01',2,2),(18,'Machine Learning - Lớp 1',15,0,30,30,'P.402','Wed-Sat (Slot 4)',16,6,'2026-01-27 21:40:40','AIL302-01',3,4),(19,'Deep Learning - Lớp 1',16,0,25,30,'P.403','Mon-Thu (Slot 1)',16,7,'2026-01-27 21:40:40','AIL303-01',1,1),(20,'Computer Vision - Lớp 1',17,0,25,30,'P.404','Tue-Fri (Slot 3)',16,8,'2026-01-27 21:40:40','AIC304-01',2,3),(21,'NLP - Lớp 1',18,0,25,30,'P.405','Wed-Sat (Slot 1)',16,9,'2026-01-27 21:40:40','AIN305-01',3,1),(22,'Hệ điều hành - Lớp 1',19,0,40,30,'P.406','Mon-Thu (Slot 3)',16,10,'2026-01-27 21:40:40','OSG202-01',1,3),(23,'Mạng máy tính - Lớp 1',20,0,40,30,'P.407','Tue-Fri (Slot 4)',16,2,'2026-01-27 21:40:40','NWC203-01',2,4),(24,'An toàn thông tin - Lớp 1',21,0,35,30,'P.408','Wed-Sat (Slot 2)',16,3,'2026-01-27 21:40:40','SEC301-01',3,2),(25,'Quản trị hệ thống - Lớp 1',22,0,30,30,'P.409','Mon-Thu (Slot 4)',16,4,'2026-01-27 21:40:40','SYS302-01',1,4),(26,'Phân tích thiết kế - Lớp 1',23,0,35,30,'P.410','Tue-Fri (Slot 1)',16,5,'2026-01-27 21:40:40','SAD201-01',2,1),(27,'Quản lý dự án - Lớp 1',24,0,30,30,'P.411','Wed-Sat (Slot 3)',16,6,'2026-01-27 21:40:40','SWP391-01',3,3),(28,'Kiểm thử phần mềm - Lớp 1',25,0,35,30,'P.412','Mon-Thu (Slot 2)',16,7,'2026-01-27 21:40:40','SWT301-01',1,2),(29,'DevOps - Lớp 1',27,0,30,30,'P.413','Tue-Fri (Slot 3)',16,8,'2026-01-27 21:40:40','DOP401-01',2,3),(30,'Cloud Computing - Lớp 1',28,0,30,30,'P.414','Wed-Sat (Slot 4)',16,9,'2026-01-27 21:40:40','CLO402-01',3,4);
/*!40000 ALTER TABLE `classes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `courses`
--

DROP TABLE IF EXISTS `courses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `courses` (
  `CourseId` int NOT NULL AUTO_INCREMENT,
  `CourseName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CourseCode` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Credits` int NOT NULL,
  `PrerequisiteCourseId` int DEFAULT NULL,
  `Major` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`CourseId`),
  UNIQUE KEY `IX_Courses_CourseCode` (`CourseCode`),
  KEY `IX_Courses_PrerequisiteCourseId` (`PrerequisiteCourseId`),
  CONSTRAINT `FK_Courses_Courses_PrerequisiteCourseId` FOREIGN KEY (`PrerequisiteCourseId`) REFERENCES `courses` (`CourseId`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `courses`
--

LOCK TABLES `courses` WRITE;
/*!40000 ALTER TABLE `courses` DISABLE KEYS */;
INSERT INTO `courses` VALUES (1,'Lập trình C','PRN101',3,NULL,'SE'),(2,'Cấu trúc dữ liệu','DSA101',3,1,'SE'),(3,'Lập trình Java','PRN211',3,1,'SE'),(4,'Lập trình C#','PRN212',3,1,'Công nghệ thông tin'),(5,'Cơ sở dữ liệu','DBI202',3,NULL,'SE'),(6,'Đồ án 1','PRJ301',3,3,'SE'),(7,'Toán rời rạc','MAD101',3,NULL,'SE'),(8,'Xác suất thống kê','STA101',3,NULL,'SE'),(9,'Vật lý đại cương','PHY101',2,NULL,'SE'),(10,'Anh văn 1','ENG101',2,NULL,NULL),(11,'Anh văn 2','ENG102',2,10,NULL),(12,'Anh văn 3','ENG103',2,11,NULL),(13,'Giáo dục thể chất','PED101',1,NULL,NULL),(14,'Trí tuệ nhân tạo cơ bản','AIF101',3,NULL,'IA'),(15,'Machine Learning','AIL302',3,14,'IA'),(16,'Deep Learning','AIL303',3,15,'IA'),(17,'Computer Vision','AIC304',3,16,'IA'),(18,'Natural Language Processing','AIN305',3,16,'IA'),(19,'Hệ điều hành','OSG202',3,1,'SE'),(20,'Mạng máy tính','NWC203',3,NULL,'SE'),(21,'An toàn thông tin','SEC301',3,20,'IS'),(22,'Quản trị hệ thống','SYS302',3,19,'IS'),(23,'Phân tích thiết kế hệ thống','SAD201',3,5,'IS'),(24,'Quản lý dự án phần mềm','SWP391',3,NULL,'SE'),(25,'Kiểm thử phần mềm','SWT301',3,6,'SE'),(26,'Kiến trúc phần mềm','SAR302',3,6,'SE'),(27,'DevOps','DOP401',3,19,'SE'),(28,'Cloud Computing','CLO402',3,20,'SE'),(29,'Blockchain','BLC403',3,5,'SE'),(30,'IoT & Embedded Systems','IOT404',3,19,'SE'),(32,'pê rờ nờ','PRN363',3,6,NULL);
/*!40000 ALTER TABLE `courses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enrollments`
--

DROP TABLE IF EXISTS `enrollments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enrollments` (
  `EnrollmentId` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `ClassId` int NOT NULL,
  `EnrollmentDate` datetime(6) NOT NULL,
  `AttemptNumber` int NOT NULL DEFAULT '0',
  `FinalScore` double DEFAULT NULL,
  `Grade` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsPassed` tinyint(1) NOT NULL DEFAULT '0',
  `Comment` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `MidtermScore` double DEFAULT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalScore` double DEFAULT NULL,
  PRIMARY KEY (`EnrollmentId`),
  KEY `IX_Enrollments_StudentId` (`StudentId`),
  KEY `IX_Enrollments_ClassId` (`ClassId`),
  CONSTRAINT `FK_Enrollments_Classes_ClassId` FOREIGN KEY (`ClassId`) REFERENCES `classes` (`ClassId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Enrollments_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=52 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enrollments`
--

LOCK TABLES `enrollments` WRITE;
/*!40000 ALTER TABLE `enrollments` DISABLE KEYS */;
INSERT INTO `enrollments` VALUES (11,11,6,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(12,12,6,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(13,13,7,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(14,14,7,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(15,15,8,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(16,16,8,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(17,17,9,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(18,18,9,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(19,19,10,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(20,20,10,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(21,21,11,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(22,22,11,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(23,23,12,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(24,24,12,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(25,25,13,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(26,26,13,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(27,27,14,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(28,28,14,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(29,29,15,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(30,30,15,'2025-09-05 00:00:00.000000',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(31,31,12,'2026-01-24 15:32:15.607668',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(32,31,11,'2026-01-24 15:32:57.649457',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(33,31,2,'2026-01-24 15:33:48.106863',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(34,31,10,'2026-01-24 15:33:55.836792',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(35,16,1,'2026-01-27 22:00:49.854820',1,6,'C',1,NULL,5,'Active',5.6),(36,18,1,'2026-01-27 22:00:50.081798',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(37,27,1,'2026-01-27 22:00:50.505155',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(38,23,1,'2026-01-27 22:00:50.824149',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(39,30,1,'2026-01-27 22:00:51.148331',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(40,11,1,'2026-01-27 22:00:51.409708',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(41,31,1,'2026-01-27 22:09:27.911506',1,0,'F',0,NULL,1,'Active',0.4),(42,24,1,'2026-01-27 22:12:31.933688',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(43,29,1,'2026-01-27 22:12:32.530147',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(44,26,1,'2026-01-27 22:12:32.820722',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(45,25,1,'2026-01-27 22:12:33.129143',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(46,20,1,'2026-01-27 22:12:33.385687',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(47,28,1,'2026-01-27 22:12:43.571403',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(48,22,1,'2026-01-27 22:16:30.074715',1,NULL,NULL,0,NULL,NULL,'Active',NULL),(49,32,1,'2026-01-27 22:19:22.336187',1,1,'D',0,NULL,9,'Active',4.2),(50,32,2,'2026-01-27 22:31:56.679005',1,9,'C+',1,NULL,3,'Active',6.6),(51,33,1,'2026-01-27 23:38:38.106889',1,1,'D',0,NULL,10,'Active',4.6);
/*!40000 ALTER TABLE `enrollments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `learningpathrecommendations`
--

DROP TABLE IF EXISTS `learningpathrecommendations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `learningpathrecommendations` (
  `RecommendationId` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `SemesterId` int NOT NULL,
  `RecommendationDate` datetime(6) NOT NULL,
  `RecommendedCoursesJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OverallStrategy` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WarningsJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AiModelUsed` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsViewed` tinyint(1) NOT NULL,
  PRIMARY KEY (`RecommendationId`),
  KEY `IX_LearningPathRecommendations_SemesterId` (`SemesterId`),
  KEY `IX_LearningPathRecommendations_StudentId` (`StudentId`),
  CONSTRAINT `FK_LearningPathRecommendations_Semesters_SemesterId` FOREIGN KEY (`SemesterId`) REFERENCES `semesters` (`SemesterId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LearningPathRecommendations_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `learningpathrecommendations`
--

LOCK TABLES `learningpathrecommendations` WRITE;
/*!40000 ALTER TABLE `learningpathrecommendations` DISABLE KEYS */;
INSERT INTO `learningpathrecommendations` VALUES (11,11,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Security\",\"Priority\":1,\"Reason\":\"An ninh\"}]','Học Security','[]','Gemini',0),(12,12,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Network Admin\",\"Priority\":1,\"Reason\":\"Quản trị\"}]','Học Admin','[]','Gemini',0),(13,13,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"System Design\",\"Priority\":1,\"Reason\":\"Thiết kế\"}]','Học Design','[]','Gemini',0),(14,14,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Testing\",\"Priority\":1,\"Reason\":\"QA\"}]','Học Testing','[]','Gemini',0),(15,15,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Physics 2\",\"Priority\":1,\"Reason\":\"Tiếp tục\"}]','Học Physics','[]','Gemini',0),(16,16,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Advanced English\",\"Priority\":1,\"Reason\":\"Nâng cao\"}]','Cải thiện English','[]','Gemini',0),(17,17,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Computer Vision\",\"Priority\":1,\"Reason\":\"AI Vision\"}]','Học CV','[]','Gemini',0),(18,18,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"AI Basics\",\"Priority\":1,\"Reason\":\"Nền tảng AI\"}]','Học AI cơ bản','[]','Gemini',0),(19,19,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Business English\",\"Priority\":1,\"Reason\":\"Thương mại\"}]','Học Business','[]','Gemini',0),(20,20,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"TOEIC\",\"Priority\":1,\"Reason\":\"Chứng chỉ\"}]','Lấy TOEIC','[]','Gemini',0),(21,21,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"NLP\",\"Priority\":1,\"Reason\":\"AI Language\"}]','Học NLP','[]','Gemini',0),(22,22,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Data Science\",\"Priority\":1,\"Reason\":\"Phân tích\"}]','Học Data','[]','Gemini',0),(23,23,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Sport Management\",\"Priority\":1,\"Reason\":\"Quản lý\"}]','Học Quản lý','[]','Gemini',0),(24,24,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Robotics\",\"Priority\":1,\"Reason\":\"Robot\"}]','Học Robot','[]','Gemini',0),(25,25,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"IoT\",\"Priority\":1,\"Reason\":\"Vạn vật\"}]','Học IoT','[]','Gemini',0),(26,26,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Blockchain\",\"Priority\":1,\"Reason\":\"Công nghệ mới\"}]','Học Blockchain','[]','Gemini',0),(27,27,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"AR/VR\",\"Priority\":1,\"Reason\":\"Thực tế ảo\"}]','Học AR/VR','[]','Gemini',0),(28,28,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Game Dev\",\"Priority\":1,\"Reason\":\"Game\"}]','Học Game','[]','Gemini',0),(29,29,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Embedded\",\"Priority\":1,\"Reason\":\"Nhúng\"}]','Học Nhúng','[]','Gemini',0),(30,30,16,'2026-01-21 10:20:53.000000','[{\"CourseName\":\"Quantum\",\"Priority\":1,\"Reason\":\"Lượng tử\"}]','Học Quantum','[]','Gemini',0);
/*!40000 ALTER TABLE `learningpathrecommendations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `NotificationId` int NOT NULL AUTO_INCREMENT,
  `Title` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsRead` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StudentId` int DEFAULT NULL,
  PRIMARY KEY (`NotificationId`),
  KEY `IX_Notifications_StudentId` (`StudentId`),
  CONSTRAINT `FK_Notifications_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
INSERT INTO `notifications` VALUES (11,'Chào mừng!','Chào mừng sinh viên mới',1,'2026-01-21 10:20:53.000000','Welcome',11),(12,'Đăng ký thành công','Đã đăng ký Database',1,'2026-01-21 10:20:53.000000','EnrollmentSuccess',12),(13,'Điểm mới','Điểm Toán đã cập nhật',0,'2026-01-21 10:20:53.000000','ScoreUpdate',13),(14,'AI Phân tích','Phân tích AI sẵn sàng',0,'2026-01-21 10:20:53.000000','AIAnalysis',14),(15,'Thông báo','Lịch thi đã ra',0,'2026-01-21 10:20:53.000000','General',15),(16,'Đăng ký thành công','Đã đăng ký Vật lý',1,'2026-01-21 10:20:53.000000','EnrollmentSuccess',16),(17,'Điểm mới','Điểm English đã có',0,'2026-01-21 10:20:53.000000','ScoreUpdate',17),(18,'Lộ trình học tập','Gợi ý môn học kỳ tới',0,'2026-01-21 10:20:53.000000','LearningPath',18),(19,'Chào mừng!','Welcome to system',1,'2026-01-21 10:20:53.000000','Welcome',19),(20,'Nhắc nhở','Đăng ký môn học',0,'2026-01-21 10:20:53.000000','Reminder',20),(21,'Điểm mới','Điểm AI đã cập nhật',0,'2026-01-21 10:20:53.000000','ScoreUpdate',21),(22,'Cảnh báo','Cần cải thiện môn yếu',0,'2026-01-21 10:20:53.000000','Warning',22),(23,'Đăng ký thành công','Đã đăng ký ML',1,'2026-01-21 10:20:53.000000','EnrollmentSuccess',23),(24,'AI Phân tích','Kết quả phân tích mới',0,'2026-01-21 10:20:53.000000','AIAnalysis',24),(25,'Thông báo','Thông báo chung',0,'2026-01-21 10:20:53.000000','General',25),(26,'Lộ trình học tập','Đề xuất môn học',0,'2026-01-21 10:20:53.000000','LearningPath',26),(27,'Điểm mới','Điểm DL đã ra',0,'2026-01-21 10:20:53.000000','ScoreUpdate',27),(28,'Nhắc nhở','Hạn nộp project',0,'2026-01-21 10:20:53.000000','Reminder',28),(29,'Đăng ký thành công','Đã đăng ký CV',1,'2026-01-21 10:20:53.000000','EnrollmentSuccess',29),(30,'Chào mừng!','Chúc học tập tốt',1,'2026-01-21 10:20:53.000000','Welcome',30);
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `RoleId` int NOT NULL AUTO_INCREMENT,
  `RoleName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Admin','Quản trị viên hệ thống'),(2,'Teacher','Giảng viên'),(3,'Student','Sinh viên'),(4,'Manager','Staff manager');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scores`
--

DROP TABLE IF EXISTS `scores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scores` (
  `ScoreId` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `CourseId` int NOT NULL,
  `ScoreValue` double NOT NULL,
  PRIMARY KEY (`ScoreId`),
  KEY `IX_Scores_StudentId` (`StudentId`),
  CONSTRAINT `FK_Scores_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scores`
--

LOCK TABLES `scores` WRITE;
/*!40000 ALTER TABLE `scores` DISABLE KEYS */;
INSERT INTO `scores` VALUES (11,11,5,9),(12,12,5,6.5),(13,13,7,7.5),(14,14,7,8),(15,15,8,6),(16,16,8,7.5),(17,17,9,8.5),(18,18,9,5.5),(19,19,10,9),(20,20,10,8),(21,21,11,7.5),(22,22,11,6.5),(23,23,13,9.5),(24,24,13,8.5),(25,25,14,7),(26,26,14,8.5),(27,27,15,9),(28,28,15,6),(29,29,16,8),(30,30,16,7.5);
/*!40000 ALTER TABLE `scores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `semesters`
--

DROP TABLE IF EXISTS `semesters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `semesters` (
  `SemesterId` int NOT NULL AUTO_INCREMENT,
  `SemesterName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SemesterCode` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`SemesterId`),
  UNIQUE KEY `IX_Semesters_SemesterCode` (`SemesterCode`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `semesters`
--

LOCK TABLES `semesters` WRITE;
/*!40000 ALTER TABLE `semesters` DISABLE KEYS */;
INSERT INTO `semesters` VALUES (1,'Fall 2020','FA2020','2020-09-01 00:00:00.000000','2021-01-15 00:00:00.000000',0),(2,'Spring 2021','SP2021','2021-02-01 00:00:00.000000','2021-06-15 00:00:00.000000',0),(3,'Summer 2021','SU2021','2021-07-01 00:00:00.000000','2021-08-30 00:00:00.000000',0),(4,'Fall 2021','FA2021','2021-09-01 00:00:00.000000','2022-01-15 00:00:00.000000',0),(5,'Spring 2022','SP2022','2022-02-01 00:00:00.000000','2022-06-15 00:00:00.000000',0),(6,'Summer 2022','SU2022','2022-07-01 00:00:00.000000','2022-08-30 00:00:00.000000',0),(7,'Fall 2022','FA2022','2022-09-01 00:00:00.000000','2023-01-15 00:00:00.000000',0),(8,'Spring 2023','SP2023','2023-02-01 00:00:00.000000','2023-06-15 00:00:00.000000',0),(9,'Summer 2023','SU2023','2023-07-01 00:00:00.000000','2023-08-30 00:00:00.000000',0),(10,'Fall 2023','FA2023','2023-09-01 00:00:00.000000','2024-01-15 00:00:00.000000',0),(11,'Spring 2024','SP2024','2024-02-01 00:00:00.000000','2024-06-15 00:00:00.000000',0),(12,'Summer 2024','SU2024','2024-07-01 00:00:00.000000','2024-08-30 00:00:00.000000',0),(13,'Fall 2024','FA2024','2024-09-01 00:00:00.000000','2025-01-15 00:00:00.000000',0),(14,'Spring 2025','SP2025','2025-02-01 00:00:00.000000','2025-06-15 00:00:00.000000',0),(15,'Summer 2025','SU2025','2025-07-01 00:00:00.000000','2025-08-30 00:00:00.000000',0),(16,'Fall 2025','FA2025','2025-09-01 00:00:00.000000','2026-01-15 00:00:00.000000',1),(17,'Spring 2026','SP2026','2026-02-01 00:00:00.000000','2026-06-15 00:00:00.000000',0),(18,'Summer 2026','SU2026','2026-07-01 00:00:00.000000','2026-08-30 00:00:00.000000',0),(19,'Fall 2026','FA2026','2026-09-01 00:00:00.000000','2027-01-15 00:00:00.000000',0),(20,'Spring 2027','SP2027','2027-02-01 00:00:00.000000','2027-06-15 00:00:00.000000',0),(21,'Summer 2027','SU2027','2027-07-01 00:00:00.000000','2027-08-30 00:00:00.000000',0),(22,'Fall 2027','FA2027','2027-09-01 00:00:00.000000','2028-01-15 00:00:00.000000',0),(23,'Spring 2028','SP2028','2028-02-01 00:00:00.000000','2028-06-15 00:00:00.000000',0),(24,'Summer 2028','SU2028','2028-07-01 00:00:00.000000','2028-08-30 00:00:00.000000',0),(25,'Fall 2028','FA2028','2028-09-01 00:00:00.000000','2029-01-15 00:00:00.000000',0),(26,'Spring 2029','SP2029','2029-02-01 00:00:00.000000','2029-06-15 00:00:00.000000',0),(27,'Summer 2029','SU2029','2029-07-01 00:00:00.000000','2029-08-30 00:00:00.000000',0),(28,'Fall 2029','FA2029','2029-09-01 00:00:00.000000','2030-01-15 00:00:00.000000',0),(29,'Spring 2030','SP2030','2030-02-01 00:00:00.000000','2030-06-15 00:00:00.000000',0),(30,'Summer 2030','SU2030','2030-07-01 00:00:00.000000','2030-08-30 00:00:00.000000',1),(31,'Học kỳ 1 - 2024','FA24','2026-01-21 11:06:57.136144','2026-05-21 11:06:57.136150',1),(32,'Học kỳ 2 - 2024','SP25','2026-01-21 11:06:57.144555','2026-05-21 11:06:57.144557',1);
/*!40000 ALTER TABLE `semesters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `StudentId` int NOT NULL AUTO_INCREMENT,
  `StudentCode` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FullName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DateOfBirth` datetime(6) NOT NULL,
  `ClassCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OverallGPA` decimal(3,2) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UserId` int NOT NULL,
  `Major` int NOT NULL,
  `AvatarUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CurrentTermNo` int DEFAULT NULL,
  `IsFirstLogin` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`StudentId`),
  UNIQUE KEY `IX_Students_StudentCode` (`StudentCode`),
  KEY `IX_Students_UserId` (`UserId`),
  CONSTRAINT `FK_Students_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES (11,'STU202600011','Cao Văn T','student011@fpt.edu.vn','0901234577','2005-11-23 00:00:00.000000','IS1802',0.00,'2026-01-21 10:20:53.000000',21,0,NULL,1,1),(12,'STU202600012','Đỗ Thị U','student012@fpt.edu.vn','0901234578','2005-12-28 00:00:00.000000','IS1802',0.00,'2026-01-21 10:20:53.000000',22,0,NULL,1,1),(13,'STU202600013','Từ Văn V','student013@fpt.edu.vn','0901234579','2005-01-30 00:00:00.000000','SE1803',0.00,'2026-01-21 10:20:53.000000',23,0,NULL,1,1),(14,'STU202600014','Lưu Thị W','student014@fpt.edu.vn','0901234580','2005-02-14 00:00:00.000000','SE1803',0.00,'2026-01-21 10:20:53.000000',24,0,NULL,1,1),(15,'STU202600015','Nguyễn Văn X','student015@fpt.edu.vn','0901234581','2005-03-17 00:00:00.000000','SE1804',0.00,'2026-01-21 10:20:53.000000',25,0,NULL,1,1),(16,'STU202600016','Trần Thị Y','student016@fpt.edu.vn','0901234582','2005-04-21 00:00:00.000000','SE1804',0.00,'2026-01-21 10:20:53.000000',26,0,NULL,1,1),(17,'STU202600017','Lê Văn Z','student017@fpt.edu.vn','0901234583','2005-05-26 00:00:00.000000','IA1803',0.00,'2026-01-21 10:20:53.000000',27,0,NULL,1,1),(18,'STU202600018','Phạm Thị AA','student018@fpt.edu.vn','0901234584','2005-06-30 00:00:00.000000','IA1803',0.00,'2026-01-21 10:20:53.000000',28,0,NULL,1,1),(19,'STU202600019','Hoàng Văn BB','student019@fpt.edu.vn','0901234585','2005-07-04 00:00:00.000000','IA1804',0.00,'2026-01-21 10:20:53.000000',29,0,NULL,1,1),(20,'STU202600020','Vũ Thị CC','student020@fpt.edu.vn','0901234586','2005-08-08 00:00:00.000000','IA1804',0.00,'2026-01-21 10:20:53.000000',30,0,NULL,1,1),(21,'STU202600021','Nguyễn Văn DD','student021@fpt.edu.vn','0901234587','2005-09-11 00:00:00.000000','IS1803',0.00,'2026-01-21 10:20:53.000000',11,0,NULL,1,1),(22,'STU202600022','Trần Thị EE','student022@fpt.edu.vn','0901234588','2005-10-15 00:00:00.000000','IS1803',0.00,'2026-01-21 10:20:53.000000',12,0,NULL,1,1),(23,'STU202600023','Lê Văn FF','student023@fpt.edu.vn','0901234589','2005-11-19 00:00:00.000000','IS1804',0.00,'2026-01-21 10:20:53.000000',13,0,NULL,1,1),(24,'STU202600024','Phạm Thị GG','student024@fpt.edu.vn','0901234590','2005-12-23 00:00:00.000000','IS1804',0.00,'2026-01-21 10:20:53.000000',14,0,NULL,1,1),(25,'STU202600025','Hoàng Văn HH','student025@fpt.edu.vn','0901234591','2005-01-27 00:00:00.000000','SE1805',0.00,'2026-01-21 10:20:53.000000',15,0,NULL,1,1),(26,'STU202600026','Vũ Thị II','student026@fpt.edu.vn','0901234592','2005-02-01 00:00:00.000000','SE1805',0.00,'2026-01-21 10:20:53.000000',16,0,NULL,1,1),(27,'STU202600027','Đặng Văn JJ','student027@fpt.edu.vn','0901234593','2005-03-06 00:00:00.000000','SE1806',0.00,'2026-01-21 10:20:53.000000',17,0,NULL,1,1),(28,'STU202600028','Bùi Thị KK','student028@fpt.edu.vn','0901234594','2005-04-10 00:00:00.000000','SE1806',0.00,'2026-01-21 10:20:53.000000',18,0,NULL,1,1),(29,'STU202600029','Dương Văn LL','student029@fpt.edu.vn','0901234595','2005-05-14 00:00:00.000000','IA1805',0.00,'2026-01-21 10:20:53.000000',19,0,NULL,1,1),(30,'STU202600030','Ngô Thị MM','student030@fpt.edu.vn','0901234596','2005-06-18 00:00:00.000000','IA1805',0.00,'2026-01-21 10:20:53.000000',20,0,NULL,1,1),(31,'STU202600031','tùng cube','nguyenvantung30052004@gmail.com','0936544411','2008-01-24 00:00:00.000000','Chưa phân lớp',0.00,'2026-01-24 14:10:58.893048',43,10,NULL,1,1),(32,'STU202600032','hoànggggggggggggggggggggggg','se180090doquochoang@gmail.com','0936544111','2008-01-27 00:00:00.000000','PRN101-01',0.00,'2026-01-27 22:18:49.675645',45,1,NULL,1,1),(33,'STU202600033','khoaaaaaaaaaaaaaaaaaaa','moyajom715@gxuzi.com','0111111111','2008-01-27 00:00:00.000000','Chưa phân lớp',0.00,'2026-01-27 23:38:04.195252',46,1,NULL,1,1);
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FullName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `LastLogin` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `RoleId` int NOT NULL,
  `GoogleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `MustChangePassword` tinyint(1) NOT NULL DEFAULT '0',
  `PasswordChangedAt` datetime(6) DEFAULT NULL,
  `RefreshToken` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RefreshTokenExpiryTime` datetime(6) DEFAULT NULL,
  `AvatarUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `IX_Users_Email` (`Email`),
  KEY `IX_Users_RoleId` (`RoleId`),
  KEY `IX_Users_GoogleId` (`GoogleId`),
  CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`RoleId`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Administrator','0999999999','2026-01-21 10:20:53.000000',NULL,1,1,NULL,0,NULL,NULL,NULL,NULL),(2,'teachera@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Nguyễn Văn A','0901111111','2026-01-21 10:20:53.000000',NULL,0,2,NULL,0,NULL,NULL,NULL,NULL),(3,'teacherb@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Trần Thị B','0902222222','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(4,'teacherc@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Lê Văn C','0903333333','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(5,'teacherd@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Phạm Thị D','0904444444','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(6,'teachere@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Hoàng Văn E','0905555555','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(7,'teacherf@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Vũ Thị F','0906666666','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(8,'teacherg@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Đặng Văn G','0907777777','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(9,'teacherh@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Bùi Thị H','0908888888','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(10,'teacheri@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Dương Văn I','0909999999','2026-01-21 10:20:53.000000',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(11,'student001@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Ngô Minh J','0901234567','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(12,'student002@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Lý Thị K','0901234568','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(13,'student003@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Đinh Văn L','0901234569','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(14,'student004@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Võ Thị M','0901234570','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(15,'student005@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Trương Văn N','0901234571','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(16,'student006@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Phan Thị O','0901234572','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(17,'student007@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Mai Văn P','0901234573','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(18,'student008@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Tô Thị Q','0901234574','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(19,'student009@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Hồ Văn R','0901234575','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(20,'student010@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Lâm Thị S','0901234576','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(21,'student011@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Cao Văn T','0901234577','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(22,'student012@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Đỗ Thị U','0901234578','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(23,'student013@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Từ Văn V','0901234579','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(24,'student014@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Lưu Thị W','0901234580','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(25,'student015@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Nguyễn Văn X','0901234581','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(26,'student016@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Trần Thị Y','0901234582','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(27,'student017@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Lê Văn Z','0901234583','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(28,'student018@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Phạm Thị AA','0901234584','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(29,'student019@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Hoàng Văn BB','0901234585','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(30,'student020@fpt.edu.vn','$2a$11$KZQJvGxVKx5Pu3JZtZ6ZKO5K8QKtQxZ5Z6Z7Z8Z9Z0ZaZbZcZdZeZ','Vũ Thị CC','0901234586','2026-01-21 10:20:53.000000',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(31,'admin@student.com','$2a$11$vJGt49J/8.KZi/eC44tC/OqXTEMJfcG0advghawgiYKvLHk3UrCAi','System Administrator','0999999999','2026-01-21 11:06:56.803196','2026-01-27 23:59:28.488804',1,1,NULL,0,NULL,NULL,NULL,NULL),(32,'manager@student.com','$2a$11$UyJjCKqiVab1JrUYtvW5neGOVGTXHWnK/wGzyV0b4Uo3MxXEpaF3W','Training Manager','0988888888','2026-01-21 11:06:56.960692','2026-01-27 22:22:28.431947',1,4,NULL,0,NULL,NULL,NULL,NULL),(33,'teacher@student.com','$2a$11$VfJYCpYc8gtSYizSv4gya.SKxi/CdWWsGAZf.HcjOQ4u66oMO.6vG','Nguyen Van Teacher','0977777777','2026-01-21 11:06:57.122678',NULL,1,2,NULL,0,NULL,NULL,NULL,NULL),(34,'khoa@student.com','$2a$11$jlQnGL.YioEr5ll4FZ9m/eIviafVTI7wqlvRF0beQuWGsM/klvGxG','KHOA','0936544222','2026-01-21 11:23:42.018296',NULL,1,2,NULL,1,NULL,NULL,NULL,NULL),(35,'khoanhdse183541@fpt.edu.vn','$2a$11$1kGED42q23yI5Z2wkgP2NevfLwNeSgwdjSpiRsObno91bGRYrEE1e','khoaaaaaaaaaaaaaaa','0936544429','2026-01-21 11:24:35.652383','2026-01-24 13:29:44.912901',0,3,NULL,1,NULL,NULL,NULL,NULL),(36,'1dqhoang@gmail.com','$2a$11$aYRvGfLgcojqlx5qeV42Vuj3yrtd4U/8tCs49vGhA9V2fNxPYXyW.','hoànggggggggggggggggggggg','0936544429','2026-01-21 11:34:00.535398','2026-01-24 13:29:49.163558',1,3,NULL,0,NULL,NULL,NULL,NULL),(43,'nguyenvantung30052004@gmail.com','$2a$11$53yL.6dD7Mxi0nblhR2RA.zAyKeQNEdh0gy0WPDLUVR5o/e63i.dO','tùng cube','0936544411','2026-01-24 14:10:58.753901','2026-01-24 15:31:38.365987',1,3,NULL,0,'2026-01-24 15:29:49.899535',NULL,NULL,NULL),(44,'KHOAMAP5544@GMAIL.COM','$2a$11$gBvOsH4eIs2tT8/Ww7jqYeEBfelLqKcpkMedXwCQCWQ8/Q7AoECT6','KHOAGIAOVIEN','0123123123','2026-01-26 18:00:16.780657','2026-01-28 00:15:56.581439',1,3,NULL,0,'2026-01-27 23:59:07.510732',NULL,NULL,NULL),(45,'se180090doquochoang@gmail.com','$2a$11$RU8XXH4/IS6sOwcUoqnPPusPkE.gftRv3zMDvifRDXcLGTTaA5dBC','hoànggggggggggggggggggggggg','0936544111','2026-01-27 22:18:49.662098',NULL,1,3,NULL,1,NULL,NULL,NULL,NULL),(46,'moyajom715@gxuzi.com','$2a$11$hCzQHHt7DB9V4VyNhKMS7u6Cocrvp/Hn3ua0wZoqWQy0LQ23GkrSO','khoaaaaaaaaaaaaaaaaaaa','0111111111','2026-01-27 23:38:04.005554','2026-01-27 23:45:43.561412',1,3,NULL,0,'2026-01-27 23:45:40.277022',NULL,NULL,NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-28  0:35:18
