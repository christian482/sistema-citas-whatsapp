-- ============================================================================
-- SCRIPT DE CREACIÓN DE BASE DE DATOS - SISTEMA DE CITAS WHATSAPP
-- ============================================================================
-- Fecha: 30 de septiembre de 2025
-- Autor: Sistema automatizado
-- Descripción: Script completo para crear la base de datos MySQL del sistema
--              de citas con integración WhatsApp
-- ============================================================================

-- Crear la base de datos (opcional, FreeMySQLHosting ya la crea)
-- CREATE DATABASE IF NOT EXISTS sistema_citas_whatsapp;
-- USE sistema_citas_whatsapp;

-- ============================================================================
-- TABLA: Businesses (Negocios)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Businesses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `WorkingHours` varchar(100) NOT NULL,
    `WorkingDays` varchar(100) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABLA: Services (Servicios)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Services` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `BusinessId` int NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Services_BusinessId` (`BusinessId`),
    CONSTRAINT `FK_Services_Businesses_BusinessId` 
        FOREIGN KEY (`BusinessId`) REFERENCES `Businesses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABLA: Doctors (Doctores)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Doctors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Specialty` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABLA: Clients (Clientes)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Clients` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) NOT NULL,
    `Phone` varchar(50) NOT NULL,
    `Email` varchar(255) NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Clients_Phone` (`Phone`),
    UNIQUE KEY `IX_Clients_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABLA: Appointments (Citas)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Appointments` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- DATOS DE EJEMPLO PARA PRUEBAS
-- ============================================================================

-- Insertar negocio de ejemplo
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
-- ÍNDICES ADICIONALES PARA OPTIMIZACIÓN
-- ============================================================================

-- Índices para mejorar rendimiento en consultas frecuentes
CREATE INDEX IF NOT EXISTS `IX_Appointments_Date_Business` ON `Appointments` (`Date`, `BusinessId`);
CREATE INDEX IF NOT EXISTS `IX_Appointments_Doctor_Date` ON `Appointments` (`DoctorId`, `Date`);
CREATE INDEX IF NOT EXISTS `IX_Clients_Phone_Lookup` ON `Clients` (`Phone`);
CREATE INDEX IF NOT EXISTS `IX_Services_Business_Lookup` ON `Services` (`BusinessId`, `Name`);

-- ============================================================================
-- VISTAS ÚTILES PARA EL SISTEMA
-- ============================================================================

-- Vista para citas completas con información detallada
CREATE OR REPLACE VIEW `AppointmentDetails` AS
SELECT 
    a.Id as AppointmentId,
    a.Date as AppointmentDate,
    a.IsCancelled,
    c.Name as ClientName,
    c.Phone as ClientPhone,
    c.Email as ClientEmail,
    d.Name as DoctorName,
    d.Specialty as DoctorSpecialty,
    s.Name as ServiceName,
    b.Name as BusinessName,
    b.WorkingHours,
    b.WorkingDays
FROM `Appointments` a
INNER JOIN `Clients` c ON a.ClientId = c.Id
INNER JOIN `Doctors` d ON a.DoctorId = d.Id
INNER JOIN `Services` s ON a.ServiceId = s.Id
INNER JOIN `Businesses` b ON a.BusinessId = b.Id;

-- Vista para estadísticas del negocio
CREATE OR REPLACE VIEW `BusinessStats` AS
SELECT 
    b.Id as BusinessId,
    b.Name as BusinessName,
    COUNT(DISTINCT s.Id) as TotalServices,
    COUNT(DISTINCT a.Id) as TotalAppointments,
    COUNT(DISTINCT CASE WHEN a.IsCancelled = 0 THEN a.Id END) as ActiveAppointments,
    COUNT(DISTINCT CASE WHEN a.IsCancelled = 1 THEN a.Id END) as CancelledAppointments,
    COUNT(DISTINCT c.Id) as UniqueClients
FROM `Businesses` b
LEFT JOIN `Services` s ON b.Id = s.BusinessId
LEFT JOIN `Appointments` a ON b.Id = a.BusinessId
LEFT JOIN `Clients` c ON a.ClientId = c.Id
GROUP BY b.Id, b.Name;

-- ============================================================================
-- PROCEDIMIENTOS ALMACENADOS ÚTILES
-- ============================================================================

-- Procedimiento para obtener citas disponibles por doctor y fecha
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS GetAvailableSlots(
    IN doctorId INT,
    IN appointmentDate DATE
)
BEGIN
    SELECT 
        d.Name as DoctorName,
        d.Specialty,
        appointmentDate as SelectedDate,
        COUNT(a.Id) as BookedSlots
    FROM `Doctors` d
    LEFT JOIN `Appointments` a ON d.Id = a.DoctorId 
        AND DATE(a.Date) = appointmentDate 
        AND a.IsCancelled = 0
    WHERE d.Id = doctorId
    GROUP BY d.Id, d.Name, d.Specialty;
END //
DELIMITER ;

-- Procedimiento para cancelar cita
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS CancelAppointment(
    IN appointmentId INT
)
BEGIN
    UPDATE `Appointments` 
    SET IsCancelled = 1 
    WHERE Id = appointmentId;
    
    SELECT 
        'Cita cancelada exitosamente' as Message,
        appointmentId as AppointmentId,
        NOW() as CancelledAt;
END //
DELIMITER ;

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

-- Mostrar todas las tablas creadas
SHOW TABLES;

-- Mostrar estructura de tablas principales
DESCRIBE `Businesses`;
DESCRIBE `Services`;
DESCRIBE `Doctors`;
DESCRIBE `Clients`;
DESCRIBE `Appointments`;

-- Mostrar datos de ejemplo insertados
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
-- SCRIPT COMPLETADO EXITOSAMENTE
-- ============================================================================
-- Este script crea toda la estructura necesaria para el Sistema de Citas
-- con integración WhatsApp, incluyendo:
-- - 5 tablas principales con relaciones
-- - Índices para optimización
-- - Datos de ejemplo para pruebas
-- - Vistas útiles para consultas
-- - Procedimientos almacenados
-- - Verificaciones finales
-- ============================================================================
