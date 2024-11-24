using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer;

namespace HotelBookingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost;Database=HotelBookingDB;Trusted_Connection=True;TrustServerCertificate=true";

            try
            {
                Console.WriteLine("Försöker ansluta till databasen...");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Anslutning lyckades!");
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                Console.WriteLine($"Error Code: {sqlEx.Number}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Programmet avslutas. Tryck på valfri tangent...");
                Console.ReadKey();
            }
        }
    }
}
