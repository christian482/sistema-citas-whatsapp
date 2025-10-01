-- ============================================================================
-- SCRIPT MÍNIMO - SISTEMA DE CITAS WHATSAPP (FreeMySQLHosting)
-- ============================================================================
-- Este script contiene solo lo esencial para FreeMySQLHosting
-- que tiene limitaciones en la cantidad de queries

-- Crear tablas principales
CREATE TABLE `Businesses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `WorkingHours` varchar(100) NOT NULL,
    `WorkingDays` varchar(100) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Services` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `BusinessId` int NOT NULL,
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`BusinessId`) REFERENCES `Businesses` (`Id`)
);

CREATE TABLE `Doctors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Specialty` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Clients` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Phone` varchar(50) NOT NULL,
    `Email` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Appointments` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Date` datetime NOT NULL,
    `ServiceId` int NOT NULL,
    `ClientId` int NOT NULL,
    `DoctorId` int NOT NULL,
    `BusinessId` int NOT NULL,
    `IsCancelled` tinyint(1) NOT NULL DEFAULT '0',
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`BusinessId`) REFERENCES `Businesses` (`Id`),
    FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`Id`),
    FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`Id`),
    FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`)
);

-- Datos de ejemplo básicos
INSERT INTO `Businesses` VALUES (1, 'Clínica Demo', '08:00-18:00', 'Lunes-Viernes');
INSERT INTO `Services` VALUES (1, 'Consulta General', 1);
INSERT INTO `Doctors` VALUES (1, 'Dr. Demo', 'Medicina General');
INSERT INTO `Clients` VALUES (1, 'Cliente Prueba', '+50612345678', 'test@email.com');

-- ============================================================================
-- CONNECTION STRING PARA TU APLICACIÓN:
-- mysql://username:password@hostname:3306/database_name
-- ============================================================================
