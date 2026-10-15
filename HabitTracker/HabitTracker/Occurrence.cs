namespace HabitTracker;

public class Occurrence(int id, DateTime timeOfOccurrence)
{
    private int _id = id;
    public DateTime TimeOfOccurrence { get; set; } = timeOfOccurrence;
}