namespace HabitTracker
{
    internal class Habit(string name, List<DateTime> occurrences)
    {
        private string _name = name;
        private List<DateTime> _occurrences = occurrences;

        public Habit(string name) : this(name, []) {}
        
        
    }
}
