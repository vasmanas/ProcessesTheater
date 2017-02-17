namespace ProcessesTheater.Core.Eventing
{
    /// <summary>
    /// Bus interface.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        void RegisterHandler<T>(IMessageHandler<T> handler) where T : Message;

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        void UnregisterHandler<T>(IMessageHandler<T> handler) where T : Message;

        /// <summary>
        /// Send command.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="message"> Message message. </param>
        void Send<T>(T message) where T : Message;

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        /// <param name="replaceIfExists"> Replace registered if exists. If false throws exception. </param>
        void RegisterHandler<TRequest, TResponse>(
            IRequestHandler<TRequest, TResponse> handler,
            bool replaceIfExists = true) where TRequest : Request<TResponse>;

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        void UnregisterHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : Request<TResponse>;

        /// <summary>
        /// Query request.
        /// </summary>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="request"> Request. </param>
        /// <returns> Response. </returns>
        TResponse Query<TResponse>(Request<TResponse> request);
    }
}