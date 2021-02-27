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
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using static KanbanProjectManagementApp.Application.RoadmapConfigurations.RoadmapConfigurator;

namespace KanbanProjectManagementApp.ViewModels
{
    public class RoadmapConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly RoadmapConfigurator configurator;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoadmapConfigurationViewModel(IAskUserForConfirmationToProceed confirmationAsker)
        {
            configurator = new RoadmapConfigurator(confirmationAsker);
        }

        public int TotalNumberOfWorkItemsToBeCompleted
        {
            get
            {
                return configurator.TotalNumberOfWorkItemsToBeCompleted;
            }
            set
            {
                if (value != configurator.TotalNumberOfWorkItemsToBeCompleted)
                {
                    configurator.TotalNumberOfWorkItemsToBeCompleted = value;
                    NotifyNumberOfWorkItemsToBeCompletedChanged();
                }
            }
        }

        public int NumberOfProjects => configurator.NumberOfProjects;
        public RoadmapConfiguration Roadmap => configurator.Configuration;
        public ConfigurationMode ConfigurationMode => configurator.Mode;

        public void SwitchToAdvancedConfigurationMode()
        {
            if (configurator.SwitchToAdvancedConfigurationMode())
            {
                NotifyConfigurationModeChanged();
            }
        }

        public void SwitchToSimpleConfigurationMode()
        {
            var originalNumberOfProjects = configurator.NumberOfProjects;
            if (configurator.SwitchToSimpleConfigurationMode())
            {
                if(originalNumberOfProjects != configurator.NumberOfProjects)
                {
                    NotifyNumberOfProjectsChanged();
                }

                NotifyConfigurationModeChanged();
            }
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="newProjects"/> is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="newProjects"/> doesn't contain
        /// exactly 1 project when <see cref="ConfigurationMode"/> equals to <see cref="Application.RoadmapConfigurations.ConfigurationMode.Simple"/>.</exception>
        public void ResetRoadmap(IReadOnlyCollection<ProjectConfiguration> newProjects)
        {
            var originalNumberOfProjects = configurator.NumberOfProjects;
            var originalNumberOfWorkItems = configurator.TotalNumberOfWorkItemsToBeCompleted;

            configurator.ResetRoadmap(newProjects);

            if(originalNumberOfWorkItems != configurator.TotalNumberOfWorkItemsToBeCompleted)
            {
                NotifyNumberOfWorkItemsToBeCompletedChanged();
            }

            if (originalNumberOfProjects != configurator.NumberOfProjects)
            {
                NotifyNumberOfProjectsChanged();
            }
        }

        private void NotifyNumberOfWorkItemsToBeCompletedChanged() =>
            NotifyPropertyChange(nameof(TotalNumberOfWorkItemsToBeCompleted));

        private void NotifyNumberOfProjectsChanged() =>
            NotifyPropertyChange(nameof(NumberOfProjects));

        private void NotifyConfigurationModeChanged() =>
            NotifyPropertyChange(nameof(ConfigurationMode));

        private void NotifyPropertyChange(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
