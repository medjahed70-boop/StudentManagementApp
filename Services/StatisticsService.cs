using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// خدمة الإحصائيات المتقدمة
    /// </summary>
    public class StatisticsService
    {
        private DatabaseConnection _dbConnection;

        public StatisticsService()
        {
            _dbConnection = new DatabaseConnection();
        }

        /// <summary>
        /// الحصول على إحصائيات الحضور الشهرية
        /// </summary>
        public DataTable GetMonthlyAttendanceStatistics(int month, int year, int classID = -1)
        {
            try
            {
                string query = $@"
                    SELECT 
                        c.ClassName,
                        COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS TotalPresent,
                        COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS TotalAbsent,
                        COUNT(CASE WHEN a.Status = 'Excused' THEN 1 END) AS TotalExcused,
                        COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS TotalLate,
                        ROUND(
                            (CAST(COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS FLOAT) / 
                             COUNT(*)) * 100, 2
                        ) AS AttendancePercentage
                    FROM Classes c
                    LEFT JOIN Students s ON c.ClassID = s.ClassID
                    LEFT JOIN Attendance a ON s.StudentID = a.StudentID
                        AND MONTH(a.AttendanceDate) = @Month
                        AND YEAR(a.AttendanceDate) = @Year
                    WHERE s.IsActive = 1
                        {(classID != -1 ? "AND c.ClassID = @ClassID" : "")}
                    GROUP BY c.ClassID, c.ClassName
                    ORDER BY c.ClassName";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Month", month),
                    new SqlParameter("@Year", year)
                };

                if (classID != -1)
                {
                    parameters.Add(new SqlParameter("@ClassID", classID));
                }

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب الإحصائيات: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// التلاميذ الأكثر غياباً
        /// </summary>
        public DataTable GetTopAbsentStudents(int topCount = 10, int classID = -1)
        {
            try
            {
                string query = $@"
                    SELECT TOP {topCount}
                        s.StudentID,
                        s.FirstName + ' ' + s.LastName AS StudentName,
                        COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS AbsentDays,
                        SUM(CASE WHEN a.Status = 'Absent' THEN a.AbsenceHours ELSE 0 END) AS TotalAbsenceHours,
                        s.GuardianEmail,
                        s.GuardianPhone
                    FROM Students s
                    LEFT JOIN Attendance a ON s.StudentID = a.StudentID
                    WHERE s.IsActive = 1
                        {(classID != -1 ? "AND s.ClassID = @ClassID" : "")}
                    GROUP BY s.StudentID, s.FirstName, s.LastName, s.GuardianEmail, s.GuardianPhone
                    ORDER BY AbsentDays DESC";

                List<SqlParameter> parameters = new List<SqlParameter>();
                if (classID != -1)
                {
                    parameters.Add(new SqlParameter("@ClassID", classID));
                }

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب التلاميذ الأكثر غياباً: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// إحصائيات المخالفات
        /// </summary>
        public DataTable GetViolationStatistics(int month, int year, int classID = -1)
        {
            try
            {
                string query = $@"
                    SELECT 
                        v.ViolationType,
                        COUNT(*) AS ViolationCount,
                        COUNT(DISTINCT v.StudentID) AS AffectedStudents,
                        v.Severity
                    FROM Violations v
                    INNER JOIN Students s ON v.StudentID = s.StudentID
                    WHERE MONTH(v.ViolationDate) = @Month
                        AND YEAR(v.ViolationDate) = @Year
                        {(classID != -1 ? "AND s.ClassID = @ClassID" : "")}
                    GROUP BY v.ViolationType, v.Severity
                    ORDER BY ViolationCount DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Month", month),
                    new SqlParameter("@Year", year)
                };

                if (classID != -1)
                {
                    parameters.Add(new SqlParameter("@ClassID", classID));
                }

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب إحصائيات المخالفات: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// التقرير الشامل الشهري
        /// </summary>
        public string GenerateComprehensiveReport(int month, int year, int classID = -1)
        {
            try
            {
                string report = $@"
╔════════════════════════════════════════════════════════════════════════════╗
║                    التقرير الشامل الشهري                                  ║
║                      الشهر: {month}/{year}                                     ║
╚════════════════════════════════════════════════════════════════════════════╝

═══════════════════════════════════════════════════════════════════════════════
1️⃣  إحصائيات الحضور والغياب
═══════════════════════════════════════════════════════════════════════════════
";

                DataTable attendanceStats = GetMonthlyAttendanceStatistics(month, year, classID);
                if (attendanceStats != null && attendanceStats.Rows.Count > 0)
                {
                    foreach (DataRow row in attendanceStats.Rows)
                    {
                        report += $@"
{row["ClassName"]}:
  • الحاضرون: {row["TotalPresent"]}
  • الغائبون: {row["TotalAbsent"]}
  • المعذرون: {row["TotalExcused"]}
  • المتأخرون: {row["TotalLate"]}
  • نسبة الحضور: {row["AttendancePercentage"]}%
";
                    }
                }

                report += $@"
═══════════════════════════════════════════════════════════════════════════════
2️⃣  التلاميذ الأكثر غياباً
═══════════════════════════════════════════════════════════════════════════════
";

                DataTable topAbsent = GetTopAbsentStudents(10, classID);
                if (topAbsent != null && topAbsent.Rows.Count > 0)
                {
                    int rank = 1;
                    foreach (DataRow row in topAbsent.Rows)
                    {
                        report += $@"{rank}. {row["StudentName"]} - أيام غياب: {row["AbsentDays"]}, ساعات: {row["TotalAbsenceHours"]}\n";
                        rank++;
                    }
                }

                report += $@"
═══════════════════════════════════════════════════════════════════════════════
3️⃣  إحصائيات المخالفات
═══════════════════════════════════════════════════════════════════════════════
";

                DataTable violations = GetViolationStatistics(month, year, classID);
                if (violations != null && violations.Rows.Count > 0)
                {
                    foreach (DataRow row in violations.Rows)
                    {
                        report += $@"{row["ViolationType"]} ({row["Severity"]}): {row["ViolationCount"]} حالات\n";
                    }
                }

                report += $@"
═══════════════════════════════════════════════════════════════════════════════

تاريخ الطباعة: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
تم إنشاء هذا التقرير تلقائياً من نظام إدارة التلاميذ

═══════════════════════════════════════════════════════════════════════════════
";

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إنشاء التقرير الشامل: {ex.Message}");
                return null;
            }
        }
    }
}