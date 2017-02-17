namespace ProcessesTheater.Core
{
    /// <summary>
    /// Queue cause interface.
    /// </summary>
    /// <typeparam name="TModel"> Model type. </typeparam>
    public interface IQueueCause<TModel> : ICause
    {
        /// <summary>
        /// Enqueue element.
        /// </summary>
        /// <param name="element"> Element to put to queue. </param>
        /// <param name="parameters"> Additional parameters identifying data. </param>
        void Enqueue(TModel element, params object[] parameters);

        /// <summary>
        /// De queue element.
        /// </summary>
        /// <param name="parameters"> Additional parameters identifying data. </param>
        /// <returns> Element from queue. </returns>
        TModel Dequeue(params object[] parameters);
    }
}