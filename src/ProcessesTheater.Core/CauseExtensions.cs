namespace ProcessesTheater.Core
{
    using System;

    /// <summary>
    /// Cause extensions.
    /// </summary>
    public static class CauseExtensions
    {
        /// <summary>
        /// Wrap with exception suppressing cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithExceptionSuppressing(this ICause innerCause)
        {
            return new ExceptionSuppressingWrapperCause(innerCause);
        }

        /// <summary>
        /// Wrap with pause cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <param name="periodSec"> Period to wait in seconds. </param>
        /// <param name="firstTimeExecute"> First time execute don't wait. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithPause(this ICause innerCause, double periodSec, bool firstTimeExecute = true)
        {
            return new PauseWrapperCause(innerCause, periodSec, firstTimeExecute);
        }

        /// <summary>
        /// Wrap with required cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithRequired(this ICause innerCause)
        {
            return new RequiredWrapperCause(innerCause);
        }

        /// <summary>
        /// Wrap with required cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <param name="poolPeriodSec"> Period to wait if previous data not found. </param>
        /// <param name="firstTimeExecute"> First time execute don't wait. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithPool(this ICause innerCause, double poolPeriodSec, bool firstTimeExecute = true)
        {
            return new PoolWrapperCause(innerCause, poolPeriodSec, firstTimeExecute);
        }

        /// <summary>
        /// Wrap with retry cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <param name="maxRetryAttempts"> Max retry attempts. </param>
        /// <param name="retryPeriodProvider"> Retry period provider. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithRetry(
            this ICause innerCause,
            int maxRetryAttempts,
            Func<int, TimeSpan> retryPeriodProvider = null)
        {
            return new RetryWrapperCause(innerCause, maxRetryAttempts, retryPeriodProvider);
        }

        /// <summary>
        /// Wrap with debug cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <param name="name"> Name to print. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithDebug(this ICause innerCause, string name = null)
        {
            return new DebugCause(innerCause, name);
        }

        /// <summary>
        /// Wrap with function cause.
        /// </summary>
        /// <param name="innerCause"> Inner cause. </param>
        /// <param name="wrappingFunc"> Wrapping function. </param>
        /// <returns> Wrapped cause. </returns>
        public static ICause WrapWithFunc(this ICause innerCause, Func<ICause, ICause> wrappingFunc)
        {
            if (wrappingFunc == null)
            {
                throw new ArgumentNullException("wrappingFunc");
            }

            return wrappingFunc(innerCause);
        }
    }
}
