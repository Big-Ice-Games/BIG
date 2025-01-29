﻿namespace BIG
{
    /// <summary>
    /// Because <see cref="Entity"/> is a structure to understand which entity INDEX is not used we need a state.
    /// This state keep track on those indexes to tell us what is going on which our entities.
    /// </summary>
    public enum EntityState : byte
    {
        /// <summary>
        /// Empty mean that index is free to use and create a new entity.
        /// </summary>
        Empty,

        /// <summary>
        /// Alive mean that index is occupied by entity.
        /// </summary>
        Alive,

        /// <summary>
        /// Entities can't be created during <see cref="ECS"/> frame.
        /// Every time we are creating a new entity is marked as IsCreating and is changing to Alive
        /// in BeforeUpdate frame of <see cref="ECS"/>.
        /// </summary>
        IsCreating,

        /// <summary>
        /// Entities can't be destroyed during <see cref="ECS"/> frame.
        /// Every time we are destroying a new entity is marked as IsDestroying and is changing to Empty
        /// in AfterUpdate frame of <see cref="ECS"/>.
        /// </summary>
        IsDestroying
    }

    /// <summary>
    /// Entity is Index based. It mean that we don't need Id. We need to understand on which array index is placed. The same rule is apply for every component.
    /// All components arrays contains also the same indexes.
    /// </summary>
    public struct Entity
    {
        /// <summary>
        /// State of this Index.
        /// </summary>
        public EntityState State;

        /// <summary>
        /// Flag that represents which <see cref="IComponent"/>s are associated which this index.
        /// </summary>
        public ulong Components;
    }
}
