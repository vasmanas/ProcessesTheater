namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

    /// <summary>
    /// Task pool character.
    /// </summary>
    public class TaskPoolCharacter : ICharacter, IDisposable
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Cause.
        /// </summary>
        private readonly ICause cause;

        /// <summary>
        /// Behavior.
        /// </summary>
        private readonly IBehavior behavior;

        /// <summary>
        /// Maximum task count.
        /// </summary>
        private readonly int maxCount;

        /// <summary>
        /// Pool interval.
        /// </summary>
        private readonly TimeSpan poolInterval;

        /// <summary>
        /// The active count Lock.
        /// </summary>
        private readonly object activeCountLock = new object();

        /// <summary>
        /// The active count trigger.
        /// </summary>
        private readonly ManualResetEventSlim activeCountTrigger = new ManualResetEventSlim();

        /// <summary>
        /// Cancelation token.
        /// </summary>
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Active count.
        /// </summary>
        private int activeCount;

        /// <summary>
        /// Is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPoolCharacter" /> class.
        /// </summary>
        /// <param name="cause"> Cause. </param>
        /// <param name="behavior"> Behavior. </param>
        /// <param name="maxCount"> Maximum task count. </param>
        /// <param name="poolInterval"> Pool interval. </param>
        public TaskPoolCharacter(ICause cause, IBehavior behavior, int maxCount, TimeSpan poolInterval, ILog log = null)
        {
            if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            if (behavior == null)
            {
                throw new ArgumentNullException("behavior");
            }

            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException("maxCount", "Max count must be grether than zero");
            }

            this.log = log ?? new EmptyLog();

            this.cause = cause;
            this.behavior = behavior;
            this.maxCount = maxCount;
            this.poolInterval = this.poolInterval.TotalMilliseconds < 100 ? new TimeSpan(0, 0, 0, 0, 100) : poolInterval;
        }

        /// <summary>
        /// Start working.
        /// </summary>
        public void Start()
        {
            this.CheckDisposed();

            Task.Factory.StartNew(
                () =>
                {
                    while (!this.cts.Token.IsCancellationRequested)
                    {
                        if (!this.GetTask(this.cts.Token, true, true))
                        {
                            continue;
                        }

                        this.cts.Token.ThrowIfCancellationRequested();

                        IEffect effect = null;
                        try
                        {
                            effect = this.cause.Check(this.cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            // Suppress
                        }
                        catch (Exception ex)
                        {
                            this.log.ErrorFormat("Cause check failed: {0}", ex);
                        }

                        if (effect == null)
                        {
                            this.cts.Token.ThrowIfCancellationRequested();

                            continue;
                        }

                        this.cts.Token.ThrowIfCancellationRequested();

                        if (!this.GetTask(this.cts.Token, false, false))
                        {
                            throw new Exception("Couldn't get task for starting task.");
                        }

                        Task.Factory.StartNew(
                            eff =>
                                {
                                    try
                                    {
                                        this.behavior.Act(eff as IEffect, this.cts.Token);
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        // Suppress
                                    }
                                    catch (Exception ex)
                                    {
                                        this.log.ErrorFormat("Behavior execution failed: {0}", ex);
                                    }
                                    finally
                                    {
                                        this.ReleaseTask();
                                    }
                                },
                            effect,
                            this.cts.Token);
                    }

                    this.cts.Token.ThrowIfCancellationRequested();
                },
                this.cts.Token);
        }

        /// <summary>
        /// Stop working.
        /// </summary>
        public void Stop()
        {
            this.CheckDisposed();

            this.cts.Cancel();
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

            this.log.DebugFormat("Sending shutdown request for behavior '{0}'...", this.behavior);

            this.cts.Dispose();
        }

        /// <summary>
        /// Get task.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <param name="onlyCheck"> Do not reserve task only check if is available. </param>
        /// <param name="wait"> Wait for free task. </param>
        /// <returns> Was task acquired. </returns>
        private bool GetTask(CancellationToken cancellationToken, bool onlyCheck, bool wait)
        {
            while (true)
            {
                if (this.activeCount < this.maxCount)
                {
                    lock (this.activeCountLock)
                    {
                        if (this.activeCount < this.maxCount)
                        {
                            if (!onlyCheck)
                            {
                                this.activeCount++;
                            }

                            return true;
                        }
                    }
                }

                if (wait)
                {
                    this.activeCountTrigger.Wait(this.poolInterval, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    this.activeCountTrigger.Reset();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The free task.
        /// </summary>
        private void ReleaseTask()
        {
            lock (this.activeCountLock)
            {
                this.activeCount--;
            }

            this.activeCountTrigger.Set();
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
