using Nito.Collections;

namespace NetScape.Abstractions.Model.Game
{
    /// <summary>
    /// A queue of <see cref="Direction"/>s which a <see cref="Mob"/> will follow.
    /// </summary>
    public class WalkingQueue
    {
        /// <summary>
        /// The Deque of active points in this WalkingQueue.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public Deque<Position> Points { get; } = new();

        /// <summary>
        /// The Deque of previous points in this WalkingQueue.
        /// </summary>
        /// <value>
        /// The previous points.
        /// </value>  
        public Deque<Position> PreviousPoints { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WalkingQueue"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>      
        public bool Running { get; set; }
    }
}
