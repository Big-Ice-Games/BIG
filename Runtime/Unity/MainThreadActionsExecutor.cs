#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections;

namespace BIG
{
    /// <summary>
    /// This executor is created automatically by <see cref="GameInitializer"/>.
    /// It handles all actions and coroutines that are queued in <see cref="MainThreadActionsQueue"/>.
    /// </summary>
    public class MainThreadActionExecutor : BaseBehaviour
    {
        [Inject] private MainThreadActionsQueue _mainThreadActionsQueue;

        private Action _action;
        private IEnumerator _iEnumerator;

        private void Update()
        {
            try
            {
                while (_mainThreadActionsQueue.Dequeue(out _action))
                {
                    _action?.Invoke();
                }
            
                while (_mainThreadActionsQueue.Dequeue(out _iEnumerator))
                {
                    StartCoroutine(_iEnumerator);
                }
            }
            catch (Exception e)
            {
                Log($"Update exception: {e}", LogLevel.Error);
            }
        }
    }
}