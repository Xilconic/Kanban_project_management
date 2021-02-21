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
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Domain
{
    public class Roadmap_specification
    {
        [Fact]
        public void GIVEN_projects_collection_null_WHEN_constructing_new_instance_of_roadmap_THEN_throw_ArgumentNullException()
        {
            static void call() => new Roadmap(null);

            Assert.Throws<ArgumentNullException>("projects", call);
        }

        [Fact]
        public void GIVEN_no_projects_in_sequence_WHEN_constructing_new_instance_of_roadmap_THEN_throw_ArgumentException()
        {
            var emptyProjectsCollection = Array.Empty<Project>();

            void call() => new Roadmap(emptyProjectsCollection);

            var actualException = Assert.Throws<ArgumentException>("projects", call);
            Assert.StartsWith("Roadmap should contain at least one project.", actualException.Message);
        }

        [Fact]
        public void GIVEN_all_projects_have_no_work_to_be_done_WHEN_constructing_new_instance_of_roadmap_THEN_throw_ArgumentException()
        {
            var completedProject = new Project(1);
            completedProject.CompleteWorkItem();

            void call() => new Roadmap(new[] { completedProject });

            var actualException = Assert.Throws<ArgumentException>("projects", call);
            Assert.StartsWith("Roadmap should contain only out of projects that has work to be completed.", actualException.Message);
        }

        [Fact]
        public void GIVEN_project_sequence_contains_null_WHEN_constructing_new_instance_of_roadmap_THEN_throw_ArgumentException()
        {
            var projectSequenceWithNullElements = new[]
            {
                new Project(1, default, "A"),
                null,
                new Project(2, default, "B"),
            };

            void call() => new Roadmap(projectSequenceWithNullElements);

            var actualException = Assert.Throws<ArgumentException>("projects", call);
            Assert.StartsWith("Sequence of projects for roadmap cannot contain null elements.", actualException.Message);
        }

        [Fact]
        public void GIVEN_project_sequence_contains_projects_with_same_name_WHEN_constructing_new_instance_of_roadmap_THEN_throw_ArgumentException()
        {
            var projectSequenceWithNullElements = new[]
            {
                new Project(1, default, "A"),
                new Project(2, default, "A"),
            };

            void call() => new Roadmap(projectSequenceWithNullElements);

            var actualException = Assert.Throws<ArgumentException>("projects", call);
            Assert.StartsWith("Sequence of projects for roadmap cannot contain multiple projects with the same name.", actualException.Message);
        }
    }
}
