﻿// Copyright (c) 2020 S.L. des Bouvrie
//
// This file is part of 'Kanban Project Management App'.
//
// Kanban Project Management App is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Kanban Project Management App is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Kanban Project Management App.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.TestUtilities
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
            Assert.Single(recordedPropertyChangedEvents, e => e.PropertyName == propertyName);
        }

        public void AssertPropertyChangeNotificationHappenedGivenNumberOfTimesForName(int expectedNumberOfTimes, string propertyName)
        {
            if (expectedNumberOfTimes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedNumberOfTimes), "Value must be 0 or greater.");
            }

            Assert.Equal(expectedNumberOfTimes, recordedPropertyChangedEvents.Count(e => e.PropertyName == propertyName));
        }

        internal void AssertNoPropertyChangeNotificationHappenedForName(string propertyName)
        {
            Assert.Empty(recordedPropertyChangedEvents.Where(e => e.PropertyName == propertyName));
        }

        /// <exception cref="XUnitException">Thrown if any assertion failed.</exception>
        public void AssertNoPropertyChangeNotificationsHappened()
        {
            Assert.Empty(recordedPropertyChangedEvents);
        }

        private void RecordPropertyChangeEvent(object s, PropertyChangedEventArgs e) => recordedPropertyChangedEvents.Add(e);
    }
}
