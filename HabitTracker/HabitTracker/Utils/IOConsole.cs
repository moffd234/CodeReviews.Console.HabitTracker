using System.Globalization;

namespace CSharpCasino.Utils;

public class IoConsole(AnsiColor color)
{
    private readonly AnsiColor _color = color;

    public IoConsole() : this(AnsiColor.Auto) // Calls Constructor with color parameter
    {
    }

    public void PrintColored(AnsiColor color, string message)
    {
        Console.WriteLine($"{color.GetCode()}{message}{AnsiColor.Auto.GetCode()}");
    }

    public void PrintColored(string message)
    {
        Console.WriteLine($"{_color.GetCode()}{message}{AnsiColor.Auto.GetCode()}");
    }

    public void PrintError(string message)
    {
        PrintColored(AnsiColor.Red, message);
    }

    public void PrintSuccess(string message)
    {
        PrintColored(AnsiColor.Green, message);
    }

    private string RepromptInput(string prompt, string message)
    {
        PrintError($"{message}. Please try again");
        return GetStringInput(prompt);
    }

    private static bool CheckForExit(string input)
    {
        return input.ToLower() == "exit";
    }

    private string GetStringInput(string prompt)
    {
        PrintColored(prompt);
        string? input = Console.ReadLine();

        while (input == null)
        {
            input = RepromptInput(prompt, "Invalid Input");
        }

        if (CheckForExit(input))
        {
            Environment.Exit(0);
        }
        
        return input;
    }

    public double GetDoubleInput(string prompt)
    {
        string input = GetStringInput(prompt);
        double doubleInput;
        
        while (!double.TryParse(input, out doubleInput))
        {
            input = RepromptInput(prompt, $"{input} is not a valid decimal number");
        }

        return doubleInput;
    }

    private float GetFloatInput(string prompt)
    {
        string input = GetStringInput(prompt);
        float floatInput;
        
        while (!float.TryParse(input, out floatInput))
        {
            input = RepromptInput(prompt, "Number is not a valid float");
        }

        return floatInput;
    }

    public int GetIntInput(string prompt)
    {
        string input = GetStringInput(prompt);
        int intInput;
        
        while (!int.TryParse(input, out intInput))
        {
            input = RepromptInput(prompt, "Number is not an integer");
        }

        return intInput;
    }

    public bool GetBooleanInput(string prompt)
    {
        string input = GetStringInput(prompt);
        bool booleanInput;

        while (!bool.TryParse(input, out booleanInput))
        {
            input = RepromptInput(prompt, "Input is not a boolean");
        }

        return booleanInput;
    }

    public bool GetYesNoInput(string prompt)
    {
        string[] acceptedInputs = ["yes", "no", "y", "n"];
        string input = GetValidInput(prompt, acceptedInputs).ToLower();

        if(input == "yes" || input == "y")
        {
            return true;
        }

        return false;
    }

    private static int CountDigitsAfterDecimal(float input) {
        string inputAsString = input.ToString(CultureInfo.InvariantCulture);

        return !inputAsString.Contains('.') ? 0 : inputAsString.Split(".")[1].Length;
    }

    public float GetMonetaryInput(string prompt)
    {
        float input = GetFloatInput(prompt);
        int digitsAfterDecimal = CountDigitsAfterDecimal(input);

        while(digitsAfterDecimal > 2 || input < 0)
        {
            PrintError("Invalid monetary input");
            input = GetFloatInput("Please try again");
            digitsAfterDecimal = CountDigitsAfterDecimal(input);
        }

        return input;
    }

    public string GetValidInput(string prompt, string[] validOptions)
    {
        string input = GetStringInput(prompt);


        while(!validOptions.Contains(input))
        {
            PrintError("Invalid input. Please enter a valid input");
            input = GetStringInput(prompt);
        }

        return input;
    }
}
