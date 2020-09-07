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

namespace KanbanProjectManagementApp.Domain
{
    public class Project
    {
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfWorkItemsRemaining"/> is less or equal to 0</exception>
        public Project(int numberOfWorkItemsRemaining)
        {
            if(numberOfWorkItemsRemaining <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfWorkItemsRemaining),
                    "Project should consist out of at least 1 work item.");
            }

            NumberOfWorkItemsRemaining = numberOfWorkItemsRemaining;
        }

        public bool HasWorkToBeCompleted => NumberOfWorkItemsRemaining > 0;

        public int NumberOfWorkItemsRemaining { get; private set; }

        /// <exception cref="InvalidOperationException">Thrown when <see cref="HasWorkToBeCompleted"/> is false.</exception>
        public void CompleteWorkItem()
        {
            if (!HasWorkToBeCompleted)
            {
                throw new InvalidOperationException("There is no more work to be completed.");
            }

            NumberOfWorkItemsRemaining -= 1;
        }
    }
}
