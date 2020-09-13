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
using KanbanProjectManagementApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KanbanProjectManagementApp.Views
{
    /// <summary>
    /// Interaction logic for RoadmapConfiguratorControl.xaml
    /// </summary>
    public partial class RoadmapConfiguratorControl : UserControl
    {
        public RoadmapConfiguratorControl()
        {
            InitializeComponent();
            ModeSelectorControl.SelectionChanged += TabControl_SelectionChanged;
        }

        public static readonly DependencyProperty RoadmapConfiguratorProperty =
             DependencyProperty.Register(nameof(RoadmapConfigurator), typeof(RoadmapConfigurationViewModel),
             typeof(RoadmapConfiguratorControl), new FrameworkPropertyMetadata(new RoadmapConfigurationViewModel(new IAskUserForConfirmationToProceedStub())));

        public RoadmapConfigurationViewModel RoadmapConfigurator
        {
            get { return (RoadmapConfigurationViewModel)GetValue(RoadmapConfiguratorProperty); }
            set { SetValue(RoadmapConfiguratorProperty, value); }
        }

        private void EditRoadmapProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var projectsRowItems = GetProjectRowItems(RoadmapConfigurator);
            var projectedEditorWindow = new ProjectsEditorWindow(projectsRowItems);
            var result = projectedEditorWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                RoadmapConfigurator.ResetRoadmap(projectsRowItems.Select(i => i.ToDomain()));
            }
        }

        private ObservableCollection<ProjectRowItem> GetProjectRowItems(RoadmapConfigurationViewModel configurationViewModel)
        {
            return new ObservableCollectionThatKeepsAtLeastOneItem<ProjectRowItem>(
                new[] { new Project(configurationViewModel.NumberOfWorkItemsToBeCompleted) }
                    .Select(ProjectRowItem.FromDomain),
                "project of Roadmap"
            );
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = (TabControl)e.Source;
            ConfigurationMode configurationMode = ToConfigurationMode(tabControl.SelectedIndex);
            switch (configurationMode)
            {
                case ConfigurationMode.Simple:
                    RoadmapConfigurator.SwitchToSimpleConfigurationMode();
                    break;
                case ConfigurationMode.Advanced:
                    RoadmapConfigurator.SwitchToAdvancedConfigurationMode();
                    break;
                default: throw new InvalidEnumArgumentException(
                    nameof(configurationMode),
                    (int)configurationMode,
                    typeof(ConfigurationMode));
            }
        }

        private ConfigurationMode ToConfigurationMode(int selectedIndex)
        {
            return selectedIndex switch
            {
                0 => ConfigurationMode.Simple,
                1 => ConfigurationMode.Advanced,
                _ => throw new ArgumentOutOfRangeException(nameof(selectedIndex)),
            };
        }

        private enum ConfigurationMode
        {
            Simple,
            Advanced,
        }

        private class IAskUserForConfirmationToProceedStub : IAskUserForConfirmationToProceed
        {
            public bool ConfirmToProceed(string questionToUser)
            {
                return true;
            }
        }
    }
}
