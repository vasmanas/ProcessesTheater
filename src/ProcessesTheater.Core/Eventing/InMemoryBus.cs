namespace ProcessesTheater.Core.Eventing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// In memory bus.
    /// </summary>
    public class InMemoryBus : IBus
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Registered message handlers.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<object>> RegisteredMessageHandlers =
            new ConcurrentDictionary<Type, List<object>>();

        /// <summary>
        /// Registered request handlers.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> RegisteredRequestHandlers =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryBus"/> class.
        /// </summary>
        /// <param name="log"> Log. </param>
        public InMemoryBus(ILog log = null)
        {
            this.log = log ?? new EmptyLog();
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public void RegisterHandler<T>(IMessageHandler<T> handler) where T : Message
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var handlers = InMemoryBus.RegisteredMessageHandlers.GetOrAdd(typeof(T), type => new List<object>());

            handlers.Add(handler);
        }

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public void UnregisterHandler<T>(IMessageHandler<T> handler) where T : Message
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            List<object> handlers;
            if (InMemoryBus.RegisteredMessageHandlers.TryGetValue(typeof(T), out handlers))
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <typeparam name="T"> Message type. </typeparam>
        /// <param name="message"> Message message. </param>
        public void Send<T>(T message) where T : Message
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var messageType = typeof(T);
            List<object> handlers;

            if (!InMemoryBus.RegisteredMessageHandlers.TryGetValue(messageType, out handlers))
            {
                return;
            }

            var closedHandlers = handlers.Cast<IMessageHandler<T>>().ToList();

            var handled = false;
            foreach (var handler in closedHandlers)
            {
                try
                {
                    handler.Handle(message);
                }
                catch (Exception ex)
                {
                    this.log.ErrorFormat("Failed message (type: {1}) handling with exception: {0}", ex.ToString(), messageType);
                }

                handled = true;
            }

            if (!handled)
            {
                this.log.DebugFormat("Message {0} was not handeled", messageType.Name);
            }
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        /// <param name="replaceIfExists"> Replace registered if exists. If false throws exception. </param>
        public void RegisterHandler<TRequest, TResponse>(
            IRequestHandler<TRequest, TResponse> handler,
            bool replaceIfExists = true) where TRequest : Request<TResponse>
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (replaceIfExists)
            {
                InMemoryBus.RegisteredRequestHandlers.AddOrUpdate(typeof(TRequest), handler, (t, h) => handler);
            }
            else
            {
                if (!InMemoryBus.RegisteredRequestHandlers.TryAdd(typeof(TRequest), handler))
                {
                    throw new InvalidOperationException(
                        string.Format("Handler for request {0} exists", typeof(TRequest)));
                }
            }
        }

        /// <summary>
        /// Unregister handler.
        /// </summary>
        /// <typeparam name="TRequest"> Request type. </typeparam>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="handler"> Message handler. </param>
        public void UnregisterHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : Request<TResponse>
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            object remHandler;
            InMemoryBus.RegisteredRequestHandlers.TryRemove(typeof(TRequest), out remHandler);
        }

        /// <summary>
        /// Query request.
        /// </summary>
        /// <typeparam name="TResponse"> Response type. </typeparam>
        /// <param name="request"> Request. </param>
        /// <returns> Response. </returns>
        public TResponse Query<TResponse>(Request<TResponse> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var requestType = request.GetType();
            object handler;

            if (!InMemoryBus.RegisteredRequestHandlers.TryGetValue(requestType, out handler) || handler == null)
            {
                throw new InvalidOperationException(
                    string.Format("Request handler for response {0} is not registered", requestType.Name));
            }

            var closedHandler = (dynamic)handler;
            
            try
            {
                return closedHandler.Handle((dynamic)request);
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("Failed query (type: {1}) handling with exception: {0}", ex.ToString(), requestType);

                throw;
            }
        }
    }
}
