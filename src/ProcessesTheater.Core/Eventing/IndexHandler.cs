namespace ProcessesTheater.Core.Eventing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Index handler used for hadling messages with filterable property.
    /// </summary>
    /// <typeparam name="TMessage"> Message type. </typeparam>
    /// <typeparam name="TFilterKey"> Filter key type. </typeparam>
    /// <typeparam name="THandlerKey"> Handler key type. </typeparam>
    public class IndexHandler<TMessage, TFilterKey, THandlerKey> : IMessageHandler<TMessage> where TMessage : Message
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Message to key function.
        /// </summary>
        private readonly Func<TMessage, IEnumerable<TFilterKey>> keyFunc;

        /// <summary>
        /// Message handlers.
        /// </summary>
        private readonly ConcurrentDictionary<TFilterKey, ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>>>
            handlers =
                new ConcurrentDictionary<TFilterKey, ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexHandler{TMessage,TFilterKey,THandlerKey}"/> class.
        /// </summary>
        /// <param name="keyFunc"> Function for key extraction. </param>
        /// <param name="log"> Log. </param>
        public IndexHandler(Func<TMessage, TFilterKey> keyFunc, ILog log = null)
            : this(m => new List<TFilterKey> { keyFunc(m) }, log)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexHandler{TMessage,TFilterKey,THandlerKey}"/> class.
        /// </summary>
        /// <param name="keyFunc"> Function for key extraction. </param>
        /// <param name="log"> Log. </param>
        public IndexHandler(Func<TMessage, IEnumerable<TFilterKey>> keyFunc, ILog log = null)
        {
            this.log = log ?? new EmptyLog();
            this.keyFunc = keyFunc;
        }

        /// <summary>
        /// Checks if handler exists.
        /// </summary>
        /// <param name="filterKey"> Key for finding message. </param>
        /// <param name="handlerKey"> Key for finding handler. </param>
        /// <returns> True if handler exists. </returns>
        public bool HandlerExists(TFilterKey filterKey, THandlerKey handlerKey)
        {
            ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>> messageHandlers;

            return this.handlers.TryGetValue(filterKey, out messageHandlers) && messageHandlers.ContainsKey(handlerKey);
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <param name="filterKey"> Key for finding message. </param>
        /// <param name="handlerKey"> Key for finding handler. </param>
        /// <param name="handler"> Message handler. </param>
        /// <returns> True if handler was added. </returns>
        public bool RegisterHandler(TFilterKey filterKey, THandlerKey handlerKey, IMessageHandler<TMessage> handler)
        {
            if (handler == null)
            {
                return false;
            }

            var result = false;
            this.handlers.AddOrUpdate(
                filterKey,
                keyNew =>
                    {
                        var dict = new ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>>();
                        dict.TryAdd(handlerKey, handler);
                        return dict;
                    },
                (keyExist, list) =>
                    {
                        if (!list.ContainsKey(handlerKey))
                        {
                            list.TryAdd(handlerKey, handler);
                            result = true;
                        }

                        return list;
                    });

            return result;
        }

        /// <summary>
        /// Register handler.
        /// </summary>
        /// <param name="filterKey"> Key for finding message. </param>
        /// <param name="handlerKey"> Key for finding handler. </param>
        /// <returns> True if handler was removed. </returns>
        public bool UnregisterHandler(TFilterKey filterKey, THandlerKey handlerKey)
        {
            ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>> messageHandlers;

            if (!this.handlers.TryGetValue(filterKey, out messageHandlers) || !messageHandlers.Any())
            {
                return false;
            }

            IMessageHandler<TMessage> handler;

            return messageHandlers.TryRemove(handlerKey, out handler);
        }

        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message"> Message object. </param>
        public void Handle(TMessage message)
        {
            var keySet = this.keyFunc(message);

            foreach (var key in keySet)
            {
                ConcurrentDictionary<THandlerKey, IMessageHandler<TMessage>> messageHandlers;

                if (!this.handlers.TryGetValue(key, out messageHandlers) || !messageHandlers.Any())
                {
                    return;
                }

                var handled = false;
                foreach (var handler in messageHandlers.Values.ToList())
                {
                    try
                    {
                        handler.Handle(message);
                    }
                    catch (Exception ex)
                    {
                        this.log.ErrorFormat(
                            "Failed message (type: {1}) handling on index handler with exception: {0}",
                            ex.ToString(),
                            typeof(TMessage));
                    }

                    handled = true;
                }

                if (!handled)
                {
                    this.log.WarnFormat(
                        "Message {0} was not handeled on index handler. Either no handlers defined or all threw exceptions.",
                        typeof(TMessage).Name);
                }
            }
        }
    }
}
