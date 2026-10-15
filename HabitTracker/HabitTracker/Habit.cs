namespace HabitTracker
{
    public class Habit(string name, List<Occurrence> occurrences)
    {
        public string Name { get; set; } = name;
        public List<Occurrence> Occurrences { get; set; } = occurrences;

        public Habit(string name) : this(name, []) {}
        
        
    }
}
