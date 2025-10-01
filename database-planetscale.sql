-- ============================================================================
-- SCRIPT PLANETSCALE - SISTEMA DE CITAS WHATSAPP
-- ============================================================================
-- Optimizado para PlanetScale (MySQL 8.0 compatible)
-- Ejecutar en la consola web de PlanetScale

-- ============================================================================
-- TABLA: Businesses (Negocios)
-- ============================================================================
CREATE TABLE `Businesses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `WorkingHours` varchar(100) NOT NULL,
    `WorkingDays` varchar(100) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================================================
-- TABLA: Services (Servicios)
-- ============================================================================
CREATE TABLE `Services` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `BusinessId` int NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Services_BusinessId` (`BusinessId`),
    CONSTRAINT `FK_Services_Businesses_BusinessId` 
        FOREIGN KEY (`BusinessId`) REFERENCES `Businesses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================================================
-- TABLA: Doctors (Doctores)
-- ============================================================================
CREATE TABLE `Doctors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Specialty` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================================================
-- TABLA: Clients (Clientes)
-- ============================================================================
CREATE TABLE `Clients` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Phone` varchar(50) NOT NULL,
    `Email` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Clients_Phone` (`Phone`),
    UNIQUE KEY `IX_Clients_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================================================
-- TABLA: Appointments (Citas)
-- ============================================================================
CREATE TABLE `Appointments` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Date` datetime NOT NULL,
    `ServiceId` int NOT NULL,
    `ClientId` int NOT NULL,
    `DoctorId` int NOT NULL,
    `BusinessId` int NOT NULL,
    `IsCancelled` tinyint(1) NOT NULL DEFAULT '0',
    PRIMARY KEY (`Id`),
    KEY `IX_Appointments_BusinessId` (`BusinessId`),
    KEY `IX_Appointments_ClientId` (`ClientId`),
    KEY `IX_Appointments_DoctorId` (`DoctorId`),
    KEY `IX_Appointments_ServiceId` (`ServiceId`),
    KEY `IX_Appointments_Date` (`Date`),
    CONSTRAINT `FK_Appointments_Businesses_BusinessId` 
        FOREIGN KEY (`BusinessId`) REFERENCES `Businesses` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Appointments_Clients_ClientId` 
        FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Appointments_Doctors_DoctorId` 
        FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Appointments_Services_ServiceId` 
        FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================================================
-- ÍNDICES ADICIONALES PARA OPTIMIZACIÓN
-- ============================================================================
CREATE INDEX `IX_Appointments_Date_Business` ON `Appointments` (`Date`, `BusinessId`);
CREATE INDEX `IX_Appointments_Doctor_Date` ON `Appointments` (`DoctorId`, `Date`);
CREATE INDEX `IX_Services_Business_Lookup` ON `Services` (`BusinessId`, `Name`);

-- ============================================================================
-- DATOS DE EJEMPLO PARA PRUEBAS
-- ============================================================================

-- Insertar negocios de ejemplo
INSERT INTO `Businesses` (`Name`, `WorkingHours`, `WorkingDays`) VALUES 
('Clínica Dental San José', '08:00-18:00', 'Lunes-Viernes'),
('Centro Médico Familiar', '07:00-20:00', 'Lunes-Sábado'),
('Spa & Wellness Center', '09:00-19:00', 'Martes-Domingo');

-- Insertar servicios de ejemplo
INSERT INTO `Services` (`Name`, `BusinessId`) VALUES 
('Consulta General', 1),
('Limpieza Dental', 1),
('Ortodoncia', 1),
('Consulta Pediatría', 2),
('Consulta Cardiología', 2),
('Exámenes de Laboratorio', 2),
('Masaje Relajante', 3),
('Facial Hidratante', 3),
('Terapia de Relajación', 3);

-- Insertar doctores de ejemplo
INSERT INTO `Doctors` (`Name`, `Specialty`) VALUES 
('Dr. Juan Pérez', 'Odontología General'),
('Dra. María González', 'Ortodoncista'),
('Dr. Carlos Rodríguez', 'Pediatría'),
('Dra. Ana Martínez', 'Cardiología'),
('Lic. Sofia López', 'Terapista de Spa'),
('Lic. Roberto Sánchez', 'Masajista Profesional');

-- Insertar clientes de ejemplo
INSERT INTO `Clients` (`Name`, `Phone`, `Email`) VALUES 
('Cliente Prueba', '+50612345678', 'cliente.prueba@email.com'),
('María Ejemplo', '+50687654321', 'maria.ejemplo@email.com'),
('José Demo', '+50611111111', 'jose.demo@email.com');

-- Insertar citas de ejemplo
INSERT INTO `Appointments` (`Date`, `ServiceId`, `ClientId`, `DoctorId`, `BusinessId`, `IsCancelled`) VALUES 
('2025-10-01 10:00:00', 1, 1, 1, 1, 0),
('2025-10-01 14:00:00', 2, 2, 1, 1, 0),
('2025-10-02 09:00:00', 4, 3, 3, 2, 0),
('2025-10-02 15:30:00', 7, 1, 5, 3, 0);

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

-- Mostrar todas las tablas creadas
SHOW TABLES;

-- Verificar datos insertados
SELECT 'NEGOCIOS' as Tabla, COUNT(*) as Total FROM `Businesses`
UNION ALL
SELECT 'SERVICIOS', COUNT(*) FROM `Services`
UNION ALL
SELECT 'DOCTORES', COUNT(*) FROM `Doctors`
UNION ALL
SELECT 'CLIENTES', COUNT(*) FROM `Clients`
UNION ALL
SELECT 'CITAS', COUNT(*) FROM `Appointments`;

-- ============================================================================
-- SCRIPT COMPLETADO - LISTO PARA PLANETSCALE
-- ============================================================================
