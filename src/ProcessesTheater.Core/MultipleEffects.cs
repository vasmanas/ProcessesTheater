namespace ProcessesTheater.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Multiple effects.
    /// </summary>
    /// <typeparam name="T"> Type for batch. </typeparam>
    public class MultipleEffects<T> : BatchEffect<T>
        where T : class, IEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEffects{T}" /> class.
        /// </summary>
        /// <param name="records"> Records. </param>
        public MultipleEffects(IEnumerable<T> records)
            : base(records)
        {
        }
    }
}
