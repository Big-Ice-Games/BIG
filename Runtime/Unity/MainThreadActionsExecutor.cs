using System;
using System.Collections;
using BIG;
using UnityEngine;

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