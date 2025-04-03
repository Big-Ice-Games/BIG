using System;
using System.Collections;
using UnityEngine;

namespace BIG
{
    public abstract class BaseBehaviour : MonoBehaviour, IDisposable
    {
        private bool _disposed;
        private bool _initiated = false;
        protected virtual void Awake()
        {
            if (_initiated) return;
            _initiated = true;
            OnBeforeInjection();
            this.ResolveMyDependencies();
        }

        protected virtual void OnEnable() => this.Subscribe();
        protected virtual void OnDisable() => this.Unsubscribe();
        protected virtual void OnBeforeInjection() {}
        public void OnDestroy() => Dispose();
        public void Dispose()
        {
            if (!_disposed)
            {
                BeforeDispose();
                this.Unsubscribe();
                _disposed = true;
                AfterDispose();
            }
        }
        protected virtual void BeforeDispose() {}
        protected virtual void AfterDispose() {}
    }
}
