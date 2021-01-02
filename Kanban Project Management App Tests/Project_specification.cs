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
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class Project_specification
    {
        public static IEnumerable<object[]> InvalidBacklogScenarios
        {
            get
            {
                yield return new object[] { 0 };
                yield return new object[] { int.MinValue };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidBacklogScenarios))]
        public void GIVEN_an_invalid_number_of_working_items_remaining_WHEN_constructing_new_instance_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfWorkItemsRemaining)
        {
            void call() => new Project(invalidNumberOfWorkItemsRemaining);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("numberOfWorkItemsRemaining", call);
            Assert.StartsWith("Project should consist out of at least 1 work item.", actualException.Message);
        }

        [Fact]
        public void GIVEN_valid_number_of_working_items_remaining_THEN_project_name_is_set()
        {
            var project = new Project(1);

            Assert.Equal("Project", project.Name);
        }

        [Fact]
        public void GIVEN_project_that_has_all_work_completed_already_WHEN_completing_work_item_THEN_throw_InvalidOperationException()
        {
            var project = new Project(1);
            project.CompleteWorkItem();

            void call() => project.CompleteWorkItem();

            var actualException = Assert.Throws<InvalidOperationException>(call);
            Assert.Equal("There is no more work to be completed.", actualException.Message);
        }

        [Fact]
        public void GIVEN_an_project_name_null_WHEN_constructing_new_instance_THEN_throw_ArgumentNullException()
        {
            void call() => new Project(1, null);

            var actualException = Assert.Throws<ArgumentNullException>("name", call);
            Assert.StartsWith("Project name should not be null.", actualException.Message);
        }
    }
}
