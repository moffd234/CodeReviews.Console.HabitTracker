using System.Globalization;
using Microsoft.Data.Sqlite;

namespace HabitTracker;

using System.Configuration;

public class DataAccess(string dbName)
{
    private readonly string _connectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

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
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void InsertHabit(Habit habit)
    {
        const string rawSql = "INSERT INTO Habit (name) " +
                              "VALUES (@name)";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@name", habit.Name);
            command.ExecuteNonQuery();
        }
        catch (SqliteException e) when (e.SqliteErrorCode == 19)
        {
            Console.WriteLine("A habit with that name already exists.");
        }
        catch (SqliteException e)
        {
            Console.WriteLine("Issue adding habit. See below");
            Console.WriteLine(e.Message);
        }
    }

    public void InsertOccurrence(string habitName, Occurrence occurrence)
    {
        const string rawSql = "INSERT INTO occurrence (timeOfOccurrence, habitName) " +
                              "VALUES (@timeOfOccurrence, @habitName)";

        try
        {
            if (FindHabitByName(habitName) is null)
            {
                throw new ArgumentException($"Habit with name {habitName} does not exist.");
            }

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@timeOfOccurrence", occurrence.TimeOfOccurrence);
            command.Parameters.AddWithValue("@habitName", habitName);
            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            Console.WriteLine("Issue adding habit. See below");
            Console.WriteLine(e.Message);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public Habit? FindHabitByName(string habitName)
    {
        const string rawSql = "SELECT * FROM Habit WHERE name = @name";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@name", habitName);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var name = reader.GetString(reader.GetOrdinal("name"));
                var habit = new Habit(name);
                reader.Close();
                habit.Occurrences = FindOccurrencesByHabitName(name);
                
                return habit;
            }
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }

    public List<Occurrence> FindOccurrencesByHabitName(string habitName)
    {
        const string rawSql = "SELECT id, timeOfOccurrence FROM occurrence " +
                              "WHERE habitName = @habitName " +
                              "ORDER BY timeOfOccurrence";
        List<Occurrence> output = [];

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@habitName", habitName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32(reader.GetOrdinal("id"));
                var storedTime = reader.GetString(reader.GetOrdinal("timeOfOccurrence"));
                var timeOfOccurrence = DateTime.Parse(storedTime, CultureInfo.InvariantCulture);
                output.Add(new Occurrence(id, timeOfOccurrence));
            }
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }

        return output;
    }
}