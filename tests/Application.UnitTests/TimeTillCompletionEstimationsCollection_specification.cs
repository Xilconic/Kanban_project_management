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
using KanbanProjectManagementApp.Domain;
using System;
using System.Linq;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class TimeTillCompletionEstimationsCollection_specification
    {
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void GIVEN_an_invalid_number_of_simulations_WHEN_constructing_new_instance_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfSimulations)
        {
            void Call() => new TimeTillCompletionEstimationsCollection(invalidNumberOfSimulations, 1);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("numberOfSimulations", Call);
            Assert.StartsWith("Number of simulations should be at least 1.", actualException.Message);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void GIVEN_an_invalid_number_of_projects_in_roadmap_WHEN_constructing_new_instance_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfProjectsInRoadmap)
        {
            void Call() => new TimeTillCompletionEstimationsCollection(1, invalidNumberOfProjectsInRoadmap);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("numberOfProjectsInRoadmap", Call);
            Assert.StartsWith("Number of projects in roadmap should be at least 1.", actualException.Message);
        }

        [Fact]
        public void GIVEN_roadmap_estimate_null_WHEN_adding_simulation_estimates_THEN_throw_ArgumentNullException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(1, 1);

            var someProjectEstimates = new[] { new WorkEstimate(CreateCompletedProject(), 1.0) };

            void Call() => estimates.AddEstimationsForSimulation(null, someProjectEstimates);

            Assert.Throws<ArgumentNullException>("roadmapEstimate", Call);
        }

        [Fact]
        public void GIVEN_project_estimates_null_WHEN_adding_simulation_estimates_THEN_throw_ArgumentNullException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(1, 1);

            void Call() => estimates.AddEstimationsForSimulation(new WorkEstimate(CreateCompletedRoadmap(), 1.0), null);

            Assert.Throws<ArgumentNullException>("projectEstimates", Call);
        }

        [Fact]
        public void GIVEN_too_few_projects_WHEN_adding_simulation_estimates_THEN_throw_ArgumentException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(1, 1);

            void Call() => estimates.AddEstimationsForSimulation(new WorkEstimate(CreateCompletedRoadmap(), 1.0), Array.Empty<WorkEstimate>());

            var actualException = Assert.Throws<ArgumentException>("projectEstimates", Call);
            Assert.StartsWith("Expected 1 project estimate(s), but got provided 0.", actualException.Message);
        }

        [Fact]
        public void GIVEN_too_many_projects_WHEN_adding_simulation_estimates_THEN_throw_ArgumentException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(1, 1);

            var tooManyProjects = new[] { CreateCompletedProject(), CreateCompletedProject() };
            var tooManyProjectEstimates = tooManyProjects.Select(p => new WorkEstimate(p, 1.0)).ToArray();

            void Call() => estimates.AddEstimationsForSimulation(new WorkEstimate(CreateCompletedRoadmap(), 1.0), tooManyProjectEstimates);

            var actualException = Assert.Throws<ArgumentException>("projectEstimates", Call);
            Assert.StartsWith("Expected 1 project estimate(s), but got provided 2.", actualException.Message);
        }

        [Fact]
        public void GIVEN_estimations_collection_for_1_simulation_WHEN_adding_2nd_simulation_results_THEN_throw_InvalidOperationException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(1, 1);

            var roadmap = CreateCompletedRoadmap();
            var roadmapEstimate = new WorkEstimate(roadmap, 1.0);
            var projectEstimates = roadmap.Projects.Select(p => new WorkEstimate(p, 1.0)).ToArray();

            estimates.AddEstimationsForSimulation(roadmapEstimate, projectEstimates);
            void SecondCall() => estimates.AddEstimationsForSimulation(roadmapEstimate, projectEstimates);

            var actualException = Assert.Throws<InvalidOperationException>(SecondCall);
            Assert.Equal("Adding these estimation would exceed the expected number of simulations.", actualException.Message);
        }

        [Fact]
        public void GIVEN_estimations_collection_with_1_simulation_WHEN_adding_different_project_estimations_THEN_throw_ArgumentException()
        {
            var estimates = new TimeTillCompletionEstimationsCollection(2, 1);

            var roadmap = CreateCompletedRoadmap();
            var roadmapEstimate = new WorkEstimate(roadmap, 1.0);
            var projectEstimates = roadmap.Projects.Select(p => new WorkEstimate(p, 1.0)).ToArray();
            estimates.AddEstimationsForSimulation(roadmapEstimate, projectEstimates);

            var differentProjectEstimates = new[] { new WorkEstimate(CreateCompletedProject("someUniqueName"), 1.0) };
            void SecondCall() => estimates.AddEstimationsForSimulation(roadmapEstimate, differentProjectEstimates);

            var actualException = Assert.Throws<ArgumentException>("projectEstimates", SecondCall);
            Assert.StartsWith("A project estimate's identifier mismatches a the expected project's identifier.", actualException.Message);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void GIVEN_some_simulation_results_added_WHEN_getting_estimates_for_invalid_project_number_THEN_throw_ArgumentOutOfRangeException(
            int invalidProjectIndex)
        {
            var estimates = new TimeTillCompletionEstimationsCollection(2, 1);

            var roadmap = CreateCompletedRoadmap();
            var roadmapEstimate = new WorkEstimate(roadmap, 1.0);
            var projectEstimates = roadmap.Projects.Select(p => new WorkEstimate(p, 1.0)).ToArray();

            estimates.AddEstimationsForSimulation(roadmapEstimate, projectEstimates);
            estimates.AddEstimationsForSimulation(roadmapEstimate, projectEstimates);

            object Call() => estimates[invalidProjectIndex];

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("projectIndex", Call);
            Assert.StartsWith("Project index must be in range [0, 0].", actualException.Message);
        }

        private static Project CreateCompletedProject(string name = "Project")
        {
            var project = new Project(1, default, name);
            project.CompleteWorkItem();
            return project;
        }

        private static Roadmap CreateCompletedRoadmap()
        {
            var project = new Project(1);
            var projects = new[] { project };
            var roadmap = new Roadmap(projects);

            project.CompleteWorkItem();

            return roadmap;
        }
    }
}
