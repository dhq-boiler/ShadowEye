using System;
using System.Collections.Generic;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace ShadowEye.Model
{
    internal class InitialAction : IDisposable
    {
        private CompositeDisposable _disposables = new();
        private static readonly HashSet<Guid> set = new();
        private Guid _id;
        private Func<IDisposable> value;
        private bool disposedValue;

        public static void Run(Func<IDisposable> value)
        {
            new InitialAction(value);
        }

        private InitialAction(Func<IDisposable> value)
        {
            this.value = value;
            _id = Guid.NewGuid();
            if (set.Add(_id))
            {
                value().AddTo(_disposables);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}