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
using System.ComponentModel;

namespace KanbanProjectManagementApp.ViewModels
{
    public class RoadmapConfigurationViewModel : INotifyPropertyChanged
    {
        private int numberOfWorkItemsToBeCompleted = 10;

        public event PropertyChangedEventHandler PropertyChanged;

        public int NumberOfWorkItemsToBeCompleted
        {
            get
            {
                return numberOfWorkItemsToBeCompleted;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Number of work items to be completed must be at least 1.");
                }

                if(value != numberOfWorkItemsToBeCompleted)
                {
                    numberOfWorkItemsToBeCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberOfWorkItemsToBeCompleted)));
                }
            }
        }

        public int NumberOfProjects
        {
            get
            {
                return 1;
            }
        }
    }
}
