USE master;
GO

-- حذف قاعدة البيانات إن وجدت
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'StudentManagementDB')
BEGIN
    ALTER DATABASE StudentManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE StudentManagementDB;
END
GO

-- إنشاء قاعدة البيانات
CREATE DATABASE StudentManagementDB
ON PRIMARY (
    NAME = N'StudentManagementDB',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\StudentManagementDB.mdf',
    SIZE = 10MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10%
)
LOG ON (
    NAME = N'StudentManagementDB_log',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\StudentManagementDB_log.ldf',
    SIZE = 5MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10%
);
GO

USE StudentManagementDB;
GO

-- ============================================
-- 1. جدول الفئات (Classes)
-- ============================================
CREATE TABLE Classes (
    ClassID INT PRIMARY KEY IDENTITY(1,1),
    ClassName NVARCHAR(50) NOT NULL UNIQUE,
    AcademicYear INT NOT NULL,
    Capacity INT DEFAULT 30,
    Description NVARCHAR(255),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 2. جدول التلاميذ (Students)
-- ============================================
CREATE TABLE Students (
    StudentID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NCHAR(1),
    GuardianName NVARCHAR(100) NOT NULL,
    GuardianPhone NVARCHAR(15) NOT NULL,
    GuardianEmail NVARCHAR(100),
    ClassID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassID) REFERENCES Classes(ClassID)
);

-- ============================================
-- 3. جدول الحضور والغياب (Attendance)
-- ============================================
CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- Present, Absent, Excused, Late
    AbsenceHours INT DEFAULT 0,
    RecordedDate DATETIME DEFAULT GETDATE(),
    RecordedBy NVARCHAR(100),
    CONSTRAINT FK_Attendance_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    CONSTRAINT UQ_Attendance UNIQUE (StudentID, AttendanceDate)
);

-- ============================================
-- 4. جدول الإشعارات (Notifications)
-- ============================================
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL, -- إنذار أول، إنذار ثاني، إعذار، شطب
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsSent BIT DEFAULT 0,
    SentDate DATETIME,
    CONSTRAINT FK_Notifications_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- ============================================
-- 5. جدول المخالفات (Violations)
-- ============================================
CREATE TABLE Violations (
    ViolationID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    ViolationType NVARCHAR(100) NOT NULL, -- طرد من الحصة، تأخر مستمر، إلخ
    Description NVARCHAR(MAX),
    ViolationDate DATE NOT NULL,
    TeacherName NVARCHAR(100),
    Severity NVARCHAR(20), -- Minor, Major, Critical
    ActionTaken NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Violations_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- ============================================
-- 6. جدول المستخدمين (Users)
-- ============================================
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Role NVARCHAR(50), -- Administrator, Teacher, Secretary
    IsActive BIT DEFAULT 1,
    LastLogin DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 7. جدول سجل التدقيق (AuditLog)
-- ============================================
CREATE TABLE AuditLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Action NVARCHAR(MAX),
    TableName NVARCHAR(100),
    RecordID INT,
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    ActionDate DATETIME DEFAULT GETDATE(),
    IPAddress NVARCHAR(50),
    CONSTRAINT FK_AuditLog_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ============================================
-- 8. جدول التقار��ر اليومية (DailyReports)
-- ============================================
CREATE TABLE DailyReports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReportDate DATE NOT NULL UNIQUE,
    ClassID INT NOT NULL,
    TotalPresent INT DEFAULT 0,
    TotalAbsent INT DEFAULT 0,
    TotalExcused INT DEFAULT 0,
    CreatedBy NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DailyReports_Classes FOREIGN KEY (ClassID) REFERENCES Classes(ClassID)
);

-- ============================================
-- الفهارس (Indexes)
-- ============================================
CREATE INDEX IDX_Students_ClassID ON Students(ClassID);
CREATE INDEX IDX_Students_Active ON Students(IsActive);
CREATE INDEX IDX_Attendance_StudentID ON Attendance(StudentID);
CREATE INDEX IDX_Attendance_Date ON Attendance(AttendanceDate);
CREATE INDEX IDX_Notifications_StudentID ON Notifications(StudentID);
CREATE INDEX IDX_Violations_StudentID ON Violations(StudentID);
CREATE INDEX IDX_AuditLog_ActionDate ON AuditLog(ActionDate);

-- ============================================
-- إدراج البيانات الافتراضية
-- ============================================

-- إضافة المستخدم الإداري الأول
INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
VALUES 
('admin', 'admin123', 'مدير النظام', 'admin@school.com', 'Administrator', 1),
('teacher1', 'teacher123', 'أحمد محمد', 'teacher1@school.com', 'Teacher', 1),
('secretary', 'secretary123', 'فاطمة علي', 'secretary@school.com', 'Secretary', 1);

-- إضافة الفئات
INSERT INTO Classes (ClassName, AcademicYear, Capacity, Description)
VALUES 
('1ثانوي - أ', 2025, 30, 'الفصل الأول للسنة الأولى ثانوي'),
('1ثانوي - ب', 2025, 30, 'الفصل الثاني للسنة الأولى ثانوي'),
('2ثانوي - أ', 2025, 28, 'الفصل الأول للسنة الثانية ثانوي'),
('3ثانوي - ع.ت', 2025, 25, 'شعبة العلوم التجريبية'),
('3ثانوي - ر.م', 2025, 22, 'شعبة الرياضيات');

-- إضافة تلاميذ نموذجيين
INSERT INTO Students (FirstName, LastName, DateOfBirth, Gender, GuardianName, GuardianPhone, GuardianEmail, ClassID, IsActive)
VALUES 
('محمد', 'علي', '2008-03-15', 'M', 'علي أحمد', '0661234567', 'ali@email.com', 1, 1),
('فاطمة', 'حسن', '2008-05-20', 'F', 'حسن محمد', '0671234567', 'hasan@email.com', 1, 1),
('خديجة', 'محمود', '2008-07-10', 'F', 'محمود علي', '0681234567', 'mahmoud@email.com', 1, 1),
('أحمد', 'صالح', '2008-02-28', 'M', 'صالح حسن', '0661234568', 'saleh@email.com', 2, 1),
('سارة', 'عبدالله', '2008-08-05', 'F', 'عبدالله محمد', '0671234568', 'abdullah@email.com', 2, 1);

-- إضافة بيانات حضور نموذجية
DECLARE @StudentID INT;
DECLARE @DateVar DATE = DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
DECLARE @Counter INT = 0;

WHILE @Counter < 30
BEGIN
    SET @StudentID = 1;
    WHILE @StudentID <= 5
    BEGIN
        INSERT INTO Attendance (StudentID, AttendanceDate, Status, AbsenceHours, RecordedBy)
        VALUES 
        (
            @StudentID,
            @DateVar,
            CASE WHEN RAND() > 0.15 THEN 'Present' ELSE 'Absent' END,
            CASE WHEN RAND() > 0.15 THEN 0 ELSE 3 END,
            'teacher1'
        );
        SET @StudentID = @StudentID + 1;
    END
    SET @DateVar = DATEADD(DAY, 1, @DateVar);
    SET @Counter = @Counter + 1;
END
GO

-- ============================================
-- الإجراءات المخزنة (Stored Procedures)
-- ============================================

-- إجراء للحصول على ملخص غيابات التلميذ
CREATE PROCEDURE sp_GetStudentAbsenceSummary
    @StudentID INT,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @StartDate IS NULL
        SET @StartDate = DATEADD(MONTH, -1, CAST(GETDAY() AS DATE));
    IF @EndDate IS NULL
        SET @EndDate = CAST(GETDATE() AS DATE);
    
    SELECT 
        @StudentID AS StudentID,
        COUNT(CASE WHEN Status = 'Absent' THEN 1 END) AS TotalAbsentDays,
        SUM(CASE WHEN Status = 'Absent' THEN AbsenceHours ELSE 0 END) AS TotalAbsenceHours,
        COUNT(CASE WHEN Status = 'Present' THEN 1 END) AS TotalPresentDays,
        COUNT(CASE WHEN Status = 'Excused' THEN 1 END) AS ExcusedDays,
        COUNT(CASE WHEN Status = 'Late' THEN 1 END) AS LateDays
    FROM Attendance
    WHERE StudentID = @StudentID
        AND AttendanceDate BETWEEN @StartDate AND @EndDate;
END
GO

-- إجراء للحصول على التلاميذ الأكثر غياباً
CREATE PROCEDURE sp_GetTopAbsentStudents
    @ClassID INT = NULL,
    @TopCount INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopCount)
        s.StudentID,
        s.FirstName + ' ' + s.LastName AS StudentName,
        COUNT(a.AttendanceID) AS AbsentDays,
        SUM(a.AbsenceHours) AS TotalAbsenceHours,
        s.GuardianEmail,
        s.GuardianPhone
    FROM Students s
    LEFT JOIN Attendance a ON s.StudentID = a.StudentID AND a.Status = 'Absent'
    WHERE s.IsActive = 1
        AND (@ClassID IS NULL OR s.ClassID = @ClassID)
    GROUP BY s.StudentID, s.FirstName, s.LastName, s.GuardianEmail, s.GuardianPhone
    ORDER BY AbsentDays DESC;
END
GO

-- إجراء للحصول على التقرير اليومي للقسم
CREATE PROCEDURE sp_GetDailyClassReport
    @ClassID INT,
    @ReportDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.StudentID,
        s.FirstName + ' ' + s.LastName AS StudentName,
        c.ClassName,
        ISNULL(a.Status, 'Absent') AS AttendanceStatus,
        ISNULL(a.AbsenceHours, 0) AS AbsenceHours,
        s.GuardianName,
        s.GuardianPhone
    FROM Students s
    INNER JOIN Classes c ON s.ClassID = c.ClassID
    LEFT JOIN Attendance a ON s.StudentID = a.StudentID 
        AND a.AttendanceDate = @ReportDate
    WHERE s.ClassID = @ClassID AND s.IsActive = 1
    ORDER BY s.FirstName, s.LastName;
END
GO

PRINT N'✅ تم إنشاء قاعدة البيانات بنجاح!';
PRINT N'✅ تم إنشاء جميع الجداول والفهارس والإجراءات المخزنة';
PRINT N'✅ تم إدراج البيانات الافتراضية';
