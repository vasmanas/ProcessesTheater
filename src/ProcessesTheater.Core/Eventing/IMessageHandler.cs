namespace ProcessesTheater.Core.Eventing
{
    /// <summary>
    /// Message handler interface.
    /// </summary>
    /// <typeparam name="TMessage"> Message type. </typeparam>
    public interface IMessageHandler<in TMessage>
        where TMessage : Message
    {
        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message"> Message object. </param>
        void Handle(TMessage message);
    }
}