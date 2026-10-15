using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace HabitTracker;

using System.Configuration;

public class DataAccess(string dbName)
{
    private readonly string _connectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

    public void TestConnection()
    {
        try
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            connection.Close();
            
            Console.WriteLine("Test successful");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void CreateTable()
    {
        const string rawSql = "CREATE TABLE IF NOT EXISTS Habit (" +
                              "name TEXT PRIMARY KEY" +
                              ");" +
                              "CREATE TABLE IF NOT EXISTS occurrence (" +
                              "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                              "timeOfOccurrence TEXT NOT NULL," +
                              "habitName TEXT NOT NULL," +
                              "FOREIGN KEY (habitName) REFERENCES Habit(name) ON DELETE CASCADE);";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using (var command = new SqliteCommand(rawSql, connection))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Table created successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}