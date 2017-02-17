namespace ProcessesTheater.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Chain behavior.
    /// </summary>
    public class ChainBehavior : IBehavior
    {
        /// <summary>
        /// Value indicating whether execute only first chain link.
        /// </summary>
        private readonly bool firstOnly;

        /// <summary>
        /// Value indicating whether effect is batch.
        /// </summary>
        private readonly bool effectIsBatch;

        /// <summary>
        /// First chain link.
        /// </summary>
        private ChainLinkBehavior firstChainLink;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainBehavior" /> class.
        /// </summary>
        /// <param name="firstOnly"> Value indicating whether execute only first chain link. </param>
        /// <param name="effectIsBatch"> Value indicating whether effect is batch. </param>
        public ChainBehavior(bool firstOnly = true, bool effectIsBatch = false)
        {
            this.firstOnly = firstOnly;
            this.effectIsBatch = effectIsBatch;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public void Act(IEffect value, CancellationToken cancellationToken)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.firstChainLink == null)
            {
                throw new InvalidOperationException("No chain links were added");
            }

            List<IEffect> list;

            if (this.effectIsBatch)
            {
                var enumerable = value as IEnumerable;

                if (enumerable == null)
                {
                    throw new ArgumentNullException("value", "effect is not collection");
                }

                list = enumerable.OfType<IEffect>().ToList();
            }
            else
            {
                list = new List<IEffect> { value };
            }

            foreach (var effect in list)
            {
                this.firstChainLink.Act(effect, cancellationToken);
            }
        }

        /// <summary>
        /// Add link.
        /// </summary>
        /// <param name="checkFunc"> Check function. </param>
        /// <param name="behaviorFactory"> Behavior factory. </param>
        public void AddLink(Func<IEffect, bool> checkFunc, Func<IBehavior> behaviorFactory)
        {
            if (checkFunc == null)
            {
                throw new ArgumentNullException("checkFunc");
            }

            if (behaviorFactory == null)
            {
                throw new ArgumentNullException("behaviorFactory");
            }

            var cl = new ChainLinkBehavior(checkFunc, behaviorFactory, this.firstOnly);

            if (this.firstChainLink == null)
            {
                this.firstChainLink = cl;
            }
            else
            {
                this.firstChainLink.SetNext(cl);
            }
        }

        /// <summary>
        /// Chain link behavior.
        /// </summary>
        private class ChainLinkBehavior : IBehavior
        {
            /// <summary>
            /// Execute only first chain link.
            /// </summary>
            private readonly bool firstOnly;

            /// <summary>
            /// Check function.
            /// </summary>
            private readonly Func<IEffect, bool> check;

            /// <summary>
            /// Behavior factory.
            /// </summary>
            private readonly Func<IBehavior> behaviorFactory;

            /// <summary>
            /// Next link.
            /// </summary>
            private ChainLinkBehavior nextLink;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChainLinkBehavior" /> class.
            /// </summary>
            /// <param name="check"> Check function. </param>
            /// <param name="behaviorFactory"> Behavior factory. </param>
            /// <param name="firstOnly"> Execute only first chain link. </param>
            public ChainLinkBehavior(Func<IEffect, bool> check, Func<IBehavior> behaviorFactory, bool firstOnly)
            {
                if (check == null)
                {
                    throw new ArgumentNullException("check");
                }

                if (behaviorFactory == null)
                {
                    throw new ArgumentNullException("behaviorFactory");
                }

                this.firstOnly = firstOnly;
                this.check = check;
                this.behaviorFactory = behaviorFactory;
            }

            /// <summary>
            /// Set next link.
            /// </summary>
            /// <param name="next"> Next link. </param>
            public void SetNext(ChainLinkBehavior next)
            {
                if (this.nextLink == null)
                {
                    this.nextLink = next;
                }
                else
                {
                    this.nextLink.SetNext(next);
                }
            }

            /// <summary>
            /// Act method.
            /// </summary>
            /// <param name="value"> Value to execute behavior on. </param>
            /// <param name="cancellationToken"> Cancellation token. </param>
            public void Act(IEffect value, CancellationToken cancellationToken)
            {
                if (this.check(value))
                {
                    var behavior = this.behaviorFactory();

                    if (behavior == null)
                    {
                        throw new NullReferenceException(
                            string.Format(
                                "Behavior factory did not produce behavior in chain link for effect of type {0}",
                                value.GetType()));
                    }

                    behavior.Act(value, cancellationToken);

                    if (this.firstOnly)
                    {
                        return;
                    }
                }

                if (this.nextLink == null)
                {
                    throw new InvalidOperationException(string.Format("Behavior not found for effect {0}", value));
                }

                this.nextLink.Act(value, cancellationToken);
            }
        }
    }
}
