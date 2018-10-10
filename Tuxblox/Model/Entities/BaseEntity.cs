using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tuxblox.Model.Entities
{
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of the specified object and signals property changed for this object.
        /// </summary>
        /// <param name="objectToUpdate">Object to set value for.</param>
        /// <param name="value">Value to set for object.</param>
        /// <param name="propertyName">Name of the property to update.</param>
        public void SetValue<T>(ref T objectToUpdate, T value, [CallerMemberName] string propertyName = "")
        {
            objectToUpdate = value;
            NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Signals property changed for the specified property name.
        /// </summary>
        /// <param name="propertyName">Property name to signal property changed event for.</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
