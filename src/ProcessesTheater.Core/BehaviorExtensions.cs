namespace ProcessesTheater.Core
{
    using System;

    /// <summary>
    /// Behavior extensions.
    /// </summary>
    public static class BehaviorExtensions
    {
        /// <summary>
        /// Wrap with exception suppressing behavior.
        /// </summary>
        /// <param name="innerBehavior"> Inner behavior. </param>
        /// <returns> Wrapped behavior. </returns>
        public static IBehavior WrapWithExceptionSuppressing(this IBehavior innerBehavior)
        {
            return new ExceptionSuppressingWrapperBehavior(innerBehavior);
        }

        /// <summary>
        /// Wrap with retry behavior.
        /// </summary>
        /// <param name="innerBehavior"> Inner behavior. </param>
        /// <param name="maxRetryAttempts"> Max retry attempts. </param>
        /// <param name="retryPeriodProvider"> Retry period provider. </param>
        /// <returns> Wrapped behavior. </returns>
        public static IBehavior WrapWithRetry(
            this IBehavior innerBehavior,
            int maxRetryAttempts = 1,
            Func<int, TimeSpan> retryPeriodProvider = null)
        {
            return new RetryWrapperBehavior(innerBehavior, maxRetryAttempts, retryPeriodProvider);
        }

        /// <summary>
        /// Wrap with debug behavior.
        /// </summary>
        /// <param name="innerBehavior"> Inner behavior. </param>
        /// <param name="name"> Name to print. </param>
        /// <returns> Wrapped behavior. </returns>
        public static IBehavior WrapWithDebug(this IBehavior innerBehavior, string name = null)
        {
            return new DebugBehavior(innerBehavior, name);
        }

        /// <summary>
        /// Wrap with function behavior.
        /// </summary>
        /// <param name="innerBehavior"> Inner behavior. </param>
        /// <param name="wrappingFunc"> Wrapping function. </param>
        /// <returns> Wrapped behavior. </returns>
        public static IBehavior WrapWithFunc(this IBehavior innerBehavior, Func<IBehavior, IBehavior> wrappingFunc)
        {
            if (wrappingFunc == null)
            {
                throw new ArgumentNullException("wrappingFunc");
            }

            return wrappingFunc(innerBehavior);
        }
    }
}
