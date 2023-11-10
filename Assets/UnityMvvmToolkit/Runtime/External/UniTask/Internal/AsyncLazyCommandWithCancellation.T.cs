﻿#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Internal
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    internal class AsyncLazyCommandWithCancellation<T> : BaseAsyncCommand, IAsyncCommand<T>
    {
        private readonly IAsyncCommand<T> _asyncCommand;
        private CancellationTokenSource _cancellationTokenSource;

        public AsyncLazyCommandWithCancellation(IAsyncCommand<T> asyncCommand) : base(null)
        {
            _asyncCommand = asyncCommand;
        }

        public override bool DisableOnExecution
        {
            get => _asyncCommand.DisableOnExecution;
            set => _asyncCommand.DisableOnExecution = value;
        }

        public override event EventHandler<bool> CanExecuteChanged
        {
            add => _asyncCommand.CanExecuteChanged += value;
            remove => _asyncCommand.CanExecuteChanged -= value;
        }

        public void Execute(T parameter)
        {
            if (IsCommandRunning)
            {
                return;
            }

            ExecuteAsync(parameter).Forget();
        }

        public async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                SetCommandRunning(true);

                await _asyncCommand.ExecuteAsync(parameter, _cancellationTokenSource.Token);
            }
            finally
            {
                SetCommandRunning(false);

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public override void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}

#endif