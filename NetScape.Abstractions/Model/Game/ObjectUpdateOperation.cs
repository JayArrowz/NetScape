﻿using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Area;

namespace NetScape.Abstractions.Model.Game
{
    public class ObjectUpdateOperation : UpdateOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUpdateOperation"/> class.
        /// </summary>
        /// <param name="region">The region which the ObjectUpdateOperation occured. Must not be <c>null</c>.</param>
        /// <param name="type">The type.</param>
        /// <param name="obj">The object.</param>
        public ObjectUpdateOperation(Region region, EntityUpdateType type, GameObject obj) : base(region, type, obj)
        {
        }

        protected override RegionUpdateMessage Add(int offset)
        {
            return null;
            //return new SendObjectMessage(entity, offset);
        }

        protected override RegionUpdateMessage Remove(int offset)
        {
            return null;
         //   return new RemoveObjectMessage(entity, offset);
        }
    }
}
