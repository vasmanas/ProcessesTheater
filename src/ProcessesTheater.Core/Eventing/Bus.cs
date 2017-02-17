namespace ProcessesTheater.Core.Eventing
{
    using System;
    using Logging;

    /// <summary>
    /// Static bus class.
    /// </summary>
    public static class Bus
    {
        private static readonly EmptyLog emptyLog = new EmptyLog();

        /// <summary> Logging interface. </summary>
        private static ILog log = Bus.emptyLog;

        public static void SetLog(ILog log)
        {
            Bus.log = log ?? Bus.emptyLog;
        }

        /// <summary>
        /// Message bus.
        /// </summary>
        private static IBus bus;

        /// <summary>
        /// Set bus.
        /// </summary>
        /// <param name="newBus"> Nes bus. </param>
        public static void SetBus(IBus newBus)
        {
            Bus.bus = newBus;
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public static void RegisterHandler<T>(IMessageHandler<T> handler) where T : Message
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.bus.RegisterHandler(handler);
        }

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public static void UnregisterHandler<T>(IMessageHandler<T> handler) where T : Message
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.bus.UnregisterHandler(handler);
        }

        /// <summary>
        /// Send command.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="message"> Message message. </param>
        public static void Send<T>(T message) where T : Message
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.log.Debug(message.ToString());

            Bus.bus.Send(message);
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        /// <param name="replaceIfExists"> Replace registered if exists. If false throws exception. </param>
        public static void RegisterHandler<TRequest, TResponse>(
            IRequestHandler<TRequest, TResponse> handler,
            bool replaceIfExists = true) where TRequest : Request<TResponse>
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.bus.RegisterHandler(handler, replaceIfExists);
        }

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public static void UnregisterHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : Request<TResponse>
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.bus.UnregisterHandler(handler);
        }

        /// <summary>
        /// Query request.
        /// </summary>
        /// <typeparam name="TResponse"> Result type. </typeparam>
        /// <param name="request"> Request. </param>
        /// <returns> Response. </returns>
        public static TResponse Query<TResponse>(Request<TResponse> request)
        {
            if (Bus.bus == null)
            {
                throw new InvalidOperationException("Message bus not set in static class");
            }

            Bus.log.Debug(request.ToString());

            return Bus.bus.Query(request);
        }
    }
}
