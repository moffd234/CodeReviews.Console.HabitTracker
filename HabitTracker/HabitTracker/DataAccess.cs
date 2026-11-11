using System.Globalization;
using Microsoft.Data.Sqlite;

namespace HabitTracker;

using System.Configuration;

public class DataAccess(string dbName)
{
    private readonly string _connectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

    private static string BuildConnectionString(string dbName)
    {
        string connectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

        var builder = new SqliteConnectionStringBuilder(connectionString);

        builder.DataSource = Path.Combine(
            AppContext.BaseDirectory,
            builder.DataSource
        );

        return builder.ToString();
    }

    public void CreateTable()
    {
        const string rawSql = "CREATE TABLE IF NOT EXISTS Habit (" +
                              " name TEXT PRIMARY KEY," +
                              " unit TEXT NOT NULL" +
                              ");" +
                              "CREATE TABLE IF NOT EXISTS occurrence (" +
                              "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                              "timeOfOccurrence TEXT NOT NULL, " +
                              "quantity INTEGER NOT NULL, " +
                              "habitName TEXT NOT NULL, " +
                              "FOREIGN KEY (habitName) REFERENCES Habit(name) ON DELETE CASCADE" +
                              ");";

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

    public bool InsertHabit(Habit habit)
    {
        const string rawSql = "INSERT INTO Habit (name, unit) " +
                              "VALUES (@name, @unit)";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@name", habit.Name);
            command.Parameters.AddWithValue("@unit", habit.Unit);
            command.ExecuteNonQuery();
            return true;
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

        return false;
    }

    public bool InsertOccurrence(string habitName, Occurrence occurrence)
    {
        const string rawSql = "INSERT INTO occurrence (timeOfOccurrence, quantity, habitName) " +
                              "VALUES (@timeOfOccurrence, @quantity, @habitName)";

        try
        {
            if (FindHabitByName(habitName) is null)
            {
                throw new ArgumentException($"Habit with name {habitName} does not exist.");
            }

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@timeOfOccurrence",
                occurrence.TimeOfOccurrence.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@quantity", occurrence.Quantity);
            command.Parameters.AddWithValue("@habitName", habitName);
            command.ExecuteNonQuery();

            return true;
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

        return false;
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
                var unit = reader.GetString(reader.GetOrdinal("unit"));
                var habit = new Habit(name, unit);
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

    private List<Occurrence> FindOccurrencesByHabitName(string habitName)
    {
        const string rawSql = "SELECT id, timeOfOccurrence, quantity FROM occurrence " +
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
                var quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                var storedTime = reader.GetString(reader.GetOrdinal("timeOfOccurrence"));
                var timeOfOccurrence = DateTime.Parse(storedTime, CultureInfo.InvariantCulture);

                output.Add(new Occurrence(id, quantity, timeOfOccurrence));
            }
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }

        return output;
    }

    public List<Habit> GetAllHabits()
    {
        List<Habit> output = [];
        const string rawSql = "SELECT * FROM Habit";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var name = reader.GetString(reader.GetOrdinal("name"));
                var unit = reader.GetString(reader.GetOrdinal("unit"));
                var habit = new Habit(name, unit);
                habit.Occurrences = FindOccurrencesByHabitName(habit.Name);
                output.Add(habit);
            }
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e);
        }

        return output;
    }

    public void EditOccurrence(Occurrence occurrence)
    {
        const string rawSql = "UPDATE occurrence " +
                              "SET timeOfOccurrence = @timeOfOccurrence, " +
                              "quantity = @quantity " +
                              "WHERE id = @id";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@timeOfOccurrence",
                occurrence.TimeOfOccurrence.ToString(
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@quantity", occurrence.Quantity);
            command.Parameters.AddWithValue("@id", occurrence.Id);

            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public bool DeleteOccurrence(int id)
    {
        const string rawSql = "DELETE FROM occurrence " +
                              "WHERE id = @id;";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    public bool DeleteHabit(string name)
    {
        const string rawSql = "DELETE FROM Habit " +
                              "WHERE name = @name;";

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = new SqliteCommand(rawSql, connection);
            command.Parameters.AddWithValue("@name", name);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
}