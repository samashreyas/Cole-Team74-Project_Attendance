-- Created By Sean Purnell
-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               8.0.40 - MySQL Community Server - GPL
-- Server OS:                    Win64
-- HeidiSQL Version:             12.8.0.6908
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for attendance_info
CREATE DATABASE IF NOT EXISTS `attendance_info` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `attendance_info`;

-- Dumping structure for table attendance_info.attendance
CREATE TABLE IF NOT EXISTS `attendance` (
  `studentID` int NOT NULL AUTO_INCREMENT,
  `classID` int NOT NULL,
  `sessionDate` date NOT NULL,
  `ip_address` int unsigned NOT NULL,
  `signin_time` time NOT NULL,
  `quiz_responses` int NOT NULL,
  `status` char(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`studentID`,`classID`,`sessionDate`),
  KEY `classID` (`classID`),
  KEY `sessionDate` (`sessionDate`),
  CONSTRAINT `FK_attendance_quiz responses` FOREIGN KEY (`studentID`) REFERENCES `quiz responses` (`attendanceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.attendance: ~0 rows (approximately)

-- Dumping structure for table attendance_info.course
CREATE TABLE IF NOT EXISTS `course` (
  `courseID` int NOT NULL AUTO_INCREMENT,
  `instructorID` int NOT NULL,
  `course_code` int NOT NULL,
  `course_name` varchar(255) NOT NULL,
  PRIMARY KEY (`courseID`),
  KEY `instructorID` (`instructorID`),
  CONSTRAINT `FK_course_attendance` FOREIGN KEY (`courseID`) REFERENCES `attendance` (`studentID`),
  CONSTRAINT `FK_course_attendance_2` FOREIGN KEY (`courseID`) REFERENCES `attendance` (`classID`),
  CONSTRAINT `FK_course_enrollment` FOREIGN KEY (`courseID`) REFERENCES `enrollment` (`class_id`),
  CONSTRAINT `FK_course_sessions` FOREIGN KEY (`courseID`) REFERENCES `sessions` (`courseID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.course: ~0 rows (approximately)

-- Dumping structure for table attendance_info.enrollment
CREATE TABLE IF NOT EXISTS `enrollment` (
  `enrollmentID` int NOT NULL AUTO_INCREMENT,
  `student_id` int NOT NULL,
  `class_id` int NOT NULL,
  PRIMARY KEY (`enrollmentID`),
  KEY `class_id` (`class_id`),
  KEY `student_id` (`student_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.enrollment: ~0 rows (approximately)

-- Dumping structure for table attendance_info.quiz banks
CREATE TABLE IF NOT EXISTS `quiz banks` (
  `bankID` int NOT NULL AUTO_INCREMENT,
  `bankName` varchar(255) NOT NULL,
  `course_name` varchar(255) NOT NULL,
  `course_code` int NOT NULL,
  PRIMARY KEY (`bankID`),
  CONSTRAINT `FK_quiz banks_quiz questions` FOREIGN KEY (`bankID`) REFERENCES `quiz questions` (`bankID`),
  CONSTRAINT `FK_quiz banks_sessions` FOREIGN KEY (`bankID`) REFERENCES `sessions` (`quiz_bank_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.quiz banks: ~0 rows (approximately)

-- Dumping structure for table attendance_info.quiz questions
CREATE TABLE IF NOT EXISTS `quiz questions` (
  `questionID` int NOT NULL AUTO_INCREMENT,
  `bankID` int NOT NULL,
  `question_text` int NOT NULL,
  `options` int NOT NULL,
  `correct_answer` int NOT NULL,
  PRIMARY KEY (`questionID`),
  KEY `bankID` (`bankID`),
  CONSTRAINT `FK_quiz questions_quiz responses` FOREIGN KEY (`questionID`) REFERENCES `quiz responses` (`question_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.quiz questions: ~0 rows (approximately)

-- Dumping structure for table attendance_info.quiz responses
CREATE TABLE IF NOT EXISTS `quiz responses` (
  `responseID` int NOT NULL AUTO_INCREMENT,
  `attendanceID` int NOT NULL,
  `question_id` int NOT NULL,
  `selected_answer` int NOT NULL,
  PRIMARY KEY (`responseID`),
  KEY `attendanceID` (`attendanceID`),
  KEY `question_id` (`question_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.quiz responses: ~0 rows (approximately)

-- Dumping structure for table attendance_info.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `roleID` int NOT NULL AUTO_INCREMENT,
  `role_name` varchar(255) NOT NULL,
  PRIMARY KEY (`roleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.roles: ~0 rows (approximately)

-- Dumping structure for table attendance_info.session password
CREATE TABLE IF NOT EXISTS `session password` (
  `passwordID` int NOT NULL AUTO_INCREMENT,
  `password` varchar(50) NOT NULL,
  `course_id` int NOT NULL,
  `session_date` date NOT NULL,
  PRIMARY KEY (`passwordID`),
  KEY `FK_session password_sessions_2` (`session_date`),
  CONSTRAINT `FK_session password_sessions` FOREIGN KEY (`passwordID`) REFERENCES `sessions` (`password_id`),
  CONSTRAINT `FK_session password_sessions_2` FOREIGN KEY (`session_date`) REFERENCES `sessions` (`session_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.session password: ~0 rows (approximately)

-- Dumping structure for table attendance_info.sessions
CREATE TABLE IF NOT EXISTS `sessions` (
  `courseID` int NOT NULL AUTO_INCREMENT,
  `session_date` date NOT NULL,
  `quiz_bank_id` int NOT NULL,
  `password_id` int NOT NULL,
  PRIMARY KEY (`courseID`,`session_date`,`quiz_bank_id`),
  KEY `quiz_bank_id` (`quiz_bank_id`),
  KEY `session_date` (`session_date`),
  KEY `password_id` (`password_id`),
  CONSTRAINT `FK_sessions_attendance` FOREIGN KEY (`session_date`) REFERENCES `attendance` (`sessionDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.sessions: ~0 rows (approximately)

-- Dumping structure for table attendance_info.user
CREATE TABLE IF NOT EXISTS `user` (
  `userID` int NOT NULL AUTO_INCREMENT,
  `last_name` varchar(255) NOT NULL,
  `first_name` varchar(255) NOT NULL,
  `role_id` int NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `UTD_ID` int NOT NULL,
  PRIMARY KEY (`userID`),
  KEY `FK_user_roles` (`role_id`),
  CONSTRAINT `FK_user_attendance` FOREIGN KEY (`userID`) REFERENCES `attendance` (`studentID`),
  CONSTRAINT `FK_user_course` FOREIGN KEY (`userID`) REFERENCES `course` (`instructorID`),
  CONSTRAINT `FK_user_enrollment` FOREIGN KEY (`userID`) REFERENCES `enrollment` (`student_id`),
  CONSTRAINT `FK_user_roles` FOREIGN KEY (`role_id`) REFERENCES `roles` (`roleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table attendance_info.user: ~0 rows (approximately)

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
