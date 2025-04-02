#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS8625

namespace BIG
{
    public class DelayedAction
    {
        public Action Action;
        public float TimeToGo;
        public DelayedAction(Action action, float timeToGo)
        {
            Action = action;
            TimeToGo = timeToGo;
        }
    }
    /// <summary>
    /// Store actions and coroutines that can be executed in main game loop.
    /// </summary>
    [Register(true)]
    public sealed class MainThreadActionsQueue : IDisposable
    {
        private readonly Queue<Action> _threadedActions = new Queue<Action>(48);
        private readonly Queue<IEnumerator> _coroutines = new Queue<IEnumerator>(24);
        private readonly DelayedAction[] _delayedActions = new DelayedAction[24];

        public void Enqueue(DelayedAction action)
        {
            lock (_delayedActions)
            {
                for (int i = 0; i < _delayedActions.Length; i++)
                {
                    if (_delayedActions[i] == null)
                    {
                        _delayedActions[i] = action;
                        return;
                    }
                }
            }
        }

        public void TickDelayedActions(float time)
        {
            lock (_delayedActions)
            {
                for (int i = 0; i < _delayedActions.Length; i++)
                {
                    var act = _delayedActions[i];
                    if (act != null)
                    {
                        if ((act.TimeToGo -= time) <= 0)
                        {
                            act.Action?.Invoke();
                            _delayedActions[i] = null;
                        }
                    }
                }
            }
        }

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
            _coroutines.Clear();
            _threadedActions.Clear();
            _delayedActions.Clear();
        }
    }
}
