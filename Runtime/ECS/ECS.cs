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
        public Action<int> OnEntityDestroyed;

        private readonly Dictionary<ulong, IComponent[]> _components;

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
            #region Allocate components arrays
            _components = new Dictionary<ulong, IComponent[]>(64);
            _components.Add(0, new IComponent[_capacity]);
            for (int i = 0; i < 64; i++) // Check IComponent documentation.
                _components.Add(1UL << i, new IComponent[_capacity]);
            #endregion

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

        public int CreateEntity(params IComponent[] components)
        {
            lock (_entities)
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_entities[i].State != EntityState.Empty) continue;

                    _entities[i].State = EntityState.IsCreating;
                    _entities[i].Components = components.ToFlag();

                    // Override all components on this entity index.
                    for (int j = 0; j < components.Length; j++)
                    {
                        var component = components[j];
                        _components[component.ComponentType][i] = component;
                    }

                    _creating.Add(i);
                }
            }

            throw new IndexOutOfRangeException("ECS capacity reached.");
        }

        public void DestroyEntity(int index)
        {
            lock (_entities)
            {
                _entities[index].State = EntityState.IsDestroying;
            }
        }

        public void AddComponent(int index, IComponent component)
        {
            lock (_entities)
            {
                var entity = _entities[index];
                if (entity.Components.Have(component)) return;

                // Add component flag.
                entity.Components = entity.Components |= component.ComponentType;
                _entities[index] = entity;
                ValidateForQueries(index, entity);
            }

            lock (_components)
            {
                _components[component.ComponentType][index] = component;
            }
        }

        public void RemoveComponent(int index, IComponent component)
        {
            lock (_entities)
            {
                var entity = _entities[index];
                if (!entity.Components.Have(component)) return;

                // Remove component flag.
                entity.Components = entity.Components &= ~component.ComponentType;
                _entities[index] = entity;
                ValidateForQueries(index, entity);
            }
        }

        #endregion

        private void BeforeUpdate()
        {
            HandleEntitiesDestroy();
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
            lock (_queries)
            {
                for (int i = 0; i < _queries.Length; i++)
                {
                    var query = _queries[i];
                    query.ValidateEntityForQuery(index, entity.Components);
                    _queries[i] = query;
                }
            }
        }

        #endregion

        #region Delete Entity
        private void HandleEntitiesDestroy()
        {
            lock (_entities)
            {
                for (int i = 0; i < _entities.Length; i++)
                {
                    if (_entities[i].State == EntityState.IsDestroying)
                    {
                        var entity = _entities[i];
                        entity.State = EntityState.Empty;
                        _entities[i] = entity;
                        RemoveFromQueries(in i);
                        OnEntityDestroyed?.Invoke(i);
                    }
                }
            }
        }
        private void RemoveFromQueries(in int index)
        {
            lock (_queries)
            {
                for (int i = 0; i < _queries.Length; i++)
                {
                    var query = _queries[i];
                    query.OnEntityRemoved(index);
                    _queries[i] = query;
                }
            }
        }

        #endregion
    }
}
