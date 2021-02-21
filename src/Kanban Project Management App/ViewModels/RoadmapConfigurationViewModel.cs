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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KanbanProjectManagementApp.ViewModels
{
    public class RoadmapConfigurationViewModel : INotifyPropertyChanged
    {
        private int numberOfWorkItemsToBeCompleted = 10;
        private ConfigurationMode configurationMode = ConfigurationMode.Simple;
        private readonly ObservableCollectionThatKeepsAtLeastOneItem<Project> projects;

        private readonly IAskUserForConfirmationToProceed confirmationAsker;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoadmapConfigurationViewModel(IAskUserForConfirmationToProceed confirmationAsker)
        {
            projects = new ObservableCollectionThatKeepsAtLeastOneItem<Project>(
                            new[] { new Project(numberOfWorkItemsToBeCompleted) },
                            "project of Roadmap")
            {
                AreCollectionChangesAllowed = false, // Simple mode does not allow any modification to the collection
            };
            projects.CollectionChanged += Projects_CollectionChanged;
            this.confirmationAsker = confirmationAsker ?? throw new ArgumentNullException(nameof(confirmationAsker));
        }

        public int NumberOfWorkItemsToBeCompleted
        {
            get
            {
                return projects.Sum(p => p.NumberOfWorkItemsRemaining);
            }
            set
            {
                if (ConfigurationMode == ConfigurationMode.Advanced)
                {
                    throw new InvalidOperationException($"Cannot set a value for '{nameof(NumberOfWorkItemsToBeCompleted)}' when in {ConfigurationMode.Advanced} configuration mode.");
                }

                TemporarilyAllowProjectsToBeUpdated(() => SetNumberOfWorkItemsToBeCompleted(value));
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
                    projects[0] = new Project(value);
                }

                NotifyNumberOfWorkItemsToBeCompletedChanged();
            }
        }

        public int NumberOfProjects
        {
            get
            {
                return projects.Count;
            }
        }

        // TODO: It's technically possible for other classes to change stuff to this collection
        public ObservableCollection<Project> Projects => projects;

        public ConfigurationMode ConfigurationMode
        {
            get => configurationMode;
            private set
            {
                if (configurationMode != value)
                {
                    configurationMode = value;
                    projects.AreCollectionChangesAllowed = value != ConfigurationMode.Simple;
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
            if (projects.Count > 1 && !confirmationAsker.ConfirmToProceed("Switching to 'Simple Mode' will cause all projects to be flattened into one. Do you want to continue?"))
            {
                return;
            }

            FlattenProjectsIntoOne();
            ConfigurationMode = ConfigurationMode.Simple;
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="newProjects"/> is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="newProjects"/> doesn't contain
        /// exactly 1 project when <see cref="ConfigurationMode"/> equals to <see cref="ConfigurationMode.Simple"/>.</exception>
        public void ResetRoadmap(IEnumerable<Project> newProjects)
        {
            if (ConfigurationMode == ConfigurationMode.Simple)
            {
                if(newProjects.Count() != 1)
                {
                    throw new InvalidOperationException("When in simple mode, can only reset using a single project.");
                }

                TemporarilyAllowProjectsToBeUpdated(() =>
                {
                    projects[0] = newProjects.First();
                    NotifyNumberOfWorkItemsToBeCompletedChanged();
                });
            }
            else
            {
                try
                {
                    projects.ResetElements(newProjects);
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

        private void TemporarilyAllowProjectsToBeUpdated(Action call)
        {
            projects.AreCollectionChangesAllowed = true;
            try
            {
                call();
            }
            finally
            {
                projects.AreCollectionChangesAllowed = false;
            }
        }

        private void FlattenProjectsIntoOne()
        {
            if (projects.Count > 1)
            {
                ResetRoadmap(new[] { new Project(projects.Sum(p => p.NumberOfWorkItemsRemaining)) });
            }
        }

        private void Projects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    NotifyNumberOfProjectsChanged();
                    SetNumberOfWorkItemsToBeCompleted(projects.Sum(p => p.NumberOfWorkItemsRemaining));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // Note: Number of projects never changed because of a replacement.
                    if(ConfigurationMode == ConfigurationMode.Simple)
                    {
                        numberOfWorkItemsToBeCompleted = projects.Sum(p => p.NumberOfWorkItemsRemaining);
                    }
                    else
                    {
                        SetNumberOfWorkItemsToBeCompleted(projects.Sum(p => p.NumberOfWorkItemsRemaining));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // Do Nothing, as moving elements around in Projects doesn't affect NumberOfProjects nor NumberofWorkItemsToBeCompleted.
                    break;
                default: throw new InvalidEnumArgumentException(nameof(e.Action), (int)e.Action, typeof(NotifyCollectionChangedAction));
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
