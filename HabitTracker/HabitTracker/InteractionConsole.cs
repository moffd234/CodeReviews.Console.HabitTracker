using CSharpCasino.Utils;

namespace HabitTracker;

public class InteractionConsole
{
    private readonly IoConsole _console = new IoConsole();
    private readonly DataAccess _accessor = new DataAccess("HabitTracker");
    private readonly List<Habit> _habits;

    public InteractionConsole()
    {
        _accessor.CreateTable();
        _habits = _accessor.GetAllHabits();
    }

    public void Run()
    {
        _console.PrintColored("************************************************" +
                              "Welcome to your habit tracker" +
                              "Type exit at any time to exit application" +
                              "***********************************************");
        while (true)
        {
            // Note: If user types exit in any input prompt that uses console.GetStringInput
            //       the application immediately exits with exit code 0
            GetStarterInput();
        }
    }

    private void GetStarterInput()
    {
        string[] validOptions = ["v", "m"];
        string input = _console.GetValidInput("What would you like to do?\n" +
                                              "V - View habits\n" +
                                              "M - Manage Habits\n", validOptions);

        if (input.Equals(validOptions[0], StringComparison.OrdinalIgnoreCase))
        {
            // Potentially look into using pagination for habits and occurrences
            Habit habit = SelectHabit();
            PrintHabit(habit);
            _console.GetStringInput("\nEnter any value to return to main menu");
        }
        else if (input.Equals(validOptions[1], StringComparison.OrdinalIgnoreCase))
        {
            ManageHabits();
        }
    }

    private Habit SelectHabit()
    {
        for (int i = 0; i < _habits.Count; i++)
        {
            _console.PrintColored($"{i} {_habits[i].Name}");
        }

        int choice = _console.GetValidIntOption("Enter habit number", 0, _habits.Count - 1);
        return _habits[choice];
    }

    private void PrintHabit(Habit habit)
    {
        _console.PrintColored(AnsiColor.Blue, $"{habit.Name}");

        for (int i = 0; i < habit.Occurrences.Count; i++)
        {
            // Print each occurrence in a given habit
            // Look into pagination
            _console.PrintColored(AnsiColor.Purple, $"{i} {habit.Occurrences[i].TimeOfOccurrence}");
        }
    }

    private void ManageHabits()
    {
        string[] validOptions = ["a", "m"];
        string input = _console.GetValidInput("A - Add Habit\nM - Update existing habit", validOptions);

        if (input.Equals(validOptions[0], StringComparison.OrdinalIgnoreCase))
        {
            string name = _console.GetStringInput("What is the name of the habit");
            Habit habit = new Habit(name.ToLower());

            _accessor.InsertHabit(habit);
            _habits.Add(habit);
        }
        else if (input.Equals(validOptions[1], StringComparison.OrdinalIgnoreCase))
        {
            UpdateHabit();
        }
    }

    private void UpdateHabit()
    {
        string habitToUpdate = _console.GetStringInput("What is the name of the habit");
        Habit? habit = _accessor.FindHabitByName(habitToUpdate.ToLower());

        while (habit == null)
        {
            if (habitToUpdate.Equals("back", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _console.PrintError("Habit not found by that name. Try Again or type back");
            habitToUpdate = _console.GetStringInput("What is the name of the habit");
            habit = _accessor.FindHabitByName(habitToUpdate);
        }

        string[] validOptions = ["a", "u"];
        string input = _console.GetValidInput("A - Add Occurrence\n O - Update Occurrence", validOptions);

        if (input.Equals(validOptions[0], StringComparison.OrdinalIgnoreCase))
        {
            AddNewOccurrence(habit);
        }
        else if (input.Equals(validOptions[1], StringComparison.OrdinalIgnoreCase))
        {
            UpdateOccurrence(habit);
        }
    }

    private void AddNewOccurrence(Habit habit)
    {
        Occurrence occurrence = new Occurrence(GetOccurrenceDate("Enter occurrence time"));

        _accessor.InsertOccurrence(habit.Name, occurrence);
        Habit? existingHabit = _habits.FirstOrDefault(
            h => h.Name.Equals(habit.Name, StringComparison.OrdinalIgnoreCase));

        existingHabit?.Occurrences.Add(occurrence);
    }

    private void UpdateOccurrence(Habit habit)
    {
        PrintHabit(habit);
        int input = _console.GetIntInput("Enter the number of the occurrence you would like to update");
        Occurrence occurrence = habit.Occurrences[input];

        occurrence.TimeOfOccurrence = GetOccurrenceDate("Enter new occurrence time");
        _accessor.EditOccurrence(occurrence);
    }

    private DateTime GetOccurrenceDate(string prompt)
    {
        _console.PrintColored(prompt);
        bool useCurrentTime = _console.GetYesNoInput("Would you like to use current time?");
        return useCurrentTime ? DateTime.Now : _console.GetValidDateTime("Enter a date (HH-mm MM-dd-yyyy)");
    }
}