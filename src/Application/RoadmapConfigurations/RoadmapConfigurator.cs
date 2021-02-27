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
using System.Linq;

namespace KanbanProjectManagementApp.Application.RoadmapConfigurations
{
    public class RoadmapConfigurator
    {
        private const string DefaultProjectName = "Project";
        private const int DefaultPriorityWeight = 0;

        private readonly RoadmapConfiguration configuration;
        private readonly IAskUserForConfirmationToProceed confirmationAsker;

        /// <exception cref="ArgumentNullException"/>
        public RoadmapConfigurator(IAskUserForConfirmationToProceed confirmationAsker)
        {
            this.confirmationAsker = confirmationAsker ?? throw new ArgumentNullException(nameof(confirmationAsker));

            var defaultProjectConfigurations = new[]
            {
                new ProjectConfiguration(DefaultProjectName, 10, DefaultPriorityWeight),
            };
            configuration = new RoadmapConfiguration(defaultProjectConfigurations);
        }

        /// <exception cref="InvalidOperationException">Thrown when setting a value while the configuration mode is set to <see cref="ConfigurationMode.Simple"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value less than 1.</exception>
        public int TotalNumberOfWorkItemsToBeCompleted
        {
            get
            {
                return configuration.Projects.Sum(p => p.NumberOfWorkItemsToBeCompleted);
            }
            set
            {
                if (Mode == ConfigurationMode.Advanced)
                {
                    throw new InvalidOperationException($"Cannot set a value for '{nameof(TotalNumberOfWorkItemsToBeCompleted)}' when in {ConfigurationMode.Advanced} configuration mode.");
                }
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Number of work items to be completed must be at least 1.");
                }

                configuration.Projects.First().NumberOfWorkItemsToBeCompleted = value;
            }
        }

        public int NumberOfProjects => configuration.Projects.Count;

        public RoadmapConfiguration Configuration => configuration;

        public ConfigurationMode Mode { get; private set; } = ConfigurationMode.Simple;

        /// <returns>True if the configuration mode was successfully changed. False otherwise.</returns>
        public bool SwitchToAdvancedConfigurationMode()
        {
            if (Mode == ConfigurationMode.Advanced)
            {
                return false;
            }

            Mode = ConfigurationMode.Advanced;
            return true;
        }

        /// <returns>True if the configuration mode was successfully changed. False otherwise.</returns>
        public bool SwitchToSimpleConfigurationMode()
        {
            if (Mode == ConfigurationMode.Simple)
            {
                return false;
            }

            if (configuration.Projects.Count > 1 &&
                !confirmationAsker.ConfirmToProceed("Switching to 'Simple Mode' will cause all projects to be flattened into one. Do you want to continue?"))
            {
                return false;
            }

            FlattenProjectsIntoOne();
            Mode = ConfigurationMode.Simple;
            return true;
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="newProjects"/> is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="newProjects"/> doesn't contain
        /// exactly 1 project when <see cref="ConfigurationMode"/> equals to <see cref="ConfigurationMode.Simple"/>.</exception>
        public void ResetRoadmap(IReadOnlyCollection<ProjectConfiguration> newProjects)
        {
            if (Mode == ConfigurationMode.Simple)
            {
                if (newProjects.Count != 1)
                {
                    throw new InvalidOperationException("When in simple mode, can only reset using a single project.");
                }

                TotalNumberOfWorkItemsToBeCompleted = newProjects.First().NumberOfWorkItemsToBeCompleted;
            }
            else
            {
                try
                {
                    configuration.Projects = newProjects;
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
            if (configuration.Projects.Count > 1)
            {
                ResetRoadmap(new[]
                {
                    new ProjectConfiguration(
                        DefaultProjectName,
                        TotalNumberOfWorkItemsToBeCompleted,
                        DefaultPriorityWeight)
                });
            }
        }

        public enum ConfigurationMode
        {
            Simple,
            Advanced
        }
    }
}
