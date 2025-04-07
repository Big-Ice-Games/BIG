using System;
using UnityEngine;

namespace BIG
{
    public abstract class BaseBehaviour : MonoBehaviour, IDisposable
    {
        private bool _disposed; // Avoid disposing object more than once.
        private bool _initiated = false; // Avoid injecting dependencies more than once.
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
        public void RaiseEvent<T>(T eventData) where T : struct => Events.Raise(eventData);
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
        
        public void Log( string message, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false, bool withTime = false)
        {
            Logger.Log(this, message, logLevel, withStackTrace, withTime);
        }
    }
}
