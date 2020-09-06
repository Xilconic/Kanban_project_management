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
using KanbanProjectManagementApp.Tests.TestUtilities;
using KanbanProjectManagementApp.ViewModels;
using System;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class RoadmapConfigurationViewModel_specification
    {
        public class GIVEN_newly_created_instance
        {
            private readonly RoadmapConfigurationViewModel newlyCreatedViewModel;

            public GIVEN_newly_created_instance()
            {
                newlyCreatedViewModel = new RoadmapConfigurationViewModel();
            }

            [Fact]
            public void THEN_number_of_work_items_to_be_completed_is_ten()
            {
                Assert.Equal(10, newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted);
            }

            [Fact]
            public void THEN_number_of_projects_is_one()
            {
                Assert.Equal(1, newlyCreatedViewModel.NumberOfProjects);
            }

            [Theory]
            [InlineData(int.MinValue)]
            [InlineData(0)]
            public void WHEN_setting_invalid_number_of_work_items_THEN_throw_ArgumentOutOfRangeException(
                int invalidNumberOfWorkItems)
            {
                void call() => newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted = invalidNumberOfWorkItems;

                var actualException = Assert.Throws<ArgumentOutOfRangeException>("value", call);
                Assert.StartsWith("Number of work items to be completed must be at least 1.", actualException.Message);
            }

            [Fact]
            public void WHEN_setting_new_number_of_work_items_THEN_number_of_work_items_changed()
            {
                using var propertyChangeTracker = new PropertyChangedEventTracker(newlyCreatedViewModel);

                newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted = 35;

                Assert.Equal(35, newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted);

                propertyChangeTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(nameof(newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted));
            }

            [Fact]
            public void WHEN_setting_same_number_of_work_items_THEN_nothing_happens()
            {
                using var propertyChangeTracker = new PropertyChangedEventTracker(newlyCreatedViewModel);

                newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted = newlyCreatedViewModel.NumberOfWorkItemsToBeCompleted;

                propertyChangeTracker.AssertNoPropertyChangeNotificationsHappened();
            }
        }
    }
}
