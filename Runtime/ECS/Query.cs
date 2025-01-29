using System;
using System.Collections.Generic;

namespace BIG
{
    /// <summary>
    /// We can define queries in context. It stores all flags required by entity and flags excluded for this query.
    /// For example
    /// Query(ComponentType.Input | ComponentType.Transform, ComponentType.AI)
    /// Gives us a Query that is going to store all entities that have Input and Transform but doesn't have AI Component.
    /// When we call
    /// Context.AddComponent(int entity, ComponentType)
    /// Context.RemoveComponent(int entity, ComponentType)
    /// Context.AddEntity(param[] IComponent)
    /// Context.RemoveEntity(int entity)
    /// ...
    /// All Queries attached to this context are evaluated.
    /// </summary>
    public readonly struct Query
    {
        public readonly ulong RequiredFlags;
        public readonly ulong ExcludedFlags;
        public readonly HashSet<int> Entities;

        /// <summary>
        /// Creates query to store all entities that are valid for those filters.
        /// </summary>
        /// <param name="requiredFlags"></param>
        /// <param name="excludedFlags"></param>
        /// <param name="capacity"></param>
        public Query(ulong requiredFlags, ulong excludedFlags, int capacity)
        {
            RequiredFlags = requiredFlags;
            ExcludedFlags = excludedFlags;
            Entities = new HashSet<int>(capacity);
        }

        public Query(Tuple<ulong, ulong> flags, int capacity) : this(flags.Item1, flags.Item2, capacity) { }

        public void ValidateEntityForQuery(int entityIndex, ulong entityComponents)
        {
            bool validForQuery = (entityComponents & RequiredFlags) == RequiredFlags &&
                                 (entityComponents & ExcludedFlags) == 0;
            if (validForQuery)
            {
                Entities.Add(entityIndex);
            }
            else
            {
                Entities.Remove(entityIndex);
            }
        }

        public void OnEntityRemoved(int entityIndex)
        {
            Entities.Remove(entityIndex);
        }
    }
}
