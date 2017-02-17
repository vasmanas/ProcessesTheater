namespace ProcessesTheater.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Revolver cause.
    /// </summary>
    public class RevolverCause : ICause
    {
        /// <summary>
        /// Revolver chambers.
        /// </summary>
        protected readonly List<ICause> Chambers = new List<ICause>();

        /// <summary>
        /// Chambers lock.
        /// </summary>
        protected readonly object ChambersLock = new object();

        /// <summary>
        /// Current position of revolver pointer, that was first not checked.
        /// </summary>
        private int position;

        /// <summary>
        /// Add revolver chamber.
        /// </summary>
        /// <param name="cause"> Cause as a chamber. </param>
        public virtual void AddChamber(ICause cause)
        {
            lock (this.ChambersLock)
            {
                if (this.Chambers.Contains(cause))
                {
                    return;
                }

                this.Chambers.Add(cause);
            }
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            List<ICause> localChambers;
            lock (this.ChambersLock)
            {
                if (!this.Chambers.Any())
                {
                    return null;
                }

                localChambers = this.Chambers.ToList();
            }

            var initialPosition = this.position;

            do
            {
                var effect = localChambers[this.position].Check(cancellationToken);

                this.position++;

                if (localChambers.Count == this.position)
                {
                    this.position = 0;
                }

                if (effect != null)
                {
                    return effect;
                }
            }
            while (initialPosition != this.position);

            return null;
        }
    }
}
