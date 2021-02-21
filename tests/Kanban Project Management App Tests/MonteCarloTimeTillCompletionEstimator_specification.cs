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
    public class MonteCarloTimeTillCompletionEstimator_specification
    {
        private readonly IReadOnlyList<InputMetric> inputMetrics = Array.Empty<InputMetric>();

        public static IEnumerable<object[]> InvalidNumberOfSimulationsScenarions
        {
            get
            {
                yield return new object[] { int.MinValue };
                yield return new object[] { 0 };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfSimulationsScenarions))]
        public void WHEN_constructing_new_instance_AND_number_of_simulations_is_less_than_1_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfSimulations)
        {
            void call() => new MonteCarloTimeTillCompletionEstimator(invalidNumberOfSimulations, 1, inputMetrics);


            AssertActionThrowsArgumentOutOfRangeException(call, "numberOfSimulations", "Number of simulations should be at least 1.");
        }

        public static IEnumerable<object[]> InvalidMaximumNumberOfIterationsScenarions
        {
            get
            {
                yield return new object[] { int.MinValue };
                yield return new object[] { 0 };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidMaximumNumberOfIterationsScenarions))]
        public void WHEN_constructing_new_instance_AND_maximum_number_of_iterations_is_less_than_1_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfIterations)
        {
            void call() => new MonteCarloTimeTillCompletionEstimator(1, invalidNumberOfIterations, inputMetrics);

            AssertActionThrowsArgumentOutOfRangeException(call, "maximumNumberOfIterations", "Maximum number of iterations should be at least 1.");
        }

        [Fact]
        public void WHEN_constructing_new_instance_AND_input_metrics_are_null_THEN_throw_ArgumentNullException()
        {
            static void call() => new MonteCarloTimeTillCompletionEstimator(1, 1, null);

            var actualException = Assert.Throws<ArgumentNullException>("inputMetrics", call);
        }

        [Fact]
        public void GIVEN_no_input_metrics_WHEN_estimating_THEN_throw_InvalidOperationException()
        {
            var estimator = new MonteCarloTimeTillCompletionEstimator(1, 1, inputMetrics);
            var projects = new[] { new Project(1) };

            void call() => estimator.Estimate(projects);
            var actualException = Assert.Throws<InvalidOperationException>(call);
            Assert.Equal("At least 1 datapoint of input metrics is required for estimation.", actualException.Message);
        }

        [Fact]
        public void GIVEN_single_completed_project_WHEN_estimating_THEN_throw_ArgumentOutOfRangeException()
        {
            var inputMetrics = new[] { new InputMetric { Throughput = new ThroughputPerDay(2) } };
            var estimator = new MonteCarloTimeTillCompletionEstimator(1, 1, inputMetrics);

            Project project = CreateCompletedProject();
            var projects = new[] { project };

            void call() => estimator.Estimate(projects);
            AssertActionThrowsArgumentOutOfRangeException(call, "projectsToComplete", "Number of workitems to complete should be at least 1.");
        }

        [Fact]
        public void GIVEN_one_input_metric_AND_two_simulations_WHEN_estimating_THEN_return_two_work_estimates()
        {
            var inputMetrics = new[]
            {
                new InputMetric { Throughput = new ThroughputPerDay(2) }
            };
            var numberOfSimulations = 2;
            var estimator = new MonteCarloTimeTillCompletionEstimator(numberOfSimulations, 10, inputMetrics);

            var projectName = "test";
            var projects = new[] { new Project(10, default, projectName) };

            var estimations = estimator.Estimate(projects);
            Assert.Equal(numberOfSimulations, estimations.RoadmapEstimations.Count);
            Assert.Collection(estimations.RoadmapEstimations,
                estimate1 => Assert.Equal("Roadmap", estimate1.Identifier),
                estimate2 => Assert.Equal("Roadmap", estimate2.Identifier));
            Assert.Collection(estimations.RoadmapEstimations,
                estimate1 => Assert.False(estimate1.IsIndeterminate),
                estimate2 => Assert.False(estimate2.IsIndeterminate));
            Assert.Collection(estimations.RoadmapEstimations,
                estimate1 => Assert.Equal(5.0, estimate1.EstimatedNumberOfWorkingDaysRequired),
                estimate2 => Assert.Equal(5.0, estimate2.EstimatedNumberOfWorkingDaysRequired));

            var projectEstimates = estimations[0];
            Assert.Equal(numberOfSimulations, projectEstimates.Count);
            Assert.Collection(projectEstimates,
                estimate1 => Assert.Equal(projectName, estimate1.Identifier),
                estimate2 => Assert.Equal(projectName, estimate2.Identifier));
            Assert.Collection(projectEstimates,
                estimate1 => Assert.False(estimate1.IsIndeterminate),
                estimate2 => Assert.False(estimate2.IsIndeterminate));
            Assert.Collection(projectEstimates,
                estimate1 => Assert.Equal(5.0, estimate1.EstimatedNumberOfWorkingDaysRequired),
                estimate2 => Assert.Equal(5.0, estimate2.EstimatedNumberOfWorkingDaysRequired));
            Assert.Collection(projectEstimates,
                estimate1 => Assert.Equal(5.0, estimate1.EstimatedNumberOfWorkingDaysRequired),
                estimate2 => Assert.Equal(5.0, estimate2.EstimatedNumberOfWorkingDaysRequired));
        }

        private static Project CreateCompletedProject()
        {
            var project = new Project(1);
            project.CompleteWorkItem();
            return project;
        }

        private static void AssertActionThrowsArgumentOutOfRangeException(
            Action call,
            string expectedParamName,
            string expectedMessage)
        {
            var actualException = Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, call);
            Assert.StartsWith(expectedMessage, actualException.Message);
        }
    }
}
