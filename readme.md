# Habit Tracker

A console application for recording and reviewing quantity-based habits. Create a habit with a custom unit, log occurrences with a date and quantity, and keep all data in a local SQLite database.

This project was built as a solution to the [CSharp Academy Habit Logger project](https://thecsharpacademy.com/project/12/habit-logger).

## Features

- Create custom habits and units of measurement
- Add, view, update, and delete occurrences
- Delete habits and their associated occurrences
- Display the quantity total for a habit
- Automatically create the SQLite database and tables
- Validate console input and keep the application running after recoverable errors
- Enter `exit` at a text prompt to close the application

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A terminal or IDE capable of running C# console applications

The required SQLite and configuration packages are included in the project and restored automatically by `dotnet`.

## Getting Started

From the repository root, run:

```powershell
dotnet restore
cd HabitTracker
dotnet run
```

Alternatively, run the project directly from the repository root:

```powershell
dotnet run --project .\HabitTracker\HabitTracker.csproj
```

On first run, the application creates `HabitTracker.db` and the required tables automatically.

## Using the Application

At startup, the main menu provides two options:

- `V` - view habits and their occurrences
- `M` - manage habits

When managing habits, you can:

- `A` - add a habit
- `U` - update an existing habit or one of its occurrences
- `D` - delete a habit

A new occurrence asks whether it should use the current date and time. To enter a custom date, use:

```text
HH-mm MM-dd-yyyy
```

For example, `14-30 11-18-2026` represents 2:30 PM on November 18, 2026. Occurrence quantities must be positive integers.

## Database

The project uses `Microsoft.Data.Sqlite` and ADO.NET directly rather than an ORM such as Entity Framework or Dapper.

The `HabitTracker.db` database contains two tables:

| Table | Purpose |
|---|---|
| `Habit` | Stores the habit name and unit of measurement |
| `occurrence` | Stores an occurrence ID, date, quantity, and parent habit name |

Deleting a habit removes its associated occurrences through the foreign key relationship. The SQLite connection is configured in `HabitTracker/App.config`.

## Project Structure

```text
HabitTracker/
├── App.config             # SQLite connection configuration
├── DataAccess.cs          # ADO.NET database operations
├── Habit.cs               # Habit model and total calculation
├── InteractionConsole.cs  # Main menu and application workflow
├── Occurrence.cs          # Occurrence model
├── Program.cs             # Application entry point
└── Utils/
    ├── AnsiColor.cs       # Console color codes
    └── IOConsole.cs       # Console input and validation helpers
```

`InteractionConsole` coordinates user workflows, `DataAccess` handles SQLite operations, and the model classes represent habits and occurrences. The console utility classes centralize prompts, type conversion, range validation, date parsing, and colored output.

## Testing

Run the test project with:

```powershell
dotnet test .\HabitTrackerTest\HabitTrackerTest.csproj
```

## Development Reflection

Building this application helped reinforce the relationship between console workflows and database operations. Organizing the code into interaction, data-access, model, and console-utility classes made the project easier to reason about and reduced direct database logic inside the menu code.

Using ADO.NET required more explicit SQL, parameter setup, and resource handling than an ORM, but it provided a clear understanding of how the application communicates with SQLite. Input validation was also important because nearly every menu option accepts data from the keyboard; centralizing prompt and validation behavior made it easier to keep the application from crashing during invalid input.

One area that could be improved in a future version is the test coverage. The project contains an NUnit test project, but the application logic is currently not fully covered. Additional work could include unit tests for models and input helpers, as well as integration tests using a temporary SQLite database. Habit and occurrence pagination would also improve usability as the amount of stored data grows.