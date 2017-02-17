namespace ProcessesTheater.Core.Eventing
{
    using System;

    /// <summary>
    /// Basic action message handler.
    /// </summary>
    /// <typeparam name="TMessage"> Message type. </typeparam>
    public class ActionMessageHandler<TMessage> : IMessageHandler<TMessage>
        where TMessage : Message
    {
        /// <summary>
        /// Message handling action.
        /// </summary>
        private readonly Action<TMessage> handlingAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageHandler{TMessage}"/> class.
        /// </summary>
        /// <param name="handlingAction"> Message handling action. </param>
        public ActionMessageHandler(Action<TMessage> handlingAction)
        {
            this.handlingAction = handlingAction;
        }

        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message"> Message object. </param>
        public void Handle(TMessage message)
        {
            this.handlingAction(message);
        }
    }
}
