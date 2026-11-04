namespace HabitTracker;

public class Occurrence(int id, DateTime timeOfOccurrence, int quantity)
{
    public int Id { get; } = id;
    public DateTime TimeOfOccurrence { get; set; } = timeOfOccurrence;
    public int Quantity { get; set; } = quantity;

    public Occurrence(DateTime timeOfOccurrence, int quantity) : this(0, timeOfOccurrence, quantity)
    {
    }
}