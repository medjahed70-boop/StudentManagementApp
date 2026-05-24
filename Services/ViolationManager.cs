using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// مدير المخالفات والسلوك
    /// </summary>
    public class ViolationManager
    {
        private DatabaseConnection _dbConnection;

        public ViolationManager()
        {
            _dbConnection = new DatabaseConnection();
        }

        /// <summary>
        /// تسجيل مخالفة جديدة
        /// </summary>
        public bool RecordViolation(int studentID, string violationType, 
            string description, string teacherName, string severity)
        {
            try
            {
                string query = @"
                    INSERT INTO Violations 
                    (StudentID, ViolationType, Description, ViolationDate, TeacherName, Severity, CreatedDate)
                    VALUES (@StudentID, @ViolationType, @Description, GETDATE(), @TeacherName, @Severity, GETDATE())";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@StudentID", studentID),
                    new SqlParameter("@ViolationType", violationType),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@TeacherName", teacherName),
                    new SqlParameter("@Severity", severity)
                };

                return _dbConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل المخالفة: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// الحصول على مخالفات التلميذ
        /// </summary>
        public DataTable GetStudentViolations(int studentID)
        {
            try
            {
                string query = @"
                    SELECT 
                        ViolationID,
                        ViolationType,
                        Description,
                        ViolationDate,
                        TeacherName,
                        Severity,
                        ActionTaken
                    FROM Violations
                    WHERE StudentID = @StudentID
                    ORDER BY ViolationDate DESC";

                return _dbConnection.ExecuteQuery(query,
                    new List<SqlParameter> { new SqlParameter("@StudentID", studentID) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب المخالفات: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// الحصول على إحصائيات المخالفات حسب الشهر
        /// </summary>
        public DataTable GetMonthlyViolationStatistics(int month, int year)
        {
            try
            {
                string query = @"
                    SELECT 
                        ViolationType,
                        COUNT(*) AS Count,
                        Severity
                    FROM Violations
                    WHERE MONTH(ViolationDate) = @Month
                        AND YEAR(ViolationDate) = @Year
                    GROUP BY ViolationType, Severity
                    ORDER BY Count DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Month", month),
                    new SqlParameter("@Year", year)
                };

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب الإحصائيات: {ex.Message}");
                return null;
            }
        }
    }
}