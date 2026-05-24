using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// خدمة إنشاء التقارير بصيغة PDF
    /// </summary>
    public class ReportGeneratorService
    {
        public string SchoolName { get; set; } = "ثانوية سيدي عيساة";
        public string Province { get; set; } = "الجزائر غرب";
        public string Ministry { get; set; } = "وزارة التربية الوطنية";

        /// <summary>
        /// إنشاء رأس التقرير الرسمي
        /// </summary>
        public string GenerateReportHeader()
        {
            return $@"
╔════════════════════════════════════════════════════════════╗
║         الجمهورية الجزائرية الديمقراطية الشعبية          ║
║            {Ministry.PadRight(50)}║
║         {Province.PadRight(50)}║
║         مديرية التربية ولاية {Province.PadRight(37)}║
║                                                            ║
║           {SchoolName.PadRight(50)}║
╚════════════════════════════════════════════════════════════╝
";
        }

        /// <summary>
        /// إنشاء تقرير الحضور اليومي
        /// </summary>
        public string GenerateDailyAttendanceReport(DataTable attendanceData, DateTime reportDate)
        {
            string report = GenerateReportHeader();
            report += $@"
                         التقرير اليومي للحضور
                    تاريخ: {reportDate:yyyy-MM-dd}

╔════════════════════════════════════════════════════════════════════╗
║ الرقم │ اسم التلميذ              │ الحالة    │ ساعات الغياب       ║
╠════════════════════════════════════════════════════════════════════╣
";

            if (attendanceData != null && attendanceData.Rows.Count > 0)
            {
                int rowNum = 1;
                foreach (DataRow row in attendanceData.Rows)
                {
                    string studentName = row["StudentName"].ToString().PadRight(20);
                    string status = row["AttendanceStatus"].ToString().PadRight(8);
                    int absenceHours = (int)row["AbsenceHours"];

                    report += $"║ {rowNum,4} │ {studentName} │ {status} │ {absenceHours,15} ║\n";
                    rowNum++;
                }
            }

            report += $@"╚════════════════════════════════════════════════════════════════════╝

تاريخ الطباعة: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

المدير:_______________     المراقب:_______________     الناظر:_______________
";

            return report;
        }

        /// <summary>
        /// إنشاء تقرير الغيابات الشهري
        /// </summary>
        public string GenerateMonthlyAbsenceReport(DataTable absenceData, int month, int year)
        {
            string report = GenerateReportHeader();
            report += $@"
                       كشف الغيابات الشهري
                 الشهر: {month}/{year}

╔════════════════════════════════════════════════════════════════════╗
║ الرقم │ اسم التلميذ              │ عدد الأيام │ عدد الساعات        ║
╠════════════════════════════════════════════════════════════════════╣
";

            if (absenceData != null && absenceData.Rows.Count > 0)
            {
                int rowNum = 1;
                foreach (DataRow row in absenceData.Rows)
                {
                    string studentName = row["StudentName"].ToString().PadRight(20);
                    int absentDays = (int)row["AbsentDays"];
                    int absentHours = (int)row["TotalAbsenceHours"];

                    report += $"║ {rowNum,4} │ {studentName} │ {absentDays,9} │ {absentHours,15} ║\n";
                    rowNum++;
                }
            }

            report += $@"╚════════════════════════════════════════════════════════════════════╝

تاريخ الطباعة: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
";

            return report;
        }

        /// <summary>
        /// حفظ التقرير في ملف نصي
        /// </summary>
        public bool SaveReportToFile(string reportContent, string fileName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", fileName);
                
                // إنشاء مجلد Reports إذا لم يكن موجوداً
                string reportDirectory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(reportDirectory))
                {
                    Directory.CreateDirectory(reportDirectory);
                }

                File.WriteAllText(filePath, reportContent, System.Text.Encoding.UTF8);
                Console.WriteLine($"✅ تم حفظ التقرير: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في حفظ التقرير: {ex.Message}");
                return false;
            }
        }
    }
}