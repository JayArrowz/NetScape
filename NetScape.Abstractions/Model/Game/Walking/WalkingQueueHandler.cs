using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Interfaces.Region.Collision;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Abstractions.Model.Game.Walking
{
    public class WalkingQueueHandler
    {
        private readonly ICollisionManager _collisionManager;
        private readonly IRegionRepository _regionRepository;

        public WalkingQueueHandler(ICollisionManager collisionManager, IRegionRepository regionRepository)
        {
            _collisionManager = collisionManager;
            _regionRepository = regionRepository;
        }

        /// <summary>
        /// Adds the first step to the <see cref="Mob"/>s <see cref="WalkingQueue"/>.
        /// </summary>
        /// <param name="mob">The mob.</param>
        /// <param name="next">The next <see cref="Position"/> of the step.</param>
        public void AddFirstStep(Mob mob, Position next)
        {
            var walkingQueue = mob.WalkingQueue;
            walkingQueue.Points.Clear();

            // We need to connect 'current' and 'next' whilst accounting for the
            // fact that the client and server might be out of sync (i.e. what the
            // client thinks is 'current' is different to what the server thinks is
            // 'current').
            //
            // First try to connect them via points from the previous queue.
            Queue<Position> backtrack = new();
            var previousPoints = walkingQueue.PreviousPoints;
            while (previousPoints.Any())
            {
                Position position = previousPoints.RemoveFromBack();
                backtrack.Enqueue(position);

                if (position.Equals(next))
                {
                    while (backtrack.Any())
                    {
                        var step = backtrack.Dequeue();
                        this.AddStep(mob, step);
                    }
                    previousPoints.Clear();
                    return;
                }
            }

            /* If that doesn't work, connect the points directly. */
            previousPoints.Clear();
            AddStep(mob, next);
        }

        /// <summary>
        /// Adds the step to the <see cref="Mob"/>s <see cref="WalkingQueue"/>.
        /// </summary>
        /// <param name="mob">The mob.</param>
        /// <param name="next">The next <see cref="Position"/> of the step.</param>
        public void AddStep(Mob mob, Position next)
        {
            var walkingQueue = mob.WalkingQueue;
            Position current = walkingQueue.Points.LastOrDefault();

            // If current equals next, addFirstStep doesn't end up adding anything points queue. This makes peekLast()
            // return null. If it does, the correct behaviour is to fill it in with mob.getPosition().
            if (current == null)
            {
                current = mob.Position;
            }

            AddStep(mob, current, next);
        }

        /// <summary>
        /// Clears the <paramref name="mob"/>s <see cref="WalkingQueue"/>.
        /// </summary>
        /// <param name="mob">The mob.</param>
        /// <returns></returns>    
        public void Clear(Mob mob)
        {
            var walkingQueue = mob.WalkingQueue;
            walkingQueue.Points.Clear();
            walkingQueue.PreviousPoints.Clear();
        }

        /// <summary>
        /// Adds the <c>next</c> step to this WalkingQueue.
        /// </summary>
        /// <param name="mob">The mob.</param>
        /// <param name="current">The current <see cref="Position"/>.</param>
        /// <param name="next">The next Position.</param>     
        private void AddStep(Mob mob, Position current, Position next)
        {
            int nextX = next.X, nextY = next.Y, height = next.Height;
            int deltaX = nextX - current.X;
            int deltaY = nextY - current.Y;

            int max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
            IRegion region = _regionRepository.FromPosition(current);

            for (int count = 0; count < max; count++)
            {
                if (deltaX < 0)
                {
                    deltaX++;
                }
                else if (deltaX > 0)
                {
                    deltaX--;
                }

                if (deltaY < 0)
                {
                    deltaY++;
                }
                else if (deltaY > 0)
                {
                    deltaY--;
                }

                Position step = new Position(nextX - deltaX, nextY - deltaY, height);
                if (!region.Contains(step))
                {
                    region = _regionRepository.FromPosition(step);
                }

                mob.WalkingQueue.Points.AddToBack(step);
            }
        }

        public void Pulse(Mob mob)
        {
            var walkingQueue = mob.WalkingQueue;
            Position position = mob.Position;
            int height = position.Height;

            Direction firstDirection = Direction.None;
            Direction secondDirection = Direction.None;

            Position next = walkingQueue.Points.Any() ? walkingQueue.Points.RemoveFromFront() : null;
            if (next != null)
            {
                firstDirection = Direction.Between(position, next);

                if (!_collisionManager.Traversable(position, EntityType.Npc, firstDirection))
                {
                    Clear(mob);
                    firstDirection = Direction.None;
                }
                else
                {
                    walkingQueue.PreviousPoints.AddToBack(next);
                    position = new Position(next.X, next.Y, height);
                    mob.LastDirection = firstDirection;

                    if (walkingQueue.Running && walkingQueue.Points.Any())
                    {
                        next = walkingQueue.Points.RemoveFromFront();
                        if (next != null)
                        {
                            secondDirection = Direction.Between(position, next);

                            if (!_collisionManager.Traversable(position, EntityType.Npc, secondDirection))
                            {
                                Clear(mob);
                                secondDirection = Direction.None;
                            }
                            else
                            {
                                walkingQueue.PreviousPoints.AddToBack(next);
                                position = new Position(next.X, next.Y, height);
                                mob.LastDirection = secondDirection;
                            }
                        }
                    }
                }
            }

            mob.FirstDirection = firstDirection;
            mob.SecondDirection = secondDirection;
            mob.Position = position;
        }
    }
}
