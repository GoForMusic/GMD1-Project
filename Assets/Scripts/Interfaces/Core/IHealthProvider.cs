namespace Interfaces.Core
{
    public interface IHealthProvider
    {
        /// <summary>
        /// Returns the health component of the minion.
        /// </summary>
        /// <returns>The health component.</returns>
        IHealth GetHealth();
    }
}