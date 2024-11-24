CREATE SCHEMA IF NOT EXISTS `var` DEFAULT CHARACTER SET utf8 ;
USE `var` ;

CREATE TABLE IF NOT EXISTS `var`.`typesOfQualification` (
  `qualificationID` INT NOT NULL AUTO_INCREMENT,
  `qualification` VARCHAR(20) NULL,
  PRIMARY KEY (`qualificationID`),
  UNIQUE INDEX `qualification_UNIQUE` (`qualification` ASC));

CREATE TABLE IF NOT EXISTS `var`.`typesOfProfession` (
  `professionID` INT NOT NULL AUTO_INCREMENT,
  `profession` VARCHAR(20) NULL,
  PRIMARY KEY (`professionID`),
  UNIQUE INDEX `profession_UNIQUE` (`profession` ASC));

CREATE TABLE IF NOT EXISTS `var`.`applicant` (
  `applicantID` INT NOT NULL AUTO_INCREMENT,
  `lastName` VARCHAR(10) NULL,
  `firstName` VARCHAR(10) NULL,
  `patronymic` VARCHAR(10) NULL,
  `passport` CHAR(10) NULL,
  `age` DATE NULL,
  `qualificationID` INT NULL,
  `professionID` INT NULL,
  `medicalBook` INT NULL,
  `workRecordNumber` CHAR(10) NULL,
  `documentOnEducation` VARCHAR(15) NULL,
  `phoneNumber` CHAR(11) NULL,
  `mail` VARCHAR(20) NULL,
  `education` VARCHAR(25) NULL,
  `employmentStatus` TINYINT NULL,
  PRIMARY KEY (`applicantID`),
  UNIQUE INDEX `Passport_UNIQUE` (`passport` ASC) ,
  UNIQUE INDEX `PhoneNumber_UNIQUE` (`phoneNumber` ASC) ,
  INDEX `connectQualification_idx` (`qualificationID` ASC) ,
  INDEX `connectProfession_idx` (`professionID` ASC) ,
  CONSTRAINT `connectQualification`
    FOREIGN KEY (`qualificationID`)
    REFERENCES `var`.`typesOfQualification` (`qualificationID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `connectProfession`
    FOREIGN KEY (`professionID`)
    REFERENCES `var`.`typesOfProfession` (`professionID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE IF NOT EXISTS `var`.`employer` (
  `employerID` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(20) NULL,
  `address` VARCHAR(45) NULL,
  `telephone` CHAR(11) NULL,
  PRIMARY KEY (`employerID`));

CREATE TABLE IF NOT EXISTS `var`.`typesOfActivity` (
  `activityID` INT NOT NULL AUTO_INCREMENT,
  `nameOfTheActivity` VARCHAR(45) NULL,
  PRIMARY KEY (`activityID`),
  UNIQUE INDEX `nameOfTheActivity_UNIQUE` (`nameOfTheActivity` ASC) );

CREATE TABLE IF NOT EXISTS `var`.`vacancy` (
  `jobID` INT NOT NULL AUTO_INCREMENT,
  `employersID` INT NULL,
  `typeOfActivityID` INT NULL,
  `requirements` VARCHAR(20) NULL,
  `wages` INT NULL,
  `education` INT NULL,
  `workSchedule` VARCHAR(25) NULL,
  `status` TINYINT NULL,
  `dateOfTheVacancySubmission` DATETIME NULL,
  PRIMARY KEY (`jobID`),
  INDEX `ConnectEmployer_idx` (`employersID` ASC) ,
  INDEX `ConnectTypesOfActivity_idx` (`typeOfActivityID` ASC) ,
  CONSTRAINT `ConnectEmployer`
    FOREIGN KEY (`employersID`)
    REFERENCES `var`.`employer` (`employerID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `ConnectTypesOfActivity`
    FOREIGN KEY (`typeOfActivityID`)
    REFERENCES `var`.`typesOfActivity` (`activityID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE IF NOT EXISTS `var`.`document` (
  `documentID` INT NOT NULL AUTO_INCREMENT,
  `applicantsID` INT NULL,
  `jobID` INT NULL,
  `commissionFees` INT NULL,
  `dateOfConclusionOfTheContract` DATE NULL,
  PRIMARY KEY (`documentID`),
  UNIQUE INDEX `ApplicantsID_UNIQUE` (`applicantsID` ASC) ,
  UNIQUE INDEX `JobID_UNIQUE` (`jobID` ASC) ,
  CONSTRAINT `ConnectApplicantID`
    FOREIGN KEY (`applicantsID`)
    REFERENCES `var`.`applicant` (`applicantID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `ConnectJobID`
    FOREIGN KEY (`jobID`)
    REFERENCES `var`.`vacancy` (`jobID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);