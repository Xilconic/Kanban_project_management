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
using System.Collections.Specialized;
using System.Linq;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.TestUtilities
{
    internal sealed class CollectionChangedEventTracker : IDisposable
    {
        private readonly IList<NotifyCollectionChangedEventArgs> recordedCollectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
        private readonly INotifyCollectionChanged objectToBeTracked;

        public CollectionChangedEventTracker(INotifyCollectionChanged objectToBeTracked)
        {
            this.objectToBeTracked = objectToBeTracked ?? throw new ArgumentNullException(nameof(objectToBeTracked));
            this.objectToBeTracked.CollectionChanged += RecordCollectionChangeEvent;
        }

        public void Dispose()
        {
            objectToBeTracked.CollectionChanged -= RecordCollectionChangeEvent;
        }

        public void ClearAllRecordedEvents() => recordedCollectionChangedEvents.Clear();

        /// <exception cref="XUnitException">Thrown if any assertion failed.</exception>
        public void AssertCollectionChangeNotificationsHappenedForAction(
            NotifyCollectionChangedAction collectionChangeAction,
            int numberOfExpectedEvents)
        {
            Assert.Equal(numberOfExpectedEvents, recordedCollectionChangedEvents.Count(e => e.Action == collectionChangeAction));
        }

        /// <exception cref="XUnitException">Thrown if any assertion failed.</exception>
        public void AssertNoCollectionChangeNotificationsHappened()
        {
            Assert.Empty(recordedCollectionChangedEvents);
        }

        private void RecordCollectionChangeEvent(object sender, NotifyCollectionChangedEventArgs e) =>
            recordedCollectionChangedEvents.Add(e);
    }
}
