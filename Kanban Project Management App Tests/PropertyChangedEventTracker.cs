using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    internal sealed class PropertyChangedEventTracker : IDisposable
    {
        private readonly IList<PropertyChangedEventArgs> recordedPropertyChangedEvents = new List<PropertyChangedEventArgs>();
        private readonly INotifyPropertyChanged objectToBeTracked;

        /// <exception cref="ArgumentNullException"/>
        public PropertyChangedEventTracker(INotifyPropertyChanged objectToBeTracked)
        {
            this.objectToBeTracked = objectToBeTracked ?? throw new ArgumentNullException(nameof(objectToBeTracked));
            objectToBeTracked.PropertyChanged += RecordPropertyChangeEvent;
        }

        public void Dispose()
        {
            objectToBeTracked.PropertyChanged -= RecordPropertyChangeEvent;
        }

        public void ClearAllRecordedEvents() => recordedPropertyChangedEvents.Clear();

        /// <exception cref="XUnitException">Thrown if any assertion failed.</exception>
        public void AssertOnlyOnePropertyChangeNotificationHappenedForName(string propertyName)
        {
            Assert.Single(recordedPropertyChangedEvents);
            Assert.Equal(propertyName, recordedPropertyChangedEvents[0].PropertyName);
        }

        private void RecordPropertyChangeEvent(object s, PropertyChangedEventArgs e) => recordedPropertyChangedEvents.Add(e);
    }
}
