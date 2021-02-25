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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KanbanProjectManagementApp.ViewModels
{
    public class RoadmapConfigurationViewModel : INotifyPropertyChanged
    {
        private const string DefaultProjectName = "Project";
        private const int DefaultPriorityWeight = 0;

        private int numberOfWorkItemsToBeCompleted = 10;
        private ConfigurationMode configurationMode = ConfigurationMode.Simple;
        private readonly RoadmapConfiguration roadmapConfiguration;

        private readonly IAskUserForConfirmationToProceed confirmationAsker;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoadmapConfigurationViewModel(IAskUserForConfirmationToProceed confirmationAsker)
        {
            roadmapConfiguration = new RoadmapConfiguration(new[]
                {
                    new ProjectConfiguration(DefaultProjectName, numberOfWorkItemsToBeCompleted, DefaultPriorityWeight)
                });

            this.confirmationAsker = confirmationAsker ?? throw new ArgumentNullException(nameof(confirmationAsker));
        }

        public int NumberOfWorkItemsToBeCompleted
        {
            get
            {
                return roadmapConfiguration.Projects.Sum(p => p.NumberOfWorkItemsToBeCompleted);
            }
            set
            {
                if (ConfigurationMode == ConfigurationMode.Advanced)
                {
                    throw new InvalidOperationException($"Cannot set a value for '{nameof(NumberOfWorkItemsToBeCompleted)}' when in {ConfigurationMode.Advanced} configuration mode.");
                }

                SetNumberOfWorkItemsToBeCompleted(value);
            }
        }

        private void SetNumberOfWorkItemsToBeCompleted(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Number of work items to be completed must be at least 1.");
            }

            if (value != numberOfWorkItemsToBeCompleted)
            {
                numberOfWorkItemsToBeCompleted = value;

                if (ConfigurationMode == ConfigurationMode.Simple)
                {
                    roadmapConfiguration.Projects.First().NumberOfWorkItemsToBeCompleted = value;
                }

                NotifyNumberOfWorkItemsToBeCompletedChanged();
            }
        }

        public int NumberOfProjects => roadmapConfiguration.Projects.Count;

        // TODO: It's technically possible for other classes to change stuff to this Roadmap
        public RoadmapConfiguration Roadmap => roadmapConfiguration;

        public ConfigurationMode ConfigurationMode
        {
            get => configurationMode;
            private set
            {
                if (configurationMode != value)
                {
                    configurationMode = value;
                    NotifyConfigurationModeChanged();
                }
            }
        }

        public void SwitchToAdvancedConfigurationMode()
        {
            ConfigurationMode = ConfigurationMode.Advanced;
        }

        public void SwitchToSimpleConfigurationMode()
        {
            if (roadmapConfiguration.Projects.Count > 1 &&
                !confirmationAsker.ConfirmToProceed("Switching to 'Simple Mode' will cause all projects to be flattened into one. Do you want to continue?"))
            {
                return;
            }

            FlattenProjectsIntoOne();
            ConfigurationMode = ConfigurationMode.Simple;
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="newProjects"/> is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="newProjects"/> doesn't contain
        /// exactly 1 project when <see cref="ConfigurationMode"/> equals to <see cref="ConfigurationMode.Simple"/>.</exception>
        public void ResetRoadmap(IReadOnlyCollection<ProjectConfiguration> newProjects)
        {
            if (ConfigurationMode == ConfigurationMode.Simple)
            {
                if(newProjects.Count != 1)
                {
                    throw new InvalidOperationException("When in simple mode, can only reset using a single project.");
                }

                roadmapConfiguration.Projects.First().NumberOfWorkItemsToBeCompleted = newProjects.First().NumberOfWorkItemsToBeCompleted;
                NotifyNumberOfWorkItemsToBeCompletedChanged();
            }
            else
            {
                try
                {
                    roadmapConfiguration.Projects = newProjects;
                    NotifyNumberOfProjectsChanged();
                    SetNumberOfWorkItemsToBeCompleted(newProjects.Sum(p => p.NumberOfWorkItemsToBeCompleted));
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(
                        "Requires at least one project when resetting a Roadmap.",
                        nameof(newProjects),
                        ex);
                }
            }
        }

        private void FlattenProjectsIntoOne()
        {
            if (roadmapConfiguration.Projects.Count > 1)
            {
                ResetRoadmap(new[]
                {
                    new ProjectConfiguration(
                        DefaultProjectName,
                        roadmapConfiguration.Projects.Sum(p => p.NumberOfWorkItemsToBeCompleted),
                        DefaultPriorityWeight)
                });
            }
        }

        private void NotifyNumberOfWorkItemsToBeCompletedChanged() =>
            NotifyPropertyChange(nameof(NumberOfWorkItemsToBeCompleted));

        private void NotifyNumberOfProjectsChanged() =>
            NotifyPropertyChange(nameof(NumberOfProjects));

        private void NotifyConfigurationModeChanged() =>
            NotifyPropertyChange(nameof(ConfigurationMode));

        private void NotifyPropertyChange(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public enum ConfigurationMode
    {
        Simple,
        Advanced
    }
}
