namespace HabitTracker;

public class Occurrence(int id, DateTime timeOfOccurrence)
{
    public int Id { get; } = id;
    public DateTime TimeOfOccurrence { get; set; } = timeOfOccurrence;
}