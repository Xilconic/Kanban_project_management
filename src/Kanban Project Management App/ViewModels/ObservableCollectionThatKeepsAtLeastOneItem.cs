// Copyright (c) 2020 S.L. des Bouvrie
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace KanbanProjectManagementApp.ViewModels
{
    class ObservableCollectionThatKeepsAtLeastOneItem<T> : ObservableCollection<T>
    {
        private readonly string elementDescription;

        public ObservableCollectionThatKeepsAtLeastOneItem(
            IEnumerable<T> elements,
            string elementDescription) : base(elements)
        {
            this.elementDescription = elementDescription;
        }

        public bool AreCollectionChangesAllowed { get; set; } = true;

        /// <exception cref="ArgumentException">Thrown when <paramref name="newElements"/> is empty.</exception>
        public void ResetElements(IEnumerable<T> newElements)
        {
            ThrowIfCollectionChangesAreNotAllowed();

            if (!newElements.Any())
            {
                throw new ArgumentException(
                    $"Requires at least one {elementDescription} when resetting.",
                    nameof(newElements));
            }

            Items.Clear();
            foreach (T newElement in newElements)
            {
                Items.Add(newElement);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void InsertItem(int index, T item)
        {
            ThrowIfCollectionChangesAreNotAllowed();

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfCollectionChangesAreNotAllowed();

            if (Count > 1)
            {
                base.RemoveItem(index);
            }
            else
            {
                throw CreateGuardAgaintDeletingLastElementException();
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            ThrowIfCollectionChangesAreNotAllowed();

            base.MoveItem(oldIndex, newIndex);
        }

        protected override void SetItem(int index, T item)
        {
            ThrowIfCollectionChangesAreNotAllowed();

            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            ThrowIfCollectionChangesAreNotAllowed();

            throw CreateGuardAgaintDeletingLastElementException();
        }

        private InvalidOperationException CreateGuardAgaintDeletingLastElementException() =>
            new InvalidOperationException($"Cannot delete last {elementDescription}.");

        private void ThrowIfCollectionChangesAreNotAllowed()
        {
            if (!AreCollectionChangesAllowed)
            {
                throw new InvalidOperationException("Changes are not allowed.");
            }
        }
    }
}
