namespace ProcessesTheater.Core
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Batch effect.
    /// </summary>
    /// <typeparam name="T"> Type of item. </typeparam>
    public class BatchEffect<T> : IEnumerable<T>, IEffect
    {
        /// <summary>
        /// Records.
        /// </summary>
        protected readonly List<T> Records;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEffect{T}" /> class.
        /// </summary>
        /// <param name="records"> Records. </param>
        public BatchEffect(IEnumerable<T> records)
        {
            this.Records = new List<T>(records);
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns> The <see cref="IEnumerator{T}"/>. </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Records.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns> The <see cref="IEnumerator"/>. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}