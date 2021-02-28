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
using System.Linq;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class RoadmapConfiguration_specification
    {
        [Fact]
        public void GIVEN_project_configurations_null_WHEN_constructing_THEN_throw_ArgumentNullException()
        {
            static void Call() => new RoadmapConfiguration(null);

            Assert.Throws<ArgumentNullException>("configuredProjects", Call);
        }

        [Fact]
        public void GIVEN_empty_projects_collection_WHEN_constructing_THEN_throw_ArgumentException()
        {
            static void Call() => new RoadmapConfiguration(Enumerable.Empty<ProjectConfiguration>());

            var actualException = Assert.Throws<ArgumentException>("configuredProjects", Call);
            Assert.StartsWith("Roadmap should contain at least one project.", actualException.Message);
        }

        [Fact]
        public void GIVEN_projects_collection_contains_null_WHEN_constructing_THEN_throw_ArgumentException()
        {
            var projects = new[]
            {
                new ProjectConfiguration("A", 1, 2),
                null,
                new ProjectConfiguration("B", 3, 4),
            };
            void Call() => new RoadmapConfiguration(projects);

            var actualException = Assert.Throws<ArgumentException>("configuredProjects", Call);
            Assert.StartsWith("Sequence of projects for roadmap cannot contain null elements.", actualException.Message);
        }
        
        [Fact]
        public void GIVEN_2_projects_with_same_name_WHEN_constructing_THEN_throw_ArgumentException()
        {
            const string projectName = "A";
            var projects = new[]
            {
                new ProjectConfiguration(projectName, 1, 2),
                new ProjectConfiguration(projectName, 3, 4),
            };
            void Call() => new RoadmapConfiguration(projects);

            var actualException = Assert.Throws<ArgumentException>("configuredProjects", Call);
            Assert.StartsWith("Sequence of projects for roadmap cannot contain multiple projects with the same name.", actualException.Message);
        }

        public class GIVEN_some_roadmap
        {
            private const int SomeNumberOfWorkItemsToBeCompleted = 10;

            private readonly ProjectConfiguration someProject;
            private readonly RoadmapConfiguration someRoadmap;

            public GIVEN_some_roadmap()
            {
                someProject = new ProjectConfiguration(
                    "A",
                    SomeNumberOfWorkItemsToBeCompleted,
                    default);
                
                var projects = new[] {someProject};
                someRoadmap = new RoadmapConfiguration(projects);
            }
            
            [Fact]
            public void WHEN_setting_null_to_projects_THEN_throw_ArgumentNullException()
            {
                void Call() => someRoadmap.Projects = null;

                Assert.Throws<ArgumentNullException>("configuredProjects", Call);
            
                AssertProjectConfigurationRemainsUnchanged();
            }

            [Fact]
            public void WHEN_setting_empty_projects_collection_THEN_throw_ArgumentExceptionException()
            {
                void Call() => someRoadmap.Projects = Array.Empty<ProjectConfiguration>();

                var exception = Assert.Throws<ArgumentException>("configuredProjects", Call);
                Assert.StartsWith("Roadmap should contain at least one project.", exception.Message);

                AssertProjectConfigurationRemainsUnchanged();
            }
            
            [Fact]
            public void WHEN_setting_projects_collection_containing_null_THEN_throw_ArgumentException()
            {
                var projects = new[]
                {
                    new ProjectConfiguration("A", 1, 2),
                    null,
                    new ProjectConfiguration("B", 3, 4),
                };
                void Call() => someRoadmap.Projects = projects;

                var actualException = Assert.Throws<ArgumentException>("configuredProjects", Call);
                Assert.StartsWith("Sequence of projects for roadmap cannot contain null elements.", actualException.Message);
                
                AssertProjectConfigurationRemainsUnchanged();
            }
            
            [Fact]
            public void GIVEN_2_projects_with_same_name_WHEN_constructing_THEN_throw_ArgumentException()
            {
                const string projectName = "A";
                var projects = new[]
                {
                    new ProjectConfiguration(projectName, 1, 2),
                    new ProjectConfiguration(projectName, 3, 4),
                };
                void Call() => someRoadmap.Projects = projects;

                var actualException = Assert.Throws<ArgumentException>("configuredProjects", Call);
                Assert.StartsWith("Sequence of projects for roadmap cannot contain multiple projects with the same name.", actualException.Message);
                
                AssertProjectConfigurationRemainsUnchanged();
            }
            
            private void AssertProjectConfigurationRemainsUnchanged()
            {
                ProjectConfiguration project = Assert.Single(someRoadmap.Projects);
                Assert.Same(someProject, project);
                Assert.Equal(SomeNumberOfWorkItemsToBeCompleted, project.NumberOfWorkItemsToBeCompleted);
            }
        }
    }
}
