namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Indicator cause.
    /// </summary>
    /// <typeparam name="TData"> Data type. </typeparam>
    public class IndicatorCause<TData> : ICause
    {
        /// <summary>
        /// Effect factory.
        /// </summary>
        private readonly Func<TData, IEffect> effectFactory;

        /// <summary>
        /// Data lock object.
        /// </summary>
        private readonly object dataLock = new object();

        /// <summary>
        /// Data value.
        /// </summary>
        private TData dataValue;

        /// <summary>
        /// Indicates data received.
        /// </summary>
        private bool dataReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndicatorCause{TData}"/> class.
        /// </summary>
        /// <param name="effectFactory"> Effect factory. </param>
        public IndicatorCause(Func<TData, IEffect> effectFactory = null)
        {
            this.effectFactory = this.effectFactory == null ? d => new SingleValueEffect<TData>(d) : effectFactory;
        }

        /// <summary>
        /// Indicate data.
        /// </summary>
        /// <param name="value"> New value. </param>
        public void Indicate(TData value)
        {
            lock (this.dataLock)
            {
                this.dataValue = value;
                this.dataReceived = true;
            }
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            if (!this.dataReceived)
            {
                return null;
            }

            TData data;

            lock (this.dataLock)
            {
                if (!this.dataReceived)
                {
                    return null;
                }

                this.dataReceived = false;
                data = this.dataValue;
            }

            return this.effectFactory(data);
        }
    }
}
