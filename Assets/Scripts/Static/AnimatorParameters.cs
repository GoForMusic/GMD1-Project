namespace Static
{
    /// <summary>
    /// Abstract base class for all enumerations regarding the animator parameters
    /// </summary>
    public abstract class Enumeration
    {
        public string Name { get; } // Name of the enumeration value
        
        // Constructor to initialize ID and name
        protected Enumeration(string name)
        {
            Name = name;
        }

        // Override ToString() to return the name of the enumeration value
        public override string ToString() => Name;
    }
    
    /// <summary>
    /// Static class that will manipulate the Enum ex (AnimatorParameters.Attack)
    /// </summary>
    public class AnimatorParameters : Enumeration
    {
        // Static members representing specific animator parameters
        public static AnimatorParameters MovementSpeed => new("MovementSpeed");
        public static AnimatorParameters Attack => new("Attack");
        public static AnimatorParameters Die => new("Die");
        public static AnimatorParameters AttackRandom => new("AttackRandom");
            
        // Private constructor to prevent external instantiation
        private AnimatorParameters(string name) : base(name){}
        // Method to return the parameter name as a string
        public static implicit operator string(AnimatorParameters parameter)
        {
            return parameter.Name.ToString();
        }
    }
}