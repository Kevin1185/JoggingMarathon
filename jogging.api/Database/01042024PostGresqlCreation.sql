-- MySQL dump 10.13  Distrib 8.1.0, for macos13 (arm64)
--
-- Host: localhost    Database: jogging2
-- ------------------------------------------------------
-- Server version	8.1.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE DATABASE IF NOT EXISTS jogging2;
USE jogging2;

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
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `address`
--

DROP TABLE IF EXISTS `address`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `address` (
  `AddressId` int NOT NULL AUTO_INCREMENT,
  `Street` varchar(100) DEFAULT NULL,
  `City` varchar(100) NOT NULL,
  `HouseNumber` varchar(10) DEFAULT NULL,
  `ZipCode` varchar(10) DEFAULT NULL,
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`AddressId`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `address`
--

LOCK TABLES `address` WRITE;
/*!40000 ALTER TABLE `address` DISABLE KEYS */;
INSERT INTO `address` VALUES (35,'Koning Albertlaan','Brussel','101','1000','ExtendedAddress'),(36,'Meir','Antwerpen','202','2000','ExtendedAddress'),(37,'Leopoldlaan','Leuven','303','3000','ExtendedAddress'),(38,'Graslei','Gent','404','9000','ExtendedAddress'),(39,'Lange Gasthuisstraat','Brugge','505','8000','ExtendedAddress');
/*!40000 ALTER TABLE `address` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AgeCategory`
--

DROP TABLE IF EXISTS `AgeCategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AgeCategory` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `MinimumAge` int DEFAULT NULL,
  `MaximumAge` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AgeCategory`
--

LOCK TABLES `AgeCategory` WRITE;
/*!40000 ALTER TABLE `AgeCategory` DISABLE KEYS */;
INSERT INTO `AgeCategory` VALUES (5,'Geboren na 1989',0,35),(6,'Geboren van 1979 tot 1988',36,46),(7,'Geboren van 1969 tot 1978',47,56),(8,'Geboren voor 1969',57,150);
/*!40000 ALTER TABLE `AgeCategory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `auth_users`
--

DROP TABLE IF EXISTS `auth_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auth_users` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `confirm_token` varchar(255) DEFAULT NULL,
  `email_confirmed_at` datetime DEFAULT NULL,
  `confirmation_sent_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auth_users`
--

LOCK TABLES `auth_users` WRITE;
/*!40000 ALTER TABLE `auth_users` DISABLE KEYS */;
/*!40000 ALTER TABLE `auth_users` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `Competition`
--

DROP TABLE IF EXISTS `Competition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Competition` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Date` datetime DEFAULT NULL,
  `AddressId` int DEFAULT NULL,
  `Active` tinyint(1) NOT NULL DEFAULT '0',
  `img_url` text,
  `Information` text,
  `url` text,
  `RankingActive` tinyint(1) NOT NULL DEFAULT '0',
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Competition_ibfk_1` (`AddressId`),
  CONSTRAINT `Competition_ibfk_1` FOREIGN KEY (`AddressId`) REFERENCES `address` (`AddressId`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Competition`
--

LOCK TABLES `Competition` WRITE;
/*!40000 ALTER TABLE `Competition` DISABLE KEYS */;
INSERT INTO `Competition` VALUES (11,'39ste Veense Jogging\n','2024-10-06 09:00:00',35,1,'https://example.com/winter-marathon.jpg','14.40 u. : Kidsrun 0,5km - (0 tot 7 jaar)\n\n14.50 u. : Kidsrun 1km (8 tot 12 jaar)\n\n15.00 u. : Jogging € 7 (3,5 - 7 - 10,5 of 14 km)\n\nStart en aankomst Speelplein Tervenen, naturaprijs voor iedere deelnemer en voor de kinderen een medaille.\n\nhttps://www.facebook.com/TervenenKermis ','https://wintermarathon2025.be',1,'ExtendedCompetition'),(12,'Jogging Volpenswege','2024-09-13 08:00:00',36,1,'https://example.com/summer-run.jpg','Start en aankomst aan de feesttent\n\nInschrijven vanaf 17.30u in de feesttent\n\n18.45 u. : Kinderloop € 3 (1 km)\n\n19.00 u. : Jogging € 7 (3, 6, 9 of 12 km)\n\nWaterbedeling, wasgelegenheid, gratis bewaakte sporttassenzone, verkeersvrij parcours, aandenken voor iedere deelnemer.\n\nwww.volpenswegekermis.be','https://summerrun2025.be',1,'ExtendedCompetition'),(13,'Eeksken feesten','2024-06-01 10:00:00',37,1,'https://example.com/city-challenge.jpg','In samenwerking met de Kozirunners. Volledig verhard en verkeersvrij parcours. Mooie prijs voor eerste man en vrouw op het speciale Strava-segment \'Linde 72\'. Prijs Medaille en gratis ijsje voor elk kind. \n\n✍ 09:00: Start inschrijvingen en betalen\n\n🏃‍♂️ 10:00: Start Kids Run (1 km)\n\n🏃‍♀️ 10:30 Start jogging (lus van 5, 10 of 15 km)\n\nEr zijn kleedkamers in de basisschool van ‘t Eeksken, maar geen douches.\n\nwww.eekskenfeesten.be ','https://citychallenge2025.be',1,'ExtendedCompetition'),(14,'Zandekensjogging','2024-07-06 22:00:00',38,1,'https://example.com/night-run.jpg','Tent aan Zandekenbrug\n\n14.00 u. : Kidsrun - € 3\nkids tot 8 jaar - 500m \nkids 8 tot 12 jaar - 1000m - € 3\n\n15.00 u. : Jogging - € 7\n\nParcours: van Zandekenbrug naar Hultjenbrug en terug naar Zandekenbrug. Verkeersvrij parcours.\n\nNaturaprijs voor iedere deelnemer en tombola achteraf.','https://nightrun2025.be',1,'ExtendedCompetition'),(15,'Chirofeesten','2024-04-19 09:00:00',39,1,'https://example.com/brugge-half-marathon.jpg','Chirolokaal\n\n19.00 u. : Kidsrun, tot 12 jaar, gratis deelname, 1500m op een verkeersvrij parcours, leuk aandenken voor iedere deelnemer\n\n19.30 u. : Jogging, afstanden van 3, 6, 9 of 12 km op een verkeersvrij parcours. De deelname is €7 en ook hier ontvangt iedere jogger een mooi aandenken\n\nMet douchegelegenheid\n\nwww.chirofeestensleidinge.be','https://bruggehalfmarathon2025.be',1,'ExtendedCompetition');
/*!40000 ALTER TABLE `Competition` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CompetitionPerCategory`
--

DROP TABLE IF EXISTS `CompetitionPerCategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CompetitionPerCategory` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Gender` char(1) DEFAULT NULL,
  `DistanceName` varchar(30) DEFAULT NULL,
  `DistanceInKm` float DEFAULT NULL,
  `AgeCategoryId` int DEFAULT NULL,
  `CompetitionId` int DEFAULT NULL,
  `GunTime` datetime DEFAULT NULL,
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `AgeCategoryId` (`AgeCategoryId`),
  KEY `CompetitionId` (`CompetitionId`),
  CONSTRAINT `CompetitionPerCategory_ibfk_1` FOREIGN KEY (`AgeCategoryId`) REFERENCES `AgeCategory` (`Id`),
  CONSTRAINT `CompetitionPerCategory_ibfk_2` FOREIGN KEY (`CompetitionId`) REFERENCES `Competition` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=133 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CompetitionPerCategory`
--

LOCK TABLES `CompetitionPerCategory` WRITE;
/*!40000 ALTER TABLE `CompetitionPerCategory` DISABLE KEYS */;
INSERT INTO `CompetitionPerCategory` VALUES (97,'M','Korte Afstand',5,5,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(98,'M','Middellange Afstand',10,6,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(99,'M','Lange Afstand',21,7,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(100,'F','Korte Afstand',5,5,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(101,'F','Middellange Afstand',10,6,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(102,'F','Lange Afstand',21,7,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(103,'M','Korte Afstand',5,5,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(104,'M','Middellange Afstand',10,6,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(105,'M','Lange Afstand',21,7,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(106,'F','Korte Afstand',5,5,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(107,'F','Middellange Afstand',10,6,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(108,'F','Lange Afstand',21,7,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(109,'M','Korte Afstand',5,5,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(110,'M','Middellange Afstand',10,6,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(111,'M','Lange Afstand',21,7,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(112,'F','Korte Afstand',5,5,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(113,'F','Middellange Afstand',10,6,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(114,'F','Lange Afstand',21,7,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(115,'M','Korte Afstand',5,5,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(116,'M','Middellange Afstand',10,6,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(117,'M','Lange Afstand',21,7,11,'2025-01-05 09:30:00','ExtendedCompetitionpercategory'),(118,'F','Korte Afstand',5,5,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(119,'F','Middellange Afstand',10,6,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(120,'F','Lange Afstand',21,7,12,'2025-06-12 08:30:00','ExtendedCompetitionpercategory'),(121,'M','Korte Afstand',5,5,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(122,'M','Middellange Afstand',10,6,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(123,'M','Lange Afstand',21,7,13,'2025-03-15 10:30:00','ExtendedCompetitionpercategory'),(124,'F','Korte Afstand',5,5,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(125,'F','Middellange Afstand',10,6,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(126,'F','Lange Afstand',21,7,14,'2025-07-20 22:30:00','ExtendedCompetitionpercategory'),(127,'M','Korte Afstand',5,5,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(128,'M','Middellange Afstand',10,6,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(129,'M','Lange Afstand',21,7,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(130,'F','Korte Afstand',5,5,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(131,'F','Middellange Afstand',10,6,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory'),(132,'F','Lange Afstand',21,7,15,'2025-04-25 09:30:00','ExtendedCompetitionpercategory');
/*!40000 ALTER TABLE `CompetitionPerCategory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `person`
--

DROP TABLE IF EXISTS `person`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `person` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `LastName` varchar(50) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `BirthDate` date NOT NULL,
  `IBANNumber` varchar(30) DEFAULT NULL,
  `SchoolId` int DEFAULT NULL,
  `AddressId` int DEFAULT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Gender` varchar(10) DEFAULT '',
  `Email` varchar(100) DEFAULT NULL,
  `RunningClubId` int DEFAULT NULL,
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId` (`UserId`),
  KEY `SchoolId` (`SchoolId`),
  KEY `idx_Person_firstname` (`FirstName`),
  KEY `idx_Person_lastname` (`LastName`),
  KEY `Person_ibfk_3` (`RunningClubId`),
  KEY `Person_ibfk_1` (`AddressId`),
  CONSTRAINT `FK_Person_Profile_UserId` FOREIGN KEY (`UserId`) REFERENCES `Profile` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `Person_ibfk_1` FOREIGN KEY (`AddressId`) REFERENCES `address` (`AddressId`) ON DELETE RESTRICT,
  CONSTRAINT `Person_ibfk_2` FOREIGN KEY (`SchoolId`) REFERENCES `school` (`SchoolId`),
  CONSTRAINT `Person_ibfk_3` FOREIGN KEY (`RunningClubId`) REFERENCES `runningclub` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `person`
--

LOCK TABLES `person` WRITE;
/*!40000 ALTER TABLE `person` DISABLE KEYS */;
INSERT INTO `person` VALUES (108,'Janssens','Anna','1990-05-15','BE68539007547034',32,35,'b7e3c8a1-bd29-4339-b6f1-c01d344e84e5','V','anna.janssens@example.com',2,'ExtendedPerson'),(109,'Peeters','Mark','1985-09-23','BE71096123456789',33,36,'9d0b9b95-cb89-456f-b3ac-e0f1573d7a90','M','mark.peeters@example.com',8,'ExtendedPerson'),(110,'De Smet','Sofie','1995-12-02','BE79345567891234',34,37,'8f9c99b2-6a96-40d5-b40c-4d2f178d2f92','V','sofie.desmet@example.com',5,'ExtendedPerson'),(111,'Vandenberghe','Tom','1988-02-12','BE60299874612345',35,38,'4b7be2d9-7cc6-4b67-a08b-0c616a1a1b8e','M','tom.vandenberghe@example.com',6,'ExtendedPerson'),(112,'Lemoine','Julie','1992-08-30','BE63041012345678',36,39,'f3c33e2b-e3b4-4d57-bf24-5084b1d4f56d','V','julie.lemoine@example.com',7,'ExtendedPerson');
/*!40000 ALTER TABLE `person` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `handle_new_user` BEFORE INSERT ON `person` FOR EACH ROW BEGIN
    INSERT INTO Profile (id, role)
    VALUES (NEW.UserId, 'User');
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `after_Person_delete` AFTER DELETE ON `person` FOR EACH ROW BEGIN
    DELETE FROM `auth_users` WHERE `id` = OLD.`UserId`;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `delete_from_Person_details` AFTER DELETE ON `person` FOR EACH ROW BEGIN
    DELETE FROM auth_users WHERE id = OLD.UserId;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `Personview`
--

DROP TABLE IF EXISTS `Personview`;
/*!50001 DROP VIEW IF EXISTS `Personview`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `Personview` AS SELECT 
 1 AS `Id`,
 1 AS `LastName`,
 1 AS `FirstName`,
 1 AS `BirthDate`,
 1 AS `IBANNumber`,
 1 AS `SchoolId`,
 1 AS `AddressId`,
 1 AS `UserId`,
 1 AS `Gender`,
 1 AS `Email`,
 1 AS `fullname`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `Profile`
--

DROP TABLE IF EXISTS `Profile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Profile` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `role` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_Profile_id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Profile`
--

LOCK TABLES `Profile` WRITE;
/*!40000 ALTER TABLE `Profile` DISABLE KEYS */;
INSERT INTO `Profile` VALUES ('4b7be2d9-7cc6-4b67-a08b-0c616a1a1b8e','User'),('8f9c99b2-6a96-40d5-b40c-4d2f178d2f92','User'),('9d0b9b95-cb89-456f-b3ac-e0f1573d7a90','User'),('b7e3c8a1-bd29-4339-b6f1-c01d344e84e5','User'),('f3c33e2b-e3b4-4d57-bf24-5084b1d4f56d','User');
/*!40000 ALTER TABLE `Profile` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Registration`
--

DROP TABLE IF EXISTS `Registration`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Registration` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RunNumber` smallint DEFAULT NULL,
  `RunTime` text,
  `CompetitionPerCategoryId` int DEFAULT NULL,
  `Paid` tinyint(1) NOT NULL DEFAULT '0',
  `PersonId` int DEFAULT NULL,
  `CompetitionId` int DEFAULT NULL,
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RunNumber` (`RunNumber`,`CompetitionId`),
  KEY `CompetitionPerCategoryId` (`CompetitionPerCategoryId`),
  KEY `PersonId` (`PersonId`),
  KEY `CompetitionId` (`CompetitionId`),
  CONSTRAINT `Registration_ibfk_1` FOREIGN KEY (`CompetitionPerCategoryId`) REFERENCES `CompetitionPerCategory` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Registration_ibfk_2` FOREIGN KEY (`PersonId`) REFERENCES `person` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Registration_ibfk_3` FOREIGN KEY (`CompetitionId`) REFERENCES `Competition` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=157 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Registration`
--

LOCK TABLES `Registration` WRITE;
/*!40000 ALTER TABLE `Registration` DISABLE KEYS */;
INSERT INTO `Registration` VALUES (9,12345,'00:45:00',97,1,108,11,'ExtendedRegistration'),(11,12346,'00:47:00',97,1,109,11,'ExtendedRegistration'),(154,12347,'00:48:00',98,1,112,11,'ExtendedRegistration'),(155,12348,'00:50:00',98,1,110,11,'ExtendedRegistration'),(156,12349,'00:49:30',99,1,111,11,'ExtendedRegistration');
/*!40000 ALTER TABLE `Registration` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `runningclub`
--

DROP TABLE IF EXISTS `runningclub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `runningclub` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Url` varchar(200) DEFAULT NULL,
  `AdminChecked` tinyint(1) NOT NULL DEFAULT '0',
  `Logo` blob,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `runningclub`
--

LOCK TABLES `runningclub` WRITE;
/*!40000 ALTER TABLE `runningclub` DISABLE KEYS */;
INSERT INTO `runningclub` VALUES (2,'Kozi Runners','PUTTest.com',1,_binary 'https://iili.io/2UzyIBs.png'),(3,'Elite Runners','https://www.eliterunners.com',1,_binary '﻿https://iili.io/2UzyIBs.png'),(4,'City Joggers','https://www.cityjoggers.be',1,_binary '﻿https://iili.io/2UzyTEG.png'),(5,'Mountain Trail Club','https://www.mountaintrailclub.org',1,_binary '﻿https://iili.io/2gC0n0N.md.jpg'),(6,'Speed Demons','https://www.speeddemons.net',1,_binary '﻿https://iili.io/2gC0fWv.jpg'),(7,'Run for Fun','https://www.runforfun.club',1,_binary '﻿https://iili.io/2gC0qsR.md.jpg'),(8,'Lucky Runners','www.elline-webdesign.com',1,_binary 'https://iili.io/2UzyTEG.png');
/*!40000 ALTER TABLE `runningclub` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `school`
--

DROP TABLE IF EXISTS `school`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `school` (
  `SchoolId` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Discriminator` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`SchoolId`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `school`
--

LOCK TABLES `school` WRITE;
/*!40000 ALTER TABLE `school` DISABLE KEYS */;
INSERT INTO `school` VALUES (32,'Stadsscholen Antwerpen','ExtendedSchool'),(33,'Leuven College','ExtendedSchool'),(34,'Vrije Universiteit Brussel','ExtendedSchool'),(35,'Karel de Grote Hogeschool','ExtendedSchool'),(36,'Universiteit Gent','ExtendedSchool');
/*!40000 ALTER TABLE `school` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `Personview`
--
-- ------------------------------------------------------
-- Create dedicated user for jogging2 database
-- ------------------------------------------------------

-- You can change the password below to a strong one before running this.
CREATE USER IF NOT EXISTS 'evergemsejogging'@'%' IDENTIFIED BY 'evergemsejogging123';

-- Grant access only to jogging2 database
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON jogging2.* TO 'evergemsejogging'@'%';

-- Always follow up with FLUSH PRIVILEGES if not in init script
-- FLUSH PRIVILEGES;

-- ------------------------------------------------------
-- End of file
-- ------------------------------------------------------



DELIMITER $$

-- Function: find_user_email
CREATE FUNCTION find_user_email(email_address VARCHAR(255))
RETURNS JSON
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE result JSON;
    SELECT JSON_OBJECT('id', id, 'email', email) INTO result
    FROM person 
    WHERE email = email_address
    LIMIT 1;
    RETURN result;
END$$

-- Function: get_Competition_Registrations
CREATE FUNCTION get_Competition_Registrations(Competitionid INT, searchvalue TEXT)
RETURNS JSON
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE result JSON;
    SELECT JSON_ARRAYAGG(
        JSON_OBJECT(
            'Registration_id', r.Id,
            'run_number', r.RunNumber,
            'run_time', r.RunTime,
            'Competition_per_category_id', r.CompetitionPerCategoryId,
            'paid', r.Paid,
            'Person_id', r.PersonId,
            'Competition_id', r.CompetitionId,
            'last_name', p.LastName,
            'first_name', p.FirstName,
            'birth_date', p.BirthDate,
            'iban_number', p.IBANNumber,
            'school_id', p.SchoolId,
            'address_id', p.AddressId,
            'gender', p.Gender,
            'user_id', p.UserId,
            'street', a.Street,
            'house_number', a.HouseNumber,
            'city', a.City,
            'zip_code', a.ZipCode,
            'distance_name', cpc.DistanceName
        )
    ) INTO result
    FROM Registration r
    JOIN Personview p ON p.Id = r.PersonId
    JOIN Address a ON a.Id = p.AddressId
    JOIN CompetitionPerCategory cpc ON cpc.Id = r.CompetitionPerCategoryId
    WHERE r.CompetitionId = Competitionid
    AND CONCAT(p.FirstName, ' ', p.LastName) LIKE CONCAT('%', searchvalue, '%');
    RETURN result;
END$$

-- Function: get_Persons_by_search_value
CREATE FUNCTION get_Persons_by_search_value(search_value TEXT)
RETURNS JSON
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE result JSON;
    SELECT JSON_ARRAYAGG(
        JSON_OBJECT(
            'Person_id', p.Id,
            'last_name', p.LastName,
            'first_name', p.FirstName,
            'birth_date', p.BirthDate,
            'iban_number', p.IBANNumber,
            'school_id', p.SchoolId,
            'address_id', p.AddressId,
            'gender', p.Gender,
            'user_id', p.UserId,
            'email', p.Email,
            'street', a.Street,
            'house_number', a.HouseNumber,
            'city', a.City,
            'zip_code', a.ZipCode,
            'user_role', pr.role
        )
    ) INTO result
    FROM Personview p
    JOIN Address a ON a.Id = p.AddressId
    LEFT JOIN Profile pr ON p.UserId = pr.id
    WHERE CONCAT(p.FirstName, ' ', p.LastName) LIKE CONCAT('%', search_value, '%');
    RETURN result;
END$$


-- Procedure: remove_user_email
CREATE PROCEDURE remove_user_email(IN email_address VARCHAR(255))
BEGIN
    DELETE FROM auth_users WHERE email = email_address;
END$$

-- Procedure: set_email_confirm_token
CREATE PROCEDURE set_email_confirm_token(IN confirm_token VARCHAR(255), IN email VARCHAR(255))
BEGIN
    UPDATE auth_users
    SET confirmation_token = confirm_token,
        email_confirmed_at = NULL,
        confirmation_sent_at = NOW()
    WHERE email = email;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User not found with this email';
    END IF;
END$$

-- Procedure: set_password_recovery_token
CREATE PROCEDURE set_password_recovery_token(IN recovery_token VARCHAR(255), IN email VARCHAR(255))
BEGIN
    UPDATE auth_users
    SET recovery_token = recovery_token,
        recovery_sent_at = NOW()
    WHERE email = email;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User not found with this email';
    END IF;
END$$

-- Procedure: update_user_email
CREATE PROCEDURE update_user_email(IN old_email_address VARCHAR(255), IN new_email_address VARCHAR(255))
BEGIN
    IF new_email_address NOT REGEXP '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = CONCAT('Invalid new email: ', new_email_address);
    END IF;

    UPDATE auth_users
    SET email = new_email_address
    WHERE email = old_email_address;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = CONCAT('Email address not found: ', old_email_address);
    END IF;
END$$

-- Procedure: update_user_password
CREATE PROCEDURE update_user_password(IN user_id CHAR(36), IN old_password VARCHAR(255), IN new_password VARCHAR(255))
BEGIN
    DECLARE match_count INT;

    SELECT COUNT(*) INTO match_count
    FROM auth_users
    WHERE id = user_id AND encrypted_password = bcrypt_check(old_password, encrypted_password);

    IF match_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Invalid old password';
    END IF;

    UPDATE auth_users
    SET encrypted_password = bcrypt_hash(new_password)
    WHERE id = user_id;
END$$

-- Procedure: update_user_password_recovery_token
CREATE PROCEDURE update_user_password_recovery_token(IN recovery_token VARCHAR(255), IN new_password VARCHAR(255))
BEGIN
    UPDATE auth_users
    SET encrypted_password = bcrypt_hash(new_password),
        recovery_token = '',
        confirmation_token = '',
        email_confirmed_at = NOW()
    WHERE recovery_token = recovery_token;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Recovery token not found';
    END IF;
END$$

DELIMITER ;





/*!50001 DROP VIEW IF EXISTS `Personview`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `Personview` AS select `person`.`Id` AS `Id`,`person`.`LastName` AS `LastName`,`person`.`FirstName` AS `FirstName`,`person`.`BirthDate` AS `BirthDate`,`person`.`IBANNumber` AS `IBANNumber`,`person`.`SchoolId` AS `SchoolId`,`person`.`AddressId` AS `AddressId`,`person`.`UserId` AS `UserId`,`person`.`Gender` AS `Gender`,`person`.`Email` AS `Email`,concat(`person`.`FirstName`,' ',`person`.`LastName`) AS `fullname` from `person` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-01-24 21:56:29


-- Primary keys are mostly already defined in your CREATE TABLE statements.
-- Here, we just ensure foreign keys are properly added with ON DELETE RESTRICT (or CASCADE if you want).

-- person table foreign keys
ALTER TABLE person
  ADD CONSTRAINT FK_Person_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_Person_address_AddressId FOREIGN KEY (AddressId) REFERENCES address(AddressId) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_Person_school_SchoolId FOREIGN KEY (SchoolId) REFERENCES school(Id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_Person_runningclub_RunningClubId FOREIGN KEY (RunningClubId) REFERENCES runningclub(Id) ON DELETE RESTRICT;

-- address table foreign key
ALTER TABLE address
  ADD CONSTRAINT FK_address_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT;

-- runningclub table foreign key
ALTER TABLE runningclub
  ADD CONSTRAINT FK_runningclub_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT;

-- race table foreign keys
ALTER TABLE race
  ADD CONSTRAINT FK_race_racecategory_RaceCategoryId FOREIGN KEY (RaceCategoryId) REFERENCES racecategory(Id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_race_racerun_RaceRunId FOREIGN KEY (RaceRunId) REFERENCES racerun(Id) ON DELETE RESTRICT;

-- racecategory table foreign key
ALTER TABLE racecategory
  ADD CONSTRAINT FK_racecategory_race_RaceId FOREIGN KEY (RaceId) REFERENCES race(Id) ON DELETE RESTRICT;

-- racerun table foreign keys
ALTER TABLE racerun
  ADD CONSTRAINT FK_racerun_race_RaceId FOREIGN KEY (RaceId) REFERENCES race(Id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_racerun_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT;

-- racecomment table foreign keys
ALTER TABLE racecomment
  ADD CONSTRAINT FK_racecomment_racerun_RaceRunId FOREIGN KEY (RaceRunId) REFERENCES racerun(Id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_racecomment_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT;

-- racecategorycomment table foreign keys
ALTER TABLE racecategorycomment
  ADD CONSTRAINT FK_racecategorycomment_racecategory_RaceCategoryId FOREIGN KEY (RaceCategoryId) REFERENCES racecategory(Id) ON DELETE RESTRICT,
  ADD CONSTRAINT FK_racecategorycomment_Profile_UserId FOREIGN KEY (UserId) REFERENCES Profile(id) ON DELETE RESTRICT;

