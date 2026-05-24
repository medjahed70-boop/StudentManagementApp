using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// مدير التلاميذ - إدارة بيانات التلاميذ
    /// </summary>
    public class StudentManager
    {
        private DatabaseConnection _dbConnection;

        public StudentManager()
        {
            _dbConnection = new DatabaseConnection();
        }

        /// <summary>
        /// إضافة تلميذ جديد
        /// </summary>
        public bool AddStudent(string firstName, string lastName, string dateOfBirth, 
            string guardianName, string guardianPhone, int classID)
        {
            try
            {
                string query = @"
                    INSERT INTO Students 
                    (FirstName, LastName, DateOfBirth, GuardianName, GuardianPhone, ClassID, IsActive, CreatedDate)
                    VALUES (@FirstName, @LastName, @DateOfBirth, @GuardianName, @GuardianPhone, @ClassID, 1, GETDATE())";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FirstName", firstName),
                    new SqlParameter("@LastName", lastName),
                    new SqlParameter("@DateOfBirth", DateTime.Parse(dateOfBirth)),
                    new SqlParameter("@GuardianName", guardianName),
                    new SqlParameter("@GuardianPhone", guardianPhone),
                    new SqlParameter("@ClassID", classID)
                };

                _dbConnection.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إضافة التلميذ: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// جلب التلاميذ حسب القسم
        /// </summary>
        public DataTable GetStudentsByClass(int classID)
        {
            try
            {
                string query = @"
                    SELECT 
                        StudentID, 
                        FirstName, 
                        LastName, 
                        DateOfBirth, 
                        GuardianName,
                        GuardianPhone,
                        IsActive
                    FROM Students 
                    WHERE ClassID = @ClassID AND IsActive = 1
                    ORDER BY FirstName, LastName";

                return _dbConnection.ExecuteQuery(query,
                    new List<SqlParameter> { new SqlParameter("@ClassID", classID) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب التلاميذ: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// البحث عن تلميذ
        /// </summary>
        public DataTable SearchStudent(string firstName, string lastName)
        {
            try
            {
                string query = @"
                    SELECT 
                        StudentID, 
                        FirstName, 
                        LastName, 
                        DateOfBirth,
                        GuardianName,
                        GuardianPhone,
                        ClassID,
                        IsActive
                    FROM Students 
                    WHERE (FirstName LIKE @FirstName OR LastName LIKE @LastName)
                    AND IsActive = 1";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FirstName", $"%{firstName}%"),
                    new SqlParameter("@LastName", $"%{lastName}%")
                };

                return _dbConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في البحث: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// تحديث بيانات التلميذ
        /// </summary>
        public bool UpdateStudent(int studentID, string firstName, string lastName,
            string dateOfBirth, string guardianName, string guardianPhone)
        {
            try
            {
                string query = @"
                    UPDATE Students 
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        DateOfBirth = @DateOfBirth,
                        GuardianName = @GuardianName,
                        GuardianPhone = @GuardianPhone,
                        UpdatedDate = GETDATE()
                    WHERE StudentID = @StudentID";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@StudentID", studentID),
                    new SqlParameter("@FirstName", firstName),
                    new SqlParameter("@LastName", lastName),
                    new SqlParameter("@DateOfBirth", DateTime.Parse(dateOfBirth)),
                    new SqlParameter("@GuardianName", guardianName),
                    new SqlParameter("@GuardianPhone", guardianPhone)
                };

                _dbConnection.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحديث البيانات: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// شطب التلميذ من القوائم
        /// </summary>
        public bool DelistStudent(int studentID)
        {
            try
            {
                string query = @"
                    UPDATE Students 
                    SET IsActive = 0, UpdatedDate = GETDATE()
                    WHERE StudentID = @StudentID";

                _dbConnection.ExecuteNonQuery(query,
                    new List<SqlParameter> { new SqlParameter("@StudentID", studentID) });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في الشطب: {ex.Message}");
                return false;
            }
        }
    }
}