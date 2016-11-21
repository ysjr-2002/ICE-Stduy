/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50553
Source Host           : localhost:3306
Source Database       : personrepository

Target Server Type    : MYSQL
Target Server Version : 50553
File Encoding         : 65001

Date: 2016-11-21 22:40:25
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for persons
-- ----------------------------
DROP TABLE IF EXISTS `persons`;
CREATE TABLE `persons` (
  `FaceID` varchar(255) NOT NULL DEFAULT '',
  `UUID` varchar(255) NOT NULL DEFAULT '',
  `Code` varchar(255) NOT NULL DEFAULT '',
  `Name` varchar(255) DEFAULT NULL,
  `Gender` bit(1) DEFAULT NULL,
  `ImageData1` longtext,
  `SignatureCode1` longtext,
  `HasSignatureCode1` bit(1) DEFAULT NULL,
  `ImageData2` longtext,
  `SignatureCode2` longtext,
  `HasSignatureCode2` bit(1) DEFAULT NULL,
  `ImageData3` longtext,
  `SignatureCode3` longtext,
  `HasSignatureCode3` bit(1) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CreateTime` datetime NOT NULL,
  PRIMARY KEY (`FaceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for persontags
-- ----------------------------
DROP TABLE IF EXISTS `persontags`;
CREATE TABLE `persontags` (
  `TagID` int(11) NOT NULL AUTO_INCREMENT,
  `FaceID` varchar(255) DEFAULT NULL,
  `TagName` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`TagID`)
) ENGINE=InnoDB AUTO_INCREMENT=63319 DEFAULT CHARSET=utf8;

-- ----------------------------
-- View structure for persontagview
-- ----------------------------
DROP VIEW IF EXISTS `persontagview`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `persontagview` AS select distinct `p`.`FaceID` AS `FaceID`,`p`.`UUID` AS `UUID`,`p`.`Code` AS `Code`,`p`.`Name` AS `Name`,`p`.`Gender` AS `Gender`,`p`.`ImageData1` AS `ImageData1`,`p`.`SignatureCode1` AS `SignatureCode1`,`p`.`HasSignatureCode1` AS `HasSignatureCode1`,`p`.`ImageData2` AS `ImageData2`,`p`.`SignatureCode2` AS `SignatureCode2`,`p`.`HasSignatureCode2` AS `HasSignatureCode2`,`p`.`ImageData3` AS `ImageData3`,`p`.`SignatureCode3` AS `SignatureCode3`,`p`.`HasSignatureCode3` AS `HasSignatureCode3`,`pt`.`TagName` AS `TagName` from (`persons` `p` join `persontags` `pt`) where (`p`.`FaceID` = `pt`.`FaceID`) ;
SET FOREIGN_KEY_CHECKS=1;
