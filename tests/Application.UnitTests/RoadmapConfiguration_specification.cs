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
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class RoadmapConfiguration_specification
    {
        [Fact]
        public void GIVEN_some_roadmap_WHEN_setting_empty_projects_collection_THEN_throw_ArgumentExceptionException()
        {
            var projects = new[] { new ProjectConfiguration("A", 10, default) };
            var roadmap = new RoadmapConfiguration(projects);

            void Call() => roadmap.Projects = Array.Empty<ProjectConfiguration>();

            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.StartsWith("Roadmap should contain at least one project.", exception.Message);
            Assert.Equal("configuredProjects", exception.ParamName);

            var project = Assert.Single(roadmap.Projects);
            Assert.Equal(10, project.NumberOfWorkItemsToBeCompleted);
        }
    }
}
