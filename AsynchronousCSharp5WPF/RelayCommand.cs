using System;
using System.Windows.Input;

namespace AsynchronousCSharp5WPF
{
    /// <summary>
    /// Relay Command
    /// </summary>
    class RelayCommand : ICommand
    {
        #region Fields

        readonly Func<Boolean> _canExecute;
        readonly Action _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Action</param>
        /// <param name="canExecute">Command State</param>
        public RelayCommand( Action execute, Func<Boolean> canExecute )
        {
            // Validate the given action value
            if( execute == null )
            {
                // throw ArgumentNullException if the given action is null
                throw new ArgumentNullException( "execute" );
            }

            // set the action and command state from the given value
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Can Executed Changed Event Handler
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {

                if( _canExecute != null )
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {

                if( _canExecute != null )
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// Can Execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Boolean CanExecute( Object parameter )
        {
            return _canExecute == null ? true : _canExecute();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute( Object parameter )
        {
            _execute();
        }

        #endregion
    }
}
