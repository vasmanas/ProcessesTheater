namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

    /// <summary>
    /// Single task character.
    /// </summary>
    public class SingleTaskCharacter : ICharacter, IDisposable
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Main task.
        /// </summary>
        private readonly Task task;

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
        /// Starting lock.
        /// </summary>
        private readonly ManualResetEventSlim starting = new ManualResetEventSlim(false);

        /// <summary>
        /// Cause.
        /// </summary>
        private readonly ICause cause;

        /// <summary>
        /// Behavior.
        /// </summary>
        private readonly IBehavior behavior;

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTaskCharacter" /> class.
        /// </summary>
        /// <param name="cause"> The <see cref="ICause"/>. </param>
        /// <param name="behavior"> The <see cref="IBehavior"/>. </param>
        /// <param name="log"> The <see cref="ILog"/>. </param>
        public SingleTaskCharacter(ICause cause, IBehavior behavior, ILog log = null)
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

            this.log.DebugFormat(
                "Starting a new task for cause '{0}' and behavior '{1}'",
                this.cause.ToString(),
                this.behavior.ToString());

            this.task =
                Task.Factory.StartNew(
                    this.RunComponent,
                    this.cts.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            lock (this.startLock)
            {
                if (this.starting.IsSet)
                {
                    throw new InvalidOperationException("Task is started, can not start again");
                }

                this.CheckDisposed();

                if (!this.cts.Token.Equals(this.cts.Token))
                {
                    this.cts.Token.Register(this.cts.Cancel);
                }

                this.log.DebugFormat("Sending start request for behavior '{0}'...", this.behavior);

                this.starting.Set();
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            this.CheckDisposed();

            this.starting.Reset();

            this.log.DebugFormat("Sending stop request for behavior '{0}'...", this.behavior);

            lock (this.ctsLock)
            {
                this.cts.Cancel();
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Stop();

            this.isDisposed = true;

            this.log.DebugFormat("Sending shutdown request for behavior '{0}'...", this.behavior);

            this.disposingCts.Cancel();
            this.cts.Dispose();
        }

        /// <summary>
        /// Run components.
        /// </summary>
        protected void RunComponent()
        {
            try
            {
                this.log.DebugFormat("Starting behavior '{0}'...", this.behavior);

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
                this.log.DebugFormat("Behavior '{0}' finished successfully.", this.behavior);
            }
            catch (ThreadAbortException)
            {
                this.log.WarnFormat(
                    "Behavior '{0}' caught 'ThreadAbortException' due to shutdown timeout.",
                    this.behavior);
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(
                    "Error occurred during execution of '{1}' behavior. It will be stopped. See the exception for details: {0}",
                    ex,
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
