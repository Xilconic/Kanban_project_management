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
using KanbanProjectManagementApp.Application;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;

namespace KanbanProjectManagementApp.Views
{
    /// <summary>
    /// Interaction logic for ProjectsEditorWindow.xaml
    /// </summary>
    public partial class ProjectsEditorWindow
    {
        public ProjectsEditorWindow(ObservableCollection<ProjectRowItem> projectsRowItems)
        {
            InitializeComponent();
            ProjectsDataGrid.ItemsSource = projectsRowItems;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }

    public class ProjectRowItem
    {
        private int numberOfWorkItemsToBeCompleted = 1;
        private string name = "Project";

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name must be specified");
                }

                name = value;
            }
        }

        public int NumberOfWorkItemsToBeCompleted
        {
            get => numberOfWorkItemsToBeCompleted;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Number of work items should be at least 1.");
                }

                numberOfWorkItemsToBeCompleted = value;
            }
        }

        public int PriorityWeight { get; set; }

        public static ProjectRowItem FromDomain(ProjectConfiguration projectConfiguration) =>
            new ProjectRowItem
            {
                NumberOfWorkItemsToBeCompleted = projectConfiguration.NumberOfWorkItemsToBeCompleted,
                Name = projectConfiguration.Name,
                PriorityWeight = projectConfiguration.PriorityWeight,
            };

        public ProjectConfiguration ToConfiguration() =>
            new ProjectConfiguration(Name, NumberOfWorkItemsToBeCompleted, PriorityWeight);
    }
}
