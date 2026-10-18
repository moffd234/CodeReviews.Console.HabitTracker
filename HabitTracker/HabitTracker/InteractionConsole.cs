using CSharpCasino.Utils;

namespace HabitTracker;

public class InteractionConsole
{
    private readonly IoConsole _console = new IoConsole();
    private readonly DataAccess _accessor = new DataAccess("HabitTracker.db");
    private List<Habit> _habits;

    InteractionConsole()
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
            //       the application is immediately exited with exit code 0
            GetStarterInput();   
        }
    }

    private void GetStarterInput()
    {
        string[] validOptions = ["v", "m"];
        string input = _console.GetValidInput("What would you like to do?\n" +
                                              "V - View habits\n" +
                                              "M - Manage Habits\n", validOptions);
        if (input.Equals(validOptions[0], StringComparison.CurrentCultureIgnoreCase))
        {
            // Go to habit viewing
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
    
    
}