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
using KanbanProjectManagementApp.Tests.TestUtilities;
using KanbanProjectManagementApp.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using static KanbanProjectManagementApp.Application.RoadmapConfigurator;

namespace KanbanProjectManagementApp.Tests.Unit
{
    public class RoadmapConfigurationViewModel_specification
    {
        public class WHEN_constructing_new_instance
        {
            [Fact]
            public void GIVEN_IAskUserForConfirmationToProceed_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new RoadmapConfigurationViewModel(null);
                Assert.Throws<ArgumentNullException>("confirmationAsker", call);
            }
        }

        public class GIVEN_newly_created_instance
        {
            private readonly RoadmapConfigurationViewModel newlyCreatedViewModel;

            public GIVEN_newly_created_instance()
            {
                newlyCreatedViewModel = new RoadmapConfigurationViewModel(new Mock<IAskUserForConfirmationToProceed>().Object);
            }

            [Fact]
            public void THEN_configuration_mode_is_simple()
            {
                Assert.Equal(ConfigurationMode.Simple, newlyCreatedViewModel.ConfigurationMode);
            }

            [Fact]
            public void THEN_number_of_work_items_to_be_completed_is_ten()
            {
                Assert.Equal(10, newlyCreatedViewModel.TotalNumberOfWorkItemsToBeCompleted);
            }

            [Fact]
            public void THEN_number_of_projects_is_one()
            {
                Assert.Equal(1, newlyCreatedViewModel.NumberOfProjects);
            }

            [Fact]
            public void THEN_number_of_projects_is_one_and_project_is_initialized_with_some_defaults()
            {
                var theProject = Assert.Single(newlyCreatedViewModel.Roadmap.Projects);
                Assert.Equal(10, theProject.NumberOfWorkItemsToBeCompleted);
            }
        }

        public class GIVEN_view_model_configured_in_simple_mode : IDisposable
        {
            private readonly RoadmapConfigurationViewModel viewModel;
            private readonly PropertyChangedEventTracker propertyChangedTracker;

            public GIVEN_view_model_configured_in_simple_mode()
            {
                viewModel = new RoadmapConfigurationViewModel(new Mock<IAskUserForConfirmationToProceed>().Object);

                Assert.Equal(ConfigurationMode.Simple, viewModel.ConfigurationMode);

                propertyChangedTracker = new PropertyChangedEventTracker(viewModel);
            }

            public void Dispose()
            {
                propertyChangedTracker.Dispose();
            }

            [Theory]
            [InlineData(int.MinValue)]
            [InlineData(0)]
            public void WHEN_setting_invalid_number_of_work_items_THEN_throw_ArgumentOutOfRangeException(
                int invalidNumberOfWorkItems)
            {
                void call() => viewModel.TotalNumberOfWorkItemsToBeCompleted = invalidNumberOfWorkItems;

                var actualException = Assert.Throws<ArgumentOutOfRangeException>("value", call);
                Assert.StartsWith("Number of work items to be completed must be at least 1.", actualException.Message);

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void WHEN_setting_new_number_of_work_items_THEN_number_of_work_items_changed()
            {
                const int newNumberOfWorkItemsToBeCompleted = 35;
                viewModel.TotalNumberOfWorkItemsToBeCompleted = newNumberOfWorkItemsToBeCompleted;

                Assert.Equal(newNumberOfWorkItemsToBeCompleted, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                Assert.Equal(newNumberOfWorkItemsToBeCompleted, viewModel.Roadmap.Projects.First().NumberOfWorkItemsToBeCompleted);

                propertyChangedTracker.AssertNoPropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void WHEN_setting_same_number_of_work_items_THEN_nothing_happens()
            {
                viewModel.TotalNumberOfWorkItemsToBeCompleted = viewModel.TotalNumberOfWorkItemsToBeCompleted;

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void WHEN_switching_to_advanced_configuration_mode_THEN_configuration_mode_changed_to_advanced()
            {
                viewModel.SwitchToAdvancedConfigurationMode();

                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.ConfigurationMode));
            }

            [Fact]
            public void WHEN_resetting_to_single_project_THEN_projects_and_related_properties_updated()
            {
                const int NumberOfWorkItemsRemaining = 2;
                var projects = new[] { new ProjectConfiguration("A", NumberOfWorkItemsRemaining, 7) };

                viewModel.ResetRoadmap(projects);

                var actualProject = Assert.Single(viewModel.Roadmap.Projects);
                Assert.NotSame(projects[0], actualProject);
                Assert.Equal("Project", actualProject.Name);
                Assert.Equal(NumberOfWorkItemsRemaining, actualProject.NumberOfWorkItemsToBeCompleted);
                Assert.Equal(0, actualProject.PriorityWeight);

                Assert.Equal(1, viewModel.NumberOfProjects);
                propertyChangedTracker.AssertNoPropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));

                Assert.Equal(NumberOfWorkItemsRemaining, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void WHEN_resetting_to_more_than_one_project_THEN_throw_InvalidOperationException()
            {
                var projects = new[]
                {
                    new ProjectConfiguration("A", 1, default),
                    new ProjectConfiguration("B", 2, default)
                };

                void call() => viewModel.ResetRoadmap(projects);

                var actualException = Assert.Throws<InvalidOperationException>(call);
                Assert.Equal("When in simple mode, can only reset using a single project.", actualException.Message);

                Assert.Equal(1, viewModel.NumberOfProjects);
                Assert.Equal(10, viewModel.TotalNumberOfWorkItemsToBeCompleted);

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void WHEN_configuring_to_simple_mode_THEN_do_nothing()
            {
                viewModel.SwitchToSimpleConfigurationMode();

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }
        }

        public class GIVEN_view_model_configured_in_advanced_mode : IDisposable
        {
            private readonly RoadmapConfigurationViewModel viewModel;
            private readonly PropertyChangedEventTracker propertyChangedTracker;
            private readonly Mock<IAskUserForConfirmationToProceed> askUserForConfirmationToProceedMock;

            public GIVEN_view_model_configured_in_advanced_mode()
            {
                askUserForConfirmationToProceedMock = new Mock<IAskUserForConfirmationToProceed>();

                viewModel = new RoadmapConfigurationViewModel(askUserForConfirmationToProceedMock.Object);
                viewModel.SwitchToAdvancedConfigurationMode();

                Assert.Equal(ConfigurationMode.Advanced, viewModel.ConfigurationMode);

                propertyChangedTracker = new PropertyChangedEventTracker(viewModel);
            }

            public void Dispose()
            {
                propertyChangedTracker.Dispose();
            }

            [Fact]
            public void WHEN_setting_number_of_work_items_to_be_completed_THEN_throw_InvalidOperationException()
            {
                void call() => viewModel.TotalNumberOfWorkItemsToBeCompleted = 1;

                var actualException = Assert.Throws<InvalidOperationException>(call);
                Assert.Equal("Cannot set a value for 'TotalNumberOfWorkItemsToBeCompleted' when in Advanced configuration mode.", actualException.Message);
            }

            [Fact]
            public void WHEN_switching_to_advanced_configuration_mode_THEN_nothing_happens()
            {
                viewModel.SwitchToAdvancedConfigurationMode();

                Assert.Equal(ConfigurationMode.Advanced, viewModel.ConfigurationMode);
                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void WHEN_adding_new_project_THEN_number_of_projects_and_work_items_updated()
            {
                var newProjects = new[]
                {
                    viewModel.Roadmap.Projects.First(),
                    new ProjectConfiguration("A", 1, default)
                };
                viewModel.ResetRoadmap(newProjects);

                Assert.Equal(2, viewModel.NumberOfProjects);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));

                Assert.Equal(11, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void AND_an_empty_collection_of_projects_WHEN_resetting_roadmap_THEN_throw_ArgumentException()
            {
                var newProjects = Array.Empty<ProjectConfiguration>();

                void call() => viewModel.ResetRoadmap(newProjects);

                var actualException = Assert.Throws<ArgumentException>("newProjects", call);
                Assert.StartsWith("Requires at least one project when resetting a Roadmap.", actualException.Message);

                Assert.Equal(1, viewModel.NumberOfProjects);
                Assert.Equal(10, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                var actualProject = Assert.Single(viewModel.Roadmap.Projects);
                Assert.Equal(10, actualProject.NumberOfWorkItemsToBeCompleted);

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void AND_some_projects_in_a_collection_WHEN_resetting_roadmap_THEN_Projects_and_summary_properties_updated()
            {
                var newProjects = new[]
                {
                    new ProjectConfiguration("A", 1, default),
                    new ProjectConfiguration("B", 2, default),
                };

                viewModel.ResetRoadmap(newProjects);

                Assert.Equal(newProjects, viewModel.Roadmap.Projects, new ProjectConfigurationEqualityComparer());

                Assert.Equal(newProjects.Length, viewModel.NumberOfProjects);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));

                Assert.Equal(3, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void WHEN_switching_to_simple_configuration_mode_while_there_is_only_one_project_THEN_only_change_configuration_mode()
            {
                viewModel.SwitchToSimpleConfigurationMode();

                Assert.Equal(ConfigurationMode.Simple, viewModel.ConfigurationMode);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.ConfigurationMode));

                Assert.Equal(1, viewModel.NumberOfProjects);
                propertyChangedTracker.AssertNoPropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));

                Assert.Equal(10, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                propertyChangedTracker.AssertNoPropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void AND_has_multiple_projects_configured_WHEN_switching_to_simple_configuration_AND_declining_flattening_THEN_do_nothing()
            {
                askUserForConfirmationToProceedMock
                    .Setup(c => c.ConfirmToProceed(It.IsAny<string>()))
                    .Returns(false);

                viewModel.ResetRoadmap(new[]
                {
                    new ProjectConfiguration("A", 1, default),
                    new ProjectConfiguration("B", 2, default),
                });

                propertyChangedTracker.ClearAllRecordedEvents();

                viewModel.SwitchToSimpleConfigurationMode();

                Assert.Equal(ConfigurationMode.Advanced, viewModel.ConfigurationMode);

                propertyChangedTracker.AssertNoPropertyChangeNotificationsHappened();
            }

            [Fact]
            public void AND_has_multiple_projects_configured_WHEN_switching_to_simple_configuration_AND_accepting_flattening_THEN_do_nothing()
            {
                askUserForConfirmationToProceedMock
                    .Setup(c => c.ConfirmToProceed("Switching to 'Simple Mode' will cause all projects to be flattened into one. Do you want to continue?"))
                    .Returns(true);

                viewModel.ResetRoadmap(new[]
                {
                    new ProjectConfiguration("A", 1, default),
                    new ProjectConfiguration("B", 1, default),
                });

                propertyChangedTracker.ClearAllRecordedEvents();

                viewModel.SwitchToSimpleConfigurationMode();

                Assert.Equal(ConfigurationMode.Simple, viewModel.ConfigurationMode);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.ConfigurationMode));

                Assert.Equal(1, viewModel.NumberOfProjects);
                propertyChangedTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(viewModel.NumberOfProjects));

                Assert.Equal(2, viewModel.TotalNumberOfWorkItemsToBeCompleted);
                propertyChangedTracker.AssertNoPropertyChangeNotificationHappenedForName(nameof(viewModel.TotalNumberOfWorkItemsToBeCompleted));

                var actualProject = Assert.Single(viewModel.Roadmap.Projects);
                Assert.Equal(2, actualProject.NumberOfWorkItemsToBeCompleted);
            }

            internal class ProjectConfigurationEqualityComparer : IEqualityComparer<ProjectConfiguration>
            {
                public bool Equals([AllowNull] ProjectConfiguration x, [AllowNull] ProjectConfiguration y)
                {
                    if (x is null)
                    {
                        return y is null;
                    }

                    if (y is null)
                    {
                        return false;
                    }

                    return x.NumberOfWorkItemsToBeCompleted == y.NumberOfWorkItemsToBeCompleted;
                }

                public int GetHashCode([DisallowNull] ProjectConfiguration obj)
                {
                    return obj.NumberOfWorkItemsToBeCompleted.GetHashCode();
                }
            }
        }
    }
}
