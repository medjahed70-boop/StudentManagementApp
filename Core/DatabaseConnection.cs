using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace StudentManagementApp.Core
{
    /// <summary>
    /// فئة الاتصال بقاعدة البيانات SQL Server
    /// </summary>
    public class DatabaseConnection
    {
        private string _connectionString;

        public DatabaseConnection()
        {
            // الحصول على سلسلة الاتصال من App.config
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        /// <summary>
        /// اختبار الاتصال بقاعدة البيانات
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("✅ الاتصال بقاعدة البيانات نجح!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في الاتصال: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// تنفيذ استعلام SELECT
        /// </summary>
        public DataTable ExecuteQuery(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تنفيذ الاستعلام: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// تنفيذ استعلام INSERT, UPDATE, DELETE
        /// </summary>
        public bool ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تنفيذ العملية: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// تنفيذ إجراء مخزن
        /// </summary>
        public DataTable ExecuteStoredProcedure(string procedureName, List<SqlParameter> parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تنفيذ الإجراء المخزن: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// الحصول على آخر رقم معرف تم إدراجه
        /// </summary>
        public int GetLastInsertedID()
        {
            try
            {
                string query = "SELECT @@IDENTITY AS LastID";
                DataTable result = ExecuteQuery(query);
                if (result != null && result.Rows.Count > 0)
                {
                    return (int)result.Rows[0]["LastID"];
                }
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في الحصول على آخر معرف: {ex.Message}");
                return -1;
            }
        }
    }
}