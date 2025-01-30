#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BIG
{
    /// <summary>
    /// This class is not registered. It should be resolved by end user.
    /// </summary>
    public sealed class ECS
    {
        private readonly int _capacity;
        private readonly Entity[] _entities;
        private readonly Query[] _queries;
        private readonly ISystem[] _systems;
        private readonly HashSet<int> _creating;

        public Action<int> OnEntityCreated;
        public Action<int> OnEntityDeleted;

        public Entity[] Entities
        {
            get
            {
                lock (_entities)
                {
                    return _entities;
                }
            }
        }

        public ECS(ISettings settings, params ISystem[] systems)
        {
            _capacity = settings.EntitiesCapacity;
            _creating = new HashSet<int>(_capacity);
            _entities = new Entity[_capacity];
            _queries = new Query[systems.Length];
            for (int i = 0; i < systems.Length; i++)
            {
                _queries[i] = new Query(systems[i].Query, _capacity);
            }

            _systems = systems;
        }

        #region Public API

        public void Update(in int frame, in float step)
        {
            BeforeUpdate();

            for (int i = 0; i < _systems.Length; i++)
            {
                _systems[i].Update(frame, step, _queries[i].Entities);
            }

            AfterUpdate();
        }

        /// <summary>
        /// [IMPORTANT]
        /// When you are creating a new entity all components that you are passing here NEEDS TO BE ASSIGNED MANUALLY JUST AFTER THIS CALL on the index taken from result of this function.
        /// <example>
        /// For example:
        /// <code>
        /// int index = _ecs.CreateEntity(transformComponent, physicsComponent);
        /// _transforms[index] = transformComponent;
        /// _physicsComponents[index] = physicsComponent;
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="components">Provide all components that this entity should have.
        /// Remember that AddComponent and RemoveComponent are heavy like CreateEntity and RemoveEntity so try to create entities with all possibly all components that you need.</param>
        /// <returns>Entity index.</returns>
        public int CreateEntity(params IComponent[] components)
        {
            lock (_entities)
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_entities[i].State != EntityState.Empty) continue;

                    _entities[i].State = EntityState.IsCreating;
                    _entities[i].Components = components.ToFlag();
                    _creating.Add(i);
                }
            }

            throw new IndexOutOfRangeException("ECS capacity reached.");
        }

        /// <summary>
        /// As with CreateEntity this component have to be assigned somewhere else.
        /// This functions is just for storing
        /// </summary>
        /// <param name="index"></param>
        /// <param name="component"></param>
        public void AddComponent(int index, IComponent component)
        {

        }

        public void RemoveComponent(int index, IComponent component)
        {

        }

        #endregion

        private void BeforeUpdate()
        {
            CreatePendingEntities();
        }

        private void AfterUpdate()
        {

        }

        #region Create Entity

        private void CreatePendingEntities()
        {
            lock (_entities)
            {
                foreach (int index in _creating)
                {
                    var e = _entities[index];
                    if (e.State != EntityState.IsCreating)
                    {
                        throw new InvalidAsynchronousStateException($"Entity {index} exist in _creating HashSet but it's state is {e.State}");
                    }

                    e.State = EntityState.Alive;
                    ValidateForQueries(index, e);
                    _entities[index] = e;
                    OnEntityCreated?.Invoke(index);
                }

                _creating.Clear();
            }
        }

        /// <summary>
        /// Add entity to all the queries that it match.
        /// </summary>
        /// <param name="index">Entity index.</param>
        /// <param name="entity">Entity itself.</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateForQueries(in int index, in Entity entity)
        {
            foreach (Query query in _queries)
            {
                query.ValidateEntityForQuery(index, entity.Components);
            }
        }

        #endregion
    }
}
