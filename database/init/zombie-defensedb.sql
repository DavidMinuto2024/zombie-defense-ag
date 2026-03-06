IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ZombieTypes' AND xtype='U')
CREATE TABLE ZombieTypes (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Type         NVARCHAR(100) NOT NULL UNIQUE,
    ShootingTime INT NOT NULL CHECK (ShootingTime > 0),
    BulletsNeeded INT NOT NULL CHECK (BulletsNeeded > 0),
    Score        INT NOT NULL CHECK (Score > 0),
    ThreatLevel  NVARCHAR(20) NOT NULL
                 CHECK (ThreatLevel IN ('HIGH', 'MEDIUM', 'LOW')),
    CreatedAt    DATETIME2 DEFAULT GETDATE()
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Simulations' AND xtype='U')
CREATE TABLE Simulations (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Date             DATETIME2 DEFAULT GETDATE(),
    TimeAvailable    INT NOT NULL CHECK (TimeAvailable > 0),
    BulletsAvailable INT NOT NULL CHECK (BulletsAvailable > 0),
    TotalScore       INT NOT NULL DEFAULT 0,
    CreatedAt        DATETIME2 DEFAULT GETDATE()
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EliminatedZombies' AND xtype='U')
CREATE TABLE EliminatedZombies (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    ZombieTypeId INT NOT NULL REFERENCES ZombieTypes(Id),
    SimulationId INT NOT NULL REFERENCES Simulations(Id) ON DELETE CASCADE,
    PointsEarned INT NOT NULL CHECK (PointsEarned > 0),
    Timestamp    DATETIME2 DEFAULT GETDATE()
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLog' AND xtype='U')
CREATE TABLE AuditLog (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50)  NOT NULL,
    Operation NVARCHAR(10)  NOT NULL CHECK (Operation IN ('INSERT','UPDATE','DELETE')),
    OldData   NVARCHAR(MAX),
    NewData   NVARCHAR(MAX),
    DbUser    NVARCHAR(100) DEFAULT SYSTEM_USER,
    AppUser   NVARCHAR(100),
    Timestamp DATETIME2 DEFAULT GETDATE()
);
GO



CREATE OR ALTER TRIGGER trg_Simulations_Audit
ON Simulations
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        INSERT INTO AuditLog (TableName, Operation, OldData, NewData)
        SELECT
            'Simulations',
            CASE
                WHEN EXISTS (SELECT 1 FROM deleted d WHERE d.Id = i.Id)
                THEN 'UPDATE'
                ELSE 'INSERT'
            END,
            (SELECT * FROM deleted d WHERE d.Id = i.Id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            (SELECT * FROM inserted ins WHERE ins.Id = i.Id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
        FROM inserted i;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO AuditLog (TableName, Operation, OldData, NewData)
        SELECT 'Simulations', 'DELETE',
            (SELECT * FROM deleted d WHERE d.Id = del.Id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            NULL
        FROM deleted del;
    END
END;
GO



IF NOT EXISTS (SELECT 1 FROM ZombieTypes)
BEGIN
    INSERT INTO ZombieTypes (Type, ShootingTime, BulletsNeeded, Score, ThreatLevel)
    VALUES
        ('Walker',    5,  3, 10, 'LOW'),
        ('Runner',    3,  5, 25, 'MEDIUM'),
        ('Screamer',  4,  2, 15, 'MEDIUM'),
        ('Brute',     8,  6, 40, 'HIGH'),
        ('Tank',     10,  8, 50, 'HIGH');
END
GO

-- ============================================================
-- VERIFICACION
-- ============================================================
SELECT 'ZombieTypes' AS Tabla, COUNT(*) AS Registros FROM ZombieTypes
UNION ALL
SELECT 'Simulations',       COUNT(*) FROM Simulations
UNION ALL
SELECT 'EliminatedZombies', COUNT(*) FROM EliminatedZombies
UNION ALL
SELECT 'AuditLog',          COUNT(*) FROM AuditLog;
GO
