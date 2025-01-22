/*!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.19  Distrib 10.6.18-MariaDB, for debian-linux-gnu (x86_64)
--
-- Host: 127.0.0.1    Database: capstoneStore
-- ------------------------------------------------------
-- Server version	10.6.18-MariaDB-0ubuntu0.22.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Money`
--

DROP TABLE IF EXISTS `Money`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Money` (
  `money_id` int(11) NOT NULL AUTO_INCREMENT,
  `displayFormat` varchar(15) NOT NULL,
  `name` varchar(10) NOT NULL,
  PRIMARY KEY (`money_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='The list of money types available for usage';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Money`
--

LOCK TABLES `Money` WRITE;
/*!40000 ALTER TABLE `Money` DISABLE KEYS */;
INSERT INTO `Money` (`money_id`, `displayFormat`, `name`) VALUES (1,'nl_nl;2;€','Euro'),(2,'en_us;0;$','USD');
/*!40000 ALTER TABLE `Money` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Orders`
--

DROP TABLE IF EXISTS `Orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Orders` (
  `order_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) DEFAULT NULL COMMENT 'May be null when the user no longer exists',
  `money_id` int(11) NOT NULL,
  `order_date` datetime DEFAULT NULL,
  `statusId` int(11) NOT NULL,
  `orderTotal` double NOT NULL,
  PRIMARY KEY (`order_id`),
  KEY `ord_money_id_fk` (`money_id`),
  KEY `ord_user_id_fk` (`user_id`),
  CONSTRAINT `ord_money_id_fk` FOREIGN KEY (`money_id`) REFERENCES `Money` (`money_id`),
  CONSTRAINT `ord_user_id_fk` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='The table containing the general information about the orders';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Orders`
--

LOCK TABLES `Orders` WRITE;
/*!40000 ALTER TABLE `Orders` DISABLE KEYS */;
INSERT INTO `Orders` (`order_id`, `user_id`, `money_id`, `order_date`, `statusId`, `orderTotal`) VALUES (1,3,1,'2025-01-16 14:44:11',1,24.2),(2,3,1,'2025-01-16 15:43:03',1,400001090),(3,3,1,'2025-01-16 15:46:11',2,4360),(4,3,1,'2025-01-16 15:47:15',1,10900),(5,3,1,'2025-01-16 15:48:26',2,11990),(6,3,1,'2025-01-20 09:23:32',0,24.2),(7,3,1,'2025-01-20 09:30:22',0,24.2),(8,3,1,'2025-01-20 09:33:08',0,24.2),(9,3,1,'2025-01-20 10:04:42',0,24.2),(10,3,1,'2025-01-20 10:06:12',0,24.2),(11,3,1,'2025-01-20 10:07:18',2,24.2),(12,3,1,'2025-01-20 10:08:08',0,24.2),(13,3,1,'2025-01-20 10:08:40',0,24.2),(14,3,1,'2025-01-20 10:29:21',0,242),(15,3,1,'2025-01-20 10:30:49',0,1196820),(16,NULL,1,'2025-01-20 11:05:16',1,10972.6),(17,NULL,1,'2025-01-20 11:05:39',2,1900000000),(18,NULL,1,'2025-01-20 11:33:22',0,12400156960),(19,4,1,'2025-01-20 11:38:01',0,24.2),(20,NULL,1,'2025-01-21 11:18:14',2,134070),(21,3,1,'2025-01-21 11:31:47',0,-2180);
/*!40000 ALTER TABLE `Orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Products`
--

DROP TABLE IF EXISTS `Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Products` (
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `money_id` int(11) NOT NULL,
  `taxes_id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `description` text NOT NULL,
  `price` double NOT NULL,
  PRIMARY KEY (`product_id`),
  KEY `Prod_money_id_fk` (`money_id`),
  KEY `Prod_taxes_id_fk` (`taxes_id`),
  CONSTRAINT `Prod_money_id_fk` FOREIGN KEY (`money_id`) REFERENCES `Money` (`money_id`),
  CONSTRAINT `Prod_taxes_id_fk` FOREIGN KEY (`taxes_id`) REFERENCES `Taxes` (`taxes_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Products`
--

LOCK TABLES `Products` WRITE;
/*!40000 ALTER TABLE `Products` DISABLE KEYS */;
INSERT INTO `Products` (`product_id`, `money_id`, `taxes_id`, `name`, `description`, `price`) VALUES (1,1,3,'cake','the mighties cake out there',20),(2,2,1,'the powerd caker','the caker that has recieved power',1000),(3,1,2,'the caked','the subject of many cakes',100000000);
/*!40000 ALTER TABLE `Products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Settings`
--

DROP TABLE IF EXISTS `Settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Settings` (
  `DefaultLang` varchar(5) NOT NULL,
  `settingsId` int(11) NOT NULL AUTO_INCREMENT,
  `AvailableLangs` varchar(255) NOT NULL,
  `MaxInputLoop` int(11) NOT NULL,
  `DefaultMoney` int(11) NOT NULL,
  `DefaultTaxes` int(11) NOT NULL,
  PRIMARY KEY (`settingsId`),
  KEY `Settings_Money_money_id_fk` (`DefaultMoney`),
  KEY `Settings_Taxes_taxes_id_fk` (`DefaultTaxes`),
  CONSTRAINT `Settings_Money_money_id_fk` FOREIGN KEY (`DefaultMoney`) REFERENCES `Money` (`money_id`),
  CONSTRAINT `Settings_Taxes_taxes_id_fk` FOREIGN KEY (`DefaultTaxes`) REFERENCES `Taxes` (`taxes_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Settings`
--

LOCK TABLES `Settings` WRITE;
/*!40000 ALTER TABLE `Settings` DISABLE KEYS */;
INSERT INTO `Settings` (`DefaultLang`, `settingsId`, `AvailableLangs`, `MaxInputLoop`, `DefaultMoney`, `DefaultTaxes`) VALUES ('nl_nl',1,'en_us,nl_nl',5,1,2);
/*!40000 ALTER TABLE `Settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Taxes`
--

DROP TABLE IF EXISTS `Taxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Taxes` (
  `taxes_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(10) NOT NULL,
  `percent` int(11) NOT NULL,
  PRIMARY KEY (`taxes_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='The list of tax types available for usage';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Taxes`
--

LOCK TABLES `Taxes` WRITE;
/*!40000 ALTER TABLE `Taxes` DISABLE KEYS */;
INSERT INTO `Taxes` (`taxes_id`, `name`, `percent`) VALUES (1,'Btw 9%',9),(2,'Btw 0%',0),(3,'Btw 21%',21);
/*!40000 ALTER TABLE `Taxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderProducts`
--

DROP TABLE IF EXISTS `orderProducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `orderProducts` (
  `order_product_id` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `taxes_id` int(11) NOT NULL,
  `money_id` int(11) NOT NULL,
  `count` int(11) NOT NULL,
  `pcsPrice` double NOT NULL,
  `total` double NOT NULL,
  PRIMARY KEY (`order_product_id`),
  KEY `orderProducts_Money_money_id_fk` (`money_id`),
  KEY `orderProducts_Orders_order_id_fk` (`order_id`),
  KEY `orderProducts_Products_product_id_fk` (`product_id`),
  KEY `orderProducts_Taxes_taxes_id_fk` (`taxes_id`),
  CONSTRAINT `orderProducts_Money_money_id_fk` FOREIGN KEY (`money_id`) REFERENCES `Money` (`money_id`),
  CONSTRAINT `orderProducts_Orders_order_id_fk` FOREIGN KEY (`order_id`) REFERENCES `Orders` (`order_id`),
  CONSTRAINT `orderProducts_Products_product_id_fk` FOREIGN KEY (`product_id`) REFERENCES `Products` (`product_id`),
  CONSTRAINT `orderProducts_Taxes_taxes_id_fk` FOREIGN KEY (`taxes_id`) REFERENCES `Taxes` (`taxes_id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='The table containing all the products related to orders';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderProducts`
--

LOCK TABLES `orderProducts` WRITE;
/*!40000 ALTER TABLE `orderProducts` DISABLE KEYS */;
INSERT INTO `orderProducts` (`order_product_id`, `order_id`, `product_id`, `taxes_id`, `money_id`, `count`, `pcsPrice`, `total`) VALUES (1,9,1,3,1,1,20,24.2),(2,10,1,3,1,1,20,24.2),(3,11,1,3,1,1,20,24.2),(4,12,1,3,1,1,20,24.2),(5,13,1,3,1,1,20,24.2),(6,14,1,3,1,10,20,242),(7,15,2,1,2,1098,1000,1196820),(8,16,1,3,1,3,20,72.6),(9,16,2,1,2,10,1000,10900),(10,17,3,2,1,19,100000000,1900000000),(11,18,2,1,2,134,1000,146060),(12,18,3,2,1,124,100000000,12400000000),(13,18,2,1,2,10,1000,10900),(14,19,1,3,1,1,20,24.2),(15,20,2,1,2,123,1000,134070),(16,21,2,1,2,12,1000,13080);
/*!40000 ALTER TABLE `orderProducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` text NOT NULL COMMENT 'this is set to TEXT temporarily while the password handling is implemented and then it will be set to the correct type',
  `email` varchar(255) NOT NULL,
  `adres_street` varchar(255) NOT NULL,
  `adres_number` int(11) NOT NULL,
  `adres_add` varchar(5) NOT NULL,
  `adres_postal` char(6) NOT NULL,
  `adres_city` varchar(255) NOT NULL,
  `auth_id` int(11) NOT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `usernameKey` (`username`),
  KEY `auth_id_fk` (`auth_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='The table with all the information about the users';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`user_id`, `username`, `password`, `email`, `adres_street`, `adres_number`, `adres_add`, `adres_postal`, `adres_city`, `auth_id`) VALUES (1,'reventcake','password','test@example.com','test',1,'a','1111VJ','Development',2),(3,'cake','cake','test','development',12,'','3333CZ','test',1),(4,'caked','caked','ea','test',12,'a','1234AZ','City',1);
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

-- Dump completed on 2025-01-22 11:05:41
