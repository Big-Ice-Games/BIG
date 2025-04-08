// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS8625

namespace BIG
{
    /// <summary>
    /// Store actions and coroutines that can be executed in main game loop.
    /// </summary>
    public sealed class MainThreadActionsQueue : IDisposable
    {
        private readonly Queue<Action> _threadedActions = new Queue<Action>(48);
        private readonly Queue<IEnumerator> _coroutines = new Queue<IEnumerator>(24);
        
        public void Enqueue(Action action)
        {
            lock (_threadedActions)
            {
                _threadedActions.Enqueue(action);
            }
        }

        public bool Dequeue(out Action result)
        {
            lock (_threadedActions)
            {
                if (_threadedActions.Count > 0)
                {
                    result = _threadedActions.Dequeue();
                    return true;
                }
            }

            result = null;
            return false;
        }

        public void Enqueue(IEnumerator coroutine)
        {
            lock (_coroutines)
            {
                _coroutines.Enqueue(coroutine);
            }
        }

        public bool Dequeue(out IEnumerator result)
        {
            lock (_coroutines)
            {
                if (_coroutines.Count > 0)
                {
                    result = _coroutines.Dequeue();
                    return true;
                }
            }

            result = null;
            return false;
        }

        public void Dispose()
        {
            this.Log("Disposing...", LogLevel.Info);
            _coroutines.Clear();
            _threadedActions.Clear();
        }
    }
}
