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
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.Application.RoadmapConfigurations
{
    public class ProjectConfiguration
    {
        private int numberOfWorkItemsToBeCompleted;

        /// <exception cref="ArgumentException">Thrown when any of the arguments are invalid.</exception>
        public ProjectConfiguration(
            string name,
            int numberOfWorkItemsToBeCompleted,
            int priorityWeight)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name must be specified", nameof(name));
            if (numberOfWorkItemsToBeCompleted <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfWorkItemsToBeCompleted), "Project must have at least 1 work item to be completed.");

            Name = name;
            NumberOfWorkItemsToBeCompleted = numberOfWorkItemsToBeCompleted;
            PriorityWeight = priorityWeight;
        }

        public int NumberOfWorkItemsToBeCompleted
        {
            get => numberOfWorkItemsToBeCompleted;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Project must have at least 1 work item to be completed.");
                }
                numberOfWorkItemsToBeCompleted = value;
            }
        }

        public string Name { get; private set; }
        /// <summary>
        /// Represents the priority. The higher the value, the more priority it gets in being worked on.
        /// </summary>
        public int PriorityWeight { get; private set; }

        public Project ToWorkableProject() => new Project(NumberOfWorkItemsToBeCompleted, PriorityWeight, Name);
    }
}
