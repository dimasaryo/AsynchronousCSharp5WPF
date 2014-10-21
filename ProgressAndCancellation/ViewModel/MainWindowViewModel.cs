using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProgressAndCancellation
{
    class MainWindowViewModel : ViewModelBase
    {
        private CancellationTokenSource _cts;
        private CancellationToken _token;
        private IProgress<int> _progress;
        private int _currentProgress;

        public MainWindowViewModel()
        {           
            _progress = new Progress<int>( ReportProgress );
        }

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
                return new RelayCommand( async () =>{

                    _cts = new CancellationTokenSource();
                    _token = _cts.Token;
                    await LongProcess();

                } , CanExecuteProcess );
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
            return true;
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void Cancel()
        {
            // Cancel the asynchronous process
            _cts.Cancel();
        }

        private bool CanExecuteCancel()
        {
            return true;
        }

        public async Task LongProcess()
        {
            try
            {
                await Task.Run( () =>
                {
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

                            if( _progress != null )
                            {
                                _progress.Report( ( i * 10 ) );
                            }
                        }
                    }
                }, _token );

                // Reset the progress bar.
                _progress.Report( 0 );
                MessageBox.Show( "Long process completed" );
            }
            catch( OperationCanceledException e )
            {
                // Clean up
                _cts.Dispose();

                // Reset the progress bar.
                _progress.Report( 0 );
                MessageBox.Show( "Process cancelled" );
            }      
        }

        private void ReportProgress( int value )
        {
            // Update the UI to reflect the progress value that is passed back
            CurrentProgress = value;
        }
    }
}
