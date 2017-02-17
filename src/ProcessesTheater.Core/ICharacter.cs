namespace ProcessesTheater.Core
{
    /// <summary>
    /// Common character interface.
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// Start working.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop working.
        /// </summary>
        void Stop();
    }
}