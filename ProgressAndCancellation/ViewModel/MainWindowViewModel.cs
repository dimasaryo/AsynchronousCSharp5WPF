using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProgressAndCancellation
{
    /// <summary>
    /// MainWindowViewModel class. Inherited from ViewModelBase
    /// </summary>
    class MainWindowViewModel : ViewModelBase
    {
        private CancellationTokenSource _cts;
        private CancellationToken _token;
        private IProgress<int> _progress;
        private int _currentProgress;

        private bool _canExecuteProcess;
        private bool _canExecuteCancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialization
            _progress = new Progress<int>( ReportProgress );
            _canExecuteCancel = false;
            _canExecuteProcess = true;
        }


        /// <summary>
        /// Gets or sets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public int CurrentProgress
        {
            get
            {
                // return the current progress
                return _currentProgress;
            }
            set
            {
                // validate the given value
                if( _currentProgress != value )
                {
                    _currentProgress = value;
                    OnPropertyChanged( "CurrentProgress" );
                }
            }
        }

        /// <summary>
        /// Process Command
        /// </summary>
        public ICommand ProcessCommand
        {
            get
            {
                // return the Process relay command
                return new RelayCommand( async () =>
                {

                    // Enables the Cancel button and Disable Process button
                    _canExecuteProcess = false;
                    _canExecuteCancel = true;

                    // Initialize CancellationTokenSource and CancellationToken
                    _cts = new CancellationTokenSource();
                    _token = _cts.Token;

                    // Execute and wait for long process
                    await LongProcess();

                }, CanExecuteProcess );
            }
        }

        /// <summary>
        /// Cancel Command
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                // return the Cancel relay command
                return new RelayCommand( Cancel, CanExecuteCancel );
            }
        }

        /// <summary>
        /// Determine if the Process can be Executed.
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteProcess()
        {
            // Return the process button state
            return _canExecuteProcess;
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void Cancel()
        {
            // Cancel the asynchronous process
            _cts.Cancel();
        }

        /// <summary>
        /// Determines whether this instance [can execute cancel].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute cancel]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteCancel()
        {
            // Return the Cancel button state
            return _canExecuteCancel;
        }

        /// <summary>
        /// Asynchronous long the process.
        /// </summary>
        /// <returns></returns>
        public async Task LongProcess()
        {
            try
            {
                await Task.Run( () =>
                {
                    // Were the task already cancelled?
                    _token.ThrowIfCancellationRequested();

                    // Simple loop to show the progress
                    for( int i = 1; i <= 10; i++ )
                    {
                        // Always check if the process was cancelled
                        if( _token.IsCancellationRequested )
                        {
                            // Break the process and set the Cancel flag as true
                            _token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            // Sleep for 500ms to show the changes
                            Thread.Sleep( 500 );

                            // Validate the progress
                            if( _progress != null )
                            {
                                // Report the progress
                                _progress.Report( ( i * 10 ) );
                            }
                        }
                    }
                }, _token );

                // Enable Process button and disable Cancel button
                _canExecuteProcess = true;
                _canExecuteCancel = false;

                // Reset the progress bar.
                _progress.Report( 0 );
                MessageBox.Show( "Long process completed" );
            }
            catch( OperationCanceledException e )
            {
                // Clean up
                _cts.Dispose();

                // Change the button state
                _canExecuteProcess = true;
                _canExecuteCancel = false;

                // Reset the progress bar.
                _progress.Report( 0 );
                MessageBox.Show( "Process cancelled" );
            }
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ReportProgress( int value )
        {
            // Update the UI to reflect the progress value that is passed back
            CurrentProgress = value;
        }
    }
}
