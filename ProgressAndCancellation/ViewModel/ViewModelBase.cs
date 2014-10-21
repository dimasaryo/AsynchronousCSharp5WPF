using System.ComponentModel;

namespace ProgressAndCancellation
{
    /// <summary>
    /// View Model Base. Implements INotifyPropertyChanged
    /// </summary>
    class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Property Changed Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property Changed.
        /// </summary>
        /// <param name="propertyName">Property Name.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            // Get the current property changed event handler
            PropertyChangedEventHandler handler = PropertyChanged;

            // Raise the property changed event when the handler is not null
            if( handler != null )
            {
                handler( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }
    }
}
