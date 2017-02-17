namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    using Logging;

    /// <summary>
    /// Single thread character.
    /// </summary>
    public class SingleThreadCharacter : ICharacter, IDisposable
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Supervisor thread.
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        /// Cause.
        /// </summary>
        private readonly ICause cause;

        /// <summary>
        /// Behavior.
        /// </summary>
        private readonly IBehavior behavior;

        /// <summary>
        /// Shutdown timeout.
        /// </summary>
        private readonly TimeSpan shutdownTimeout;

        /// <summary>
        /// Starting lock.
        /// </summary>
        private readonly ManualResetEventSlim starting = new ManualResetEventSlim(false);

        /// <summary>
        /// Disposing cancelation token.
        /// </summary>
        private readonly CancellationTokenSource disposingCts = new CancellationTokenSource();

        /// <summary>
        /// Cancellation token trigger lock.
        /// </summary>
        private readonly object ctsLock = new object();

        /// <summary>
        /// Start lock.
        /// </summary>
        private readonly object startLock = new object();

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();
        
        /// <summary>
        /// Is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleThreadCharacter" /> class.
        /// </summary>
        /// <param name="cause"> Cause options. </param>
        /// <param name="behavior"> Behavior options. </param>
        /// <param name="shutdownTimeout"> Shutdown timeout. </param>
        public SingleThreadCharacter(ICause cause, IBehavior behavior, TimeSpan shutdownTimeout, ILog log = null)
        {
            if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            if (behavior == null)
            {
                throw new ArgumentNullException("behavior");
            }

            this.log = log ?? new EmptyLog();

            this.cause = cause;
            this.behavior = behavior;
            this.shutdownTimeout = shutdownTimeout;
            this.thread = new Thread(this.RunComponent) { IsBackground = true };

            this.log.DebugFormat(
                "Starting a new thread for cause '{0}' and behavior '{1}'",
                this.cause.ToString(),
                this.behavior.ToString());

            this.thread.Start();
        }

        /// <summary>
        /// Start execution.
        /// </summary>
        public void Start()
        {
            lock (this.startLock)
            {
                if (this.starting.IsSet)
                {
                    throw new InvalidOperationException("Thread is started, can not start again");
                }

                this.CheckDisposed();

                if (!this.cts.Token.Equals(this.cts.Token))
                {
                    this.cts.Token.Register(this.cts.Cancel);
                }

                this.log.DebugFormat(
                    "Sending start request for cause '{0}' and behavior '{1}'...",
                    this.cause,
                    this.behavior);

                this.starting.Set();
            }
        }

        /// <summary>
        /// Stop supervisor.
        /// </summary>
        public void Stop()
        {
            this.CheckDisposed();

            this.starting.Reset();

            this.log.DebugFormat("Sending stop request for cause '{0}' and behavior '{1}'...", this.cause, this.behavior);

            lock (this.ctsLock)
            {
                this.cts.Cancel();
            }
        }

        /// <summary>
        /// Dispose supervisors.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Stop();

            this.isDisposed = true;

            this.log.DebugFormat(
                "Sending shutdown request for cause '{0}' and behavior '{1}'...",
                this.cause,
                this.behavior);

            this.disposingCts.Cancel();

            if (!this.thread.Join(this.shutdownTimeout))
            {
                this.thread.Abort();
            }

            this.disposingCts.Dispose();
            this.starting.Dispose();
            this.cts.Dispose();
        }

        /// <summary>
        /// Run components.
        /// </summary>
        protected void RunComponent()
        {
            try
            {
                this.log.DebugFormat("Starting cause '{0}' and behavior '{1}'...", this.cause, this.behavior);

                while (true)
                {
                    this.starting.Wait(this.disposingCts.Token);

                    try
                    {
                        while (!this.cts.Token.IsCancellationRequested)
                        {
                            this.cts.Token.ThrowIfCancellationRequested();

                            var effect = this.cause.Check(this.cts.Token);

                            this.cts.Token.ThrowIfCancellationRequested();

                            this.behavior.Act(effect, this.cts.Token);
                        }

                        this.cts.Token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        // Suppress cancelation to proccess in finally block
                    }
                    finally
                    {
                        if (this.cts.IsCancellationRequested)
                        {
                            lock (this.ctsLock)
                            {
                                this.cts.Dispose();
                                this.cts = new CancellationTokenSource();
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                this.log.DebugFormat(
                    "Cause '{0}' and behavior '{1}' finished successfully.",
                    this.cause,
                    this.behavior);
            }
            catch (ThreadAbortException)
            {
                this.log.WarnFormat(
                    "Cause '{0}' and behavior '{1}' caught 'ThreadAbortException' due to shutdown timeout.",
                    this.cause,
                    this.behavior);
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(
                    "Error occurred during execution of cause '{1}' and behavior '{2}'. It will be stopped. See the exception for details: {0}",
                    ex,
                    this.cause,
                    this.behavior);
            }
        }

        /// <summary>
        /// Check if object is disposed.
        /// </summary>
        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
