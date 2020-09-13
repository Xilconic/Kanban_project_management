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
using KanbanProjectManagementApp.Domain;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KanbanProjectManagementApp.Views
{
    /// <summary>
    /// Interaction logic for ProjectsEditorWindow.xaml
    /// </summary>
    public partial class ProjectsEditorWindow : Window
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
        private int numberOfWorkItemsToBeCompleted;

        public string Name { get; set; } = "Project";
        public int NumberOfWorkItemsToBeCompleted
        {
            get => numberOfWorkItemsToBeCompleted;
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Number of work items should be at least 1.");
                }

                numberOfWorkItemsToBeCompleted = value;
            }
        }

        public static ProjectRowItem FromDomain(Project project) =>
            new ProjectRowItem
            {
                NumberOfWorkItemsToBeCompleted = project.NumberOfWorkItemsRemaining,
            };

        public Project ToDomain() =>
            new Project(NumberOfWorkItemsToBeCompleted);
    }


}
