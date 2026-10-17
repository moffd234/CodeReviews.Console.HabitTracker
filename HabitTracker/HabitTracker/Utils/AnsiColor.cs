namespace CSharpCasino.Utils;

public enum AnsiColor
{
    Auto,
    Black,
    Red,
    Green,
    Yellow,
    Blue,
    Purple,
    Cyan,
    White
}

public static class AnsiColorExtension
{
    public static string GetCode(this AnsiColor color)
    {
        return color switch
        {
            AnsiColor.Auto => "\u001B[0m",
            AnsiColor.Black => "\u001B[30m",
            AnsiColor.Red => "\u001B[31m",
            AnsiColor.Green => "\u001B[32m",
            AnsiColor.Yellow => "\u001B[33m",
            AnsiColor.Blue => "\u001B[34m",
            AnsiColor.Purple => "\u001B[35m",
            AnsiColor.Cyan => "\u001B[36m",
            AnsiColor.White => "\u001B[37m",

            // Default
            _ => throw new ArgumentOutOfRangeException(nameof(color), color,
                $"{color} is not a valid {nameof(AnsiColor)}")
        };
    }
}