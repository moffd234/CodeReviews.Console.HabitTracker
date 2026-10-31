using CSharpCasino.Utils;

namespace HabitTracker;

public class InteractionConsole
{
    private readonly IoConsole _console = new IoConsole();
    private readonly DataAccess _accessor = new DataAccess("HabitTracker");
    private readonly List<Habit> _habits;

    public InteractionConsole()
    {
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
        // Potentially look into using pagination for habits and occurrences
        Habit habit = SelectHabit();

        if (input.Equals(validOptions[0], StringComparison.CurrentCultureIgnoreCase))
        {
            PrintHabit(habit);
            _console.GetStringInput("\nEnter any value to return to main menu");
        }
        else if (input.Equals(validOptions[0], StringComparison.CurrentCultureIgnoreCase))
        {
            // Go to managing habits
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

        foreach (Occurrence occurrence in habit.Occurrences)
        {
            // Print each occurrence in a given habit
            // Look into pagination
            _console.PrintColored(AnsiColor.Purple, $"{occurrence.TimeOfOccurrence}");
        }
    }
}
