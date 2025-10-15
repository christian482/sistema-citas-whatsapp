-- Agregar campo Title a la tabla Appointments
ALTER TABLE Appointments ADD COLUMN Title VARCHAR(255) NOT NULL DEFAULT '';
