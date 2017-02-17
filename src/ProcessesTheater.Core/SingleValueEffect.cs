namespace ProcessesTheater.Core
{
    /// <summary>
    /// Single value effect.
    /// </summary>
    /// <typeparam name="T"> Type of element. </typeparam>
    public class SingleValueEffect<T> : IEffect
    {
        /// <summary>
        /// When occurred.
        /// </summary>
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleValueEffect{T}" /> class.
        /// </summary>
        /// <param name="value"> Initial value. </param>
        public SingleValueEffect(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets value.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
