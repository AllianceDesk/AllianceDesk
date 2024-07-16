using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        var connectionString = "Server=johnivanpuayap\\MSSQLSERVER01;Database=AllianceDeskDb;Trusted_Connection=True;";
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connection successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }
    }
}