namespace ProcessesTheater.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Typed queue cause.
    /// </summary>
    /// <typeparam name="TModel"> Model type. </typeparam>
    public class MemoryQueueCause<TModel> : IQueueCause<TModel>
        where TModel : class
    {
        /// <summary>
        /// If true wirks with <see cref="IEffect"/>, else with <see cref="BatchEffect{T}"/>.
        /// </summary>
        private readonly bool getSingleItem;

        /// <summary>
        /// Batch size if operates with batches.
        /// </summary>
        private readonly int batchSize;

        /// <summary>
        /// Base queue.
        /// </summary>
        private readonly ConcurrentQueue<TModel> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryQueueCause{TModel}" /> class.
        /// </summary>
        /// <param name="getSingleItem"> If true gets single item and returns object converted to <see cref="IEffect"/>, if false returns as <see cref="BatchEffect{T}"/>.  </param>
        /// <param name="batchSize"> If operates with batches its size. </param>
        public MemoryQueueCause(bool getSingleItem = true, int batchSize = 1)
        {
            this.queue = new ConcurrentQueue<TModel>();
            this.getSingleItem = getSingleItem;
            this.batchSize = batchSize;
        }
        
        /// <summary>
        /// Enqueue element.
        /// </summary>
        /// <param name="obj"> Element to put to queue. </param>
        /// <param name="parameters"> Additional parameters identifying data. </param>
        public void Enqueue(TModel element, params object[] parameters)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (getSingleItem && !(element is IEffect))
            {
                throw new ArgumentException("Object is not an effect", "obj");
            }

            this.queue.Enqueue(element);
        }
        
        /// <summary>
        /// De queue element.
        /// </summary>
        /// <param name="parameters"> Additional parameters identifying data. </param>
        /// <returns> Element from queue. </returns>
        public TModel Dequeue(params object[] parameters)
        {
            TModel result;

            return this.queue.TryDequeue(out result) ? result : null;
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            if (this.getSingleItem)
            {
                return this.Dequeue() as IEffect;
            }

            var list = new List<TModel>();
            for (var i = 0; i < this.batchSize; i++)
            {
                var result = this.Dequeue();

                if (result == null)
                {
                    break;
                }

                list.Add(result);
            }

            return list.Any() ? new BatchEffect<TModel>(list) : null;
        }
    }
}