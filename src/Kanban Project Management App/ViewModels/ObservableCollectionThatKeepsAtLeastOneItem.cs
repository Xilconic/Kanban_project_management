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

namespace KanbanProjectManagementApp.ViewModels
{
    internal class ObservableCollectionThatKeepsAtLeastOneItem<T> : ObservableCollection<T>
    {
        private readonly string elementDescription;

        public ObservableCollectionThatKeepsAtLeastOneItem(
            IEnumerable<T> collection,
            string elementDescription) : base(collection)
        {
            this.elementDescription = elementDescription;
        }

        protected override void RemoveItem(int index)
        {
            if (Count > 1)
            {
                base.RemoveItem(index);
            }
            else
            {
                throw CreateGuardAgainstDeletingLastElementException();
            }
        }

        protected override void ClearItems()
        {
            throw CreateGuardAgainstDeletingLastElementException();
        }

        private InvalidOperationException CreateGuardAgainstDeletingLastElementException() =>
            new InvalidOperationException($"Cannot delete last {elementDescription}.");
    }
}
