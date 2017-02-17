namespace ProcessesTheater.Core
{
    using ProcessesTheater.Core.Eventing;

    /// <summary>
    /// Request command.
    /// </summary>
    /// <typeparam name="TStatus"> Status type. </typeparam>
    public class RequestCommand<TStatus> : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestCommand{TStatus}"/> class.
        /// </summary>
        /// <param name="status"> Request status. </param>
        public RequestCommand(TStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets status.
        /// </summary>
        public TStatus Status { get; private set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> Returns message as sting. </returns>
        public override string ToString()
        {
            return string.Format("{0}:Status({1})", this.GetType().Name, this.Status);
        }
    }
}
