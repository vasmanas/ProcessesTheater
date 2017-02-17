namespace ProcessesTheater.Core
{
    using System.Collections.Concurrent;
    using System.Threading;

    using Eventing;

    /// <summary>
    /// Request reply cause.
    /// </summary>
    /// <typeparam name="TStatus"> Status type. </typeparam>
    public class RequestReplyCause<TStatus> : ICause, IMessageHandler<RequestCommand<TStatus>>
    {
        /// <summary>
        /// Notified by command.
        /// </summary>
        private readonly ManualResetEventSlim notifiedByCommand = new ManualResetEventSlim();

        /// <summary>
        /// Queue.
        /// </summary>
        private readonly ConcurrentQueue<TStatus> queue = new ConcurrentQueue<TStatus>();

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            TStatus status;

            if (this.queue.TryDequeue(out status))
            {
                return new SingleValueEffect<TStatus>(status);
            }

            this.notifiedByCommand.Wait(cancellationToken);
            this.notifiedByCommand.Reset();

            return null;
        }

        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message"> Message to handle. </param>
        public void Handle(RequestCommand<TStatus> message)
        {
            this.queue.Enqueue(message.Status);
            this.notifiedByCommand.Set();
        }
    }
}
