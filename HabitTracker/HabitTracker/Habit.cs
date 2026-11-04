namespace HabitTracker
{
    public class Habit(string name, string unit, List<Occurrence> occurrences)
    {
        public string Name { get; set; } = name;
        public string Unit { get; set; } = unit;
        public List<Occurrence> Occurrences { get; set; } = occurrences;

        public Habit(string name, String unit) : this(name, unit, []) {}
        
        public int GetTotalQuantity()
        {
            return Occurrences.Sum(occurrence => occurrence.Quantity);
        }
        
        
    }
}
