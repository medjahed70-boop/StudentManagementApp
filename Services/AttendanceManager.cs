using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// مدير الحضور والغياب - مع إنذارات آلية
    /// </summary>
    public class AttendanceManager
    {
        private DatabaseConnection _dbConnection;

        public AttendanceManager()
        {
            _dbConnection = new DatabaseConnection();
        }

        /// <summary>
        /// تسجيل الحضور أو الغياب
        /// </summary>
        public bool RecordAttendance(int studentID, DateTime attendanceDate, 
            string status, int absenceHours = 0)
        {
            try
            {
                string query = @"
                    INSERT INTO Attendance 
                    (StudentID, AttendanceDate, Status, AbsenceHours, RecordedDate)
                    VALUES (@StudentID, @AttendanceDate, @Status, @AbsenceHours, GETDATE())";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@StudentID", studentID),
                    new SqlParameter("@AttendanceDate", attendanceDate),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@AbsenceHours", absenceHours)
                };

                _dbConnection.ExecuteNonQuery(query, parameters);
                
                // فحص الإنذارات
                CheckAndCreateWarnings(studentID);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل الحضور: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// فحص الإنذارات المستحقة
        /// </summary>
        private void CheckAndCreateWarnings(int studentID)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) as AbsentCount
                    FROM Attendance
                    WHERE StudentID = @StudentID AND Status = 'Absent'";

                DataTable result = _dbConnection.ExecuteQuery(query,
                    new List<SqlParameter> { new SqlParameter("@StudentID", studentID) });

                if (result != null && result.Rows.Count > 0)
                {
                    int absentDays = (int)result.Rows[0]["AbsentCount"];

                    // إنذار أول (3 أيام)
                    if (absentDays == 3)
                    {
                        CreateNotification(studentID, "إنذار أول", "غياب 3 أيام");
                    }
                    // إنذار ثاني (10 أيام)
                    else if (absentDays == 10)
                    {
                        CreateNotification(studentID, "إنذار ثاني", "غياب 10 أيام");
                    }
                    // إعذار (17 يوم)
                    else if (absentDays == 17)
                    {
                        CreateNotification(studentID, "إعذار", "غياب 17 يوم");
                    }
                    // شطب (32 يوم)
                    else if (absentDays == 32)
                    {
                        CreateNotification(studentID, "شطب من القوائم", "غياب 32 يوم");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في فحص الإنذارات: {ex.Message}");
            }
        }

        /// <summary>
        /// إنشاء إشعار
        /// </summary>
        private void CreateNotification(int studentID, string notificationType, string description)
        {
            try
            {
                string query = @"
                    INSERT INTO Notifications 
                    (StudentID, NotificationType, Description, CreatedDate, IsSent)
                    VALUES (@StudentID, @NotificationType, @Description, GETDATE(), 0)";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@StudentID", studentID),
                    new SqlParameter("@NotificationType", notificationType),
                    new SqlParameter("@Description", description)
                };

                _dbConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إنشاء الإشعار: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب ملخص غيابات التلميذ
        /// </summary>
        public DataTable GetStudentAbsenceSummary(int studentID)
        {
            try
            {
                string query = @"
                    SELECT 
                        StudentID,
                        COUNT(CASE WHEN Status = 'Absent' THEN 1 END) as TotalAbsentDays,
                        SUM(CASE WHEN Status = 'Absent' THEN AbsenceHours ELSE 0 END) as TotalAbsenceHours,
                        COUNT(CASE WHEN Status = 'Present' THEN 1 END) as TotalPresentDays,
                        COUNT(CASE WHEN Status = 'Excused' THEN 1 END) as ExcusedDays
                    FROM Attendance
                    WHERE StudentID = @StudentID
                    GROUP BY StudentID";

                return _dbConnection.ExecuteQuery(query,
                    new List<SqlParameter> { new SqlParameter("@StudentID", studentID) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب الملخص: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// التقرير اليومي للقسم
        /// </summary>
        public DataTable GetDailyClassReport(int classID, DateTime reportDate)
        {
            try
            {
                string query = @"
                    SELECT 
                        s.StudentID,
                        s.FirstName + ' ' + s.LastName as StudentName,
                        ISNULL(a.Status, 'غياب') as AttendanceStatus,
                        ISNULL(a.AbsenceHours, 0) as AbsenceHours
                    FROM Students s
                    LEFT JOIN Attendance a ON s.StudentID = a.StudentID 
                        AND a.AttendanceDate = @ReportDate
                    WHERE s.ClassID = @ClassID AND s.IsActive = 1
                    ORDER BY s.FirstName";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ClassID", classID),
                    new SqlParameter("@ReportDate", reportDate)
                };

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب التقرير اليومي: {ex.Message}");
                return null;
            }
        }
    }
}