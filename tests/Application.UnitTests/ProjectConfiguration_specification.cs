//  Copyright (c) 2020 S.L. des Bouvrie
// 
//  This file is part of 'Kanban Project Management App'.
// 
//  Kanban Project Management App is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Kanban Project Management App is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with Kanban Project Management App.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class ProjectConfiguration_specification
    {
        private readonly int somePriorityWeight = default;
        private readonly string someName = "A";
        
        public static IEnumerable<object[]> InvalidProjectNames
        {
            get
            {
                yield return new object[] {null};
                yield return new object[] {string.Empty};
                yield return new object[] {"  "};
            }
        }
        
        [Theory]
        [MemberData(nameof(InvalidProjectNames))]
        public void GIVEN_invalid_name_WHEN_constructing_throw_ArgumentException(
            string invalidProjectName)
        {
            void Call() => new ProjectConfiguration(invalidProjectName, 1, somePriorityWeight);

            var actualException = Assert.Throws<ArgumentException>("name", Call);
            Assert.StartsWith("Name must be specified.", actualException.Message);
        }

        public static IEnumerable<object[]> InvalidNumberOfWorkItems
        {
            get
            {
                yield return new object[] {int.MinValue};
                yield return new object[] {0};
            }
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfWorkItems))]
        public void GIVEN_invalid_number_of_work_items_to_be_completed_WHEN_constructing_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfWorkItems)
        {
            void Call() => new ProjectConfiguration(someName, invalidNumberOfWorkItems, somePriorityWeight);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("numberOfWorkItemsToBeCompleted", Call);
            Assert.StartsWith("Project must have at least 1 work item to be completed.", actualException.Message);
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfWorkItems))]
        public void GIVEN_some_project_configuration_WHEN_setting_invalid_number_of_work_items_to_be_completed_THEN_throw_ArgumentOutOfRangeException(
                int invalidNumberOfWorkItems)
        {
            var someProjectConfiguration = new ProjectConfiguration(someName, 1, somePriorityWeight);
            void Call() => someProjectConfiguration.NumberOfWorkItemsToBeCompleted = invalidNumberOfWorkItems;

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("value", Call);
            Assert.StartsWith("Project must have at least 1 work item to be completed.", actualException.Message);
        }
    }
}