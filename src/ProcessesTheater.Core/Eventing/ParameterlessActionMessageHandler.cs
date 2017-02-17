namespace ProcessesTheater.Core.Eventing
{
    using System;

    /// <summary>
    /// Basic parameterless action message handler.
    /// </summary>
    /// <typeparam name="TMessage"> Message type. </typeparam>
    public class ParameterlessActionMessageHandler<TMessage> : IMessageHandler<TMessage>
        where TMessage : Message
    {
        /// <summary>
        /// Message handling action.
        /// </summary>
        private readonly Action<TMessage> handlingAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterlessActionMessageHandler{TMessage}"/> class.
        /// </summary>
        /// <param name="handlingAction"> Message handling action. </param>
        public ParameterlessActionMessageHandler(Action<TMessage> handlingAction)
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
