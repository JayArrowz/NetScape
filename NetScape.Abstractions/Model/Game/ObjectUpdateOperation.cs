using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Area;
using NetScape.Abstractions.Model.Area.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Game
{
    public class ObjectUpdateOperation : UpdateOperation
    {

        /**
	     * Creates the ObjectUpdateOperation.
	     *
	     * @param region The {@link Region} in which the ObjectUpdateOperation occurred. Must not be {@code null}.
	     * @param type The {@link EntityUpdateType}. Must not be {@code null}.
	     * @param object The {@linkGameObject}. Must not be {@code null}.
	     */
        public ObjectUpdateOperation(Region region, EntityUpdateType type, GameObject obj) : base(region, type, obj)
        {
        }

        protected override RegionUpdateMessage Add(int offset)
        {
            return new SendObjectMessage(entity, offset);
        }

        protected override RegionUpdateMessage Remove(int offset)
        {
            return new RemoveObjectMessage(entity, offset);
        }
    }
}
