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
using KanbanProjectManagementApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static KanbanProjectManagementApp.Application.RoadmapConfigurator;

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
                RoadmapConfigurator.ResetRoadmap(projectsRowItems.Select(i => i.ToConfiguration()).ToArray());
            }
        }

        private ObservableCollection<ProjectRowItem> GetProjectRowItems(RoadmapConfigurationViewModel configurationViewModel)
        {
            return new ObservableCollectionThatKeepsAtLeastOneItem<ProjectRowItem>(
                configurationViewModel.Roadmap.Projects.Select(ProjectRowItem.FromDomain).ToArray(), "project of Roadmap");
        }

        private bool reEntrancyGuardWhenSwitchingToSimpleMode = false;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.OriginalSource == ModeSelectorControl)
            {
                e.Handled = true;

                ConfigurationMode configurationMode = ToConfigurationMode(ModeSelectorControl.SelectedIndex);
                switch (configurationMode)
                {
                    case ConfigurationMode.Simple:
                        // Note: Unexpected behavior and unsure what is causing it:
                        // When for `SwitchToSimpleConfigurationMode` gets canceled by the user and remains in Advanced mode
                        // the 1st attempt to select the Advanced mode tab again somehow re-triggers a second attempt to put it
                        // in Simple mode. The guard is there to prevent serving the user the same question AGAIN but we still
                        // need to change the SelectedIndex again...
                        if (reEntrancyGuardWhenSwitchingToSimpleMode)
                        {
                            reEntrancyGuardWhenSwitchingToSimpleMode = false;
                            ModeSelectorControl.SelectedIndex = 1;
                        }
                        else
                        {
                            RoadmapConfigurator.SwitchToSimpleConfigurationMode();
                            if (RoadmapConfigurator.ConfigurationMode == ConfigurationMode.Advanced)
                            {
                                reEntrancyGuardWhenSwitchingToSimpleMode = true;
                                ModeSelectorControl.SelectedIndex = 1;
                            }
                        }
                        break;
                    case ConfigurationMode.Advanced:
                        RoadmapConfigurator.SwitchToAdvancedConfigurationMode();
                        break;
                    default:
                        throw new InvalidEnumArgumentException(
                           nameof(configurationMode),
                           (int)configurationMode,
                           typeof(ConfigurationMode));
                }
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

        private class IAskUserForConfirmationToProceedStub : IAskUserForConfirmationToProceed
        {
            public bool ConfirmToProceed(string questionToUser)
            {
                return true;
            }
        }
    }
}
