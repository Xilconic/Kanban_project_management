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
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using KanbanProjectManagementApp.Domain;
using KanbanProjectManagementApp.Application;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class TimeTillCompletionEstimator_specification
    {
        private readonly Mock<IRandomNumberGenerator> randomNumberGeneratorMock;
        private readonly RandomNumberGenerator realRandomNumberGenerator;
        private readonly int someMaximumNumberOfIterations = 25;
        private readonly Roadmap someRoadmap = new Roadmap(new[] { new Project(1) });

        public TimeTillCompletionEstimator_specification()
        {
            randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            realRandomNumberGenerator = new RandomNumberGenerator();
        }

        [Fact]
        public void WHEN_constructing_instance_with_input_metrics_null_THEN_throw_ArgumentNullException()
        {
            IReadOnlyList<InputMetric> inputMetrics = null;

            void Call() => new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);
            Assert.Throws<ArgumentNullException>("inputMetrics", Call);
        }

        [Fact]
        public void WHEN_constructing_instance_with_IRandomNumberGenerator_null_THEN_throw_ArgumentNullException()
        {
            var inputMetrics = Array.Empty<InputMetric>();

            void Call() => new TimeTillCompletionEstimator(inputMetrics, null, someMaximumNumberOfIterations);

            Assert.Throws<ArgumentNullException>("rng", Call);
        }

        public static IEnumerable<object[]> InvalidMaximumNumberOfIterationsScenarios
        {
            get
            {
                yield return new object[] { int.MinValue };
                yield return new object[] { -1 };
                yield return new object[] { 0 };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidMaximumNumberOfIterationsScenarios))]
        public void WHEN_constructing_instance_with_invalid_maximum_number_of_iterations_THEN_throw_ArgumentOutOfRangeException(
            int maximumNumberOfIterations)
        {
            var inputMetrics = Array.Empty<InputMetric>();

            void Call() => new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, maximumNumberOfIterations);

            Assert.Throws<ArgumentOutOfRangeException>("maximumNumberOfIterations", Call);
        }

        [Fact]
        public void GIVEN_project_is_null_WHEN_estimating_time_to_completion_THEN_throw_ArgumentNullException()
        {
            var inputMetrics = ToInputMetrics(new[] { ToThroughput(1) });

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            void Call() => estimator.Estimate(null);

            Assert.Throws<ArgumentNullException>("roadmap", Call);
        }

        [Fact]
        public void GIVEN_no_input_metrics_WHEN_estimating_time_to_completion_THEN_throw_InvalidOperationException()
        {
            var inputMetrics = Array.Empty<InputMetric>();

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            void Call() => estimator.Estimate(someRoadmap);

            var actualException = Assert.Throws<InvalidOperationException>(Call);
            Assert.StartsWith("At least 1 datapoint of input metrics is required for estimation.", actualException.Message);
        }

        public static IEnumerable<object[]> SingleMetricEstimationScenarios
        {
            get
            {
                yield return new object[] { ToThroughput(1), new Project(10), 10.0 };
                yield return new object[] { ToThroughput(2), new Project(10), 5.0 };
                yield return new object[] { ToThroughput(3), new Project(10), 3.333333 };
                yield return new object[] { ToThroughput(3), new Project(3), 1.0 };
                yield return new object[] { ToThroughput(3), new Project(1), 0.333333 };
            }
        }

        [Theory]
        [MemberData(nameof(SingleMetricEstimationScenarios))]
        public void GIVEN_one_input_metric_AND_one_project_WHEN_estimating_time_to_completion_THEN_return_number_of_work_days(
            ThroughputPerDay throughput, Project project, double expectedNumberOfDaysRequired)
        {
            var inputMetrics = ToInputMetrics(new[] { throughput });
            var roadmap = new Roadmap(new[] { project });

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            var estimations = estimator.Estimate(roadmap);

            Assert.Equal(2, estimations.Count);

            WorkEstimate roadmapWorkEstimate = estimations[0];
            Assert.Equal("Roadmap", roadmapWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedNumberOfDaysRequired, roadmapWorkEstimate);
            AssertEstimateIsDeterminate(roadmapWorkEstimate);

            WorkEstimate projectWorkEstimate = estimations[1];
            Assert.Equal("Project", projectWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedNumberOfDaysRequired, roadmapWorkEstimate);
            AssertEstimateIsDeterminate(roadmapWorkEstimate);
        }

        [Fact]
        public void GIVEN_all_input_metrics_with_zero_throughput_AND_some_project_WHEN_estimating_time_to_completion_THEN_return_indeterminate_with_lower_bound_estimate()
        {
            var inputMetrics = ToInputMetrics(new[] { ToThroughput(0) });

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            var estimations = estimator.Estimate(someRoadmap);

            Assert.Equal(2, estimations.Count);

            WorkEstimate roadmapWorkEstimate = estimations[0];
            Assert.Equal("Roadmap", roadmapWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(someMaximumNumberOfIterations, roadmapWorkEstimate);
            AssertExpectedNumberOfWorkingDaysIsIndeterminate(roadmapWorkEstimate);

            WorkEstimate projectWorkEstimate = estimations[1];
            Assert.Equal("Project", projectWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(someMaximumNumberOfIterations, projectWorkEstimate);
            AssertExpectedNumberOfWorkingDaysIsIndeterminate(projectWorkEstimate);
        }

        public static IEnumerable<object[]> MultiMetricEstimationScenarios
        {
            get
            {
                yield return new object[] { ToThroughput(2, 2).ToArray(), new Project(10), 5.0, 5.0 };
                yield return new object[] { ToThroughput(1, 2).ToArray(), new Project(10), 5.0, 10.0 };
                yield return new object[] { ToThroughput(5, 1).ToArray(), new Project(10), 2.0, 10.0 };
            }
        }

        [Theory]
        [MemberData(nameof(MultiMetricEstimationScenarios))]
        public void GIVEN_multiple_input_metrics_AND_one_project_AND_truely_random_number_generator_WHEN_estimating_time_to_completion_THEN_return_number_of_work_days_in_range(
            IReadOnlyCollection<ThroughputPerDay> throughputs, Project project, double lowerBoundExpectedNumberOfDaysRequired, double upperBoundExpectedNumberOfDaysRequired)
        {
            var inputMetrics = ToInputMetrics(throughputs);
            var roadmap = new Roadmap(new[] { project });

            var estimator = new TimeTillCompletionEstimator(inputMetrics, realRandomNumberGenerator, someMaximumNumberOfIterations);

            var estimations = estimator.Estimate(roadmap);

            Assert.Equal(2, estimations.Count);

            WorkEstimate roadmapWorkEstimate = estimations[0];
            Assert.Equal("Roadmap", roadmapWorkEstimate.Identifier);
            Assert.InRange(roadmapWorkEstimate.EstimatedNumberOfWorkingDaysRequired, lowerBoundExpectedNumberOfDaysRequired, upperBoundExpectedNumberOfDaysRequired);
            AssertEstimateIsDeterminate(roadmapWorkEstimate);

            WorkEstimate projectWorkEstimate = estimations[1];
            Assert.Equal("Project", projectWorkEstimate.Identifier);
            Assert.InRange(projectWorkEstimate.EstimatedNumberOfWorkingDaysRequired, lowerBoundExpectedNumberOfDaysRequired, upperBoundExpectedNumberOfDaysRequired);
            AssertEstimateIsDeterminate(projectWorkEstimate);
        }

        public static IEnumerable<object[]> MultiMetricRandomisedSelectionEstimationScenarios
        {
            get
            {
                yield return new object[] { ToThroughput(2, 2).ToArray(), new Project(10), new Queue<int>(new[] { 0, 1, 0, 1, 0 }), 5.0 };
                yield return new object[] { ToThroughput(1, 3).ToArray(), new Project(10), new Queue<int>(new[] { 0, 1, 0, 1, 0, 0 }), 6.0 };
                yield return new object[] { ToThroughput(1, 3).ToArray(), new Project(10), new Queue<int>(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }), 10.0 };
                yield return new object[] { ToThroughput(1, 3).ToArray(), new Project(10), new Queue<int>(new[] { 1, 1, 1, 1 }), 3.333333 };
            }
        }

        [Theory]
        [MemberData(nameof(MultiMetricRandomisedSelectionEstimationScenarios))]
        public void GIVEN_multiple_input_metrics_AND_one_project_WHEN_estimating_time_to_completion_THEN_returned_number_of_work_days_depends_on_randomly_selected_metrics(
            IReadOnlyCollection<ThroughputPerDay> throughputs, Project project, Queue<int> selectedIndices, double expectedNumberOfDaysRequired)
        {
            var inputMetrics = ToInputMetrics(throughputs);
            var roadmap = new Roadmap(new[] { project });

            randomNumberGeneratorMock
                .Setup(rng => rng.GetRandomIndex(throughputs.Count))
                .Returns(selectedIndices.Dequeue);

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            var estimations = estimator.Estimate(roadmap);

            Assert.Equal(2, estimations.Count);

            WorkEstimate roadmapWorkEstimate = estimations[0];
            Assert.Equal("Roadmap", roadmapWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedNumberOfDaysRequired, roadmapWorkEstimate);
            AssertEstimateIsDeterminate(roadmapWorkEstimate);

            WorkEstimate projectWorkEstimate = estimations[1];
            Assert.Equal("Project", projectWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedNumberOfDaysRequired, projectWorkEstimate);
            AssertEstimateIsDeterminate(projectWorkEstimate);
        }

        public static IEnumerable<object[]> MultiProjectRandomisedSelectionEstimationScenarios
        {
            get
            {
                yield return new object[] { new[] { ConvertToInputMetric(ToThroughput(2)) }, 3, 2, 1, 3 };
                yield return new object[] { new[] { ConvertToInputMetric(ToThroughput(2.6)) }, 2.357143, 2, 1, 2.357143 };
            }
        }

        [Theory]
        [MemberData(nameof(MultiProjectRandomisedSelectionEstimationScenarios))]
        public void GIVEN_single_input_metrics_AND_multiple_projects_WHEN_estimating_time_to_completion_THEN_returned_number_of_work_days_depends_on_randomly_selected_projects(
            InputMetric[] inputMetrics, double expectedEstimateRoadmap, double expectedEstimateProjectA, double expectedEstimateProjectB, double expectedEstimateProjectC)
        {
            var projectWithOneWorkItem = new Project(1, default, "A");
            var projectWithTwoWorkItems = new Project(2, default, "B");
            var projectWithThreeWorkItems = new Project(3, default, "C");
            var projects = new[] { projectWithOneWorkItem, projectWithTwoWorkItems, projectWithThreeWorkItems };
            var roadmap = new Roadmap(projects);

            var seletedProjectIndices = new Queue<int>(new[] { 1, 1, 2, 0, 2, 2 });
            randomNumberGeneratorMock
                .Setup(rng => rng.GetRandomIndex(projects.Length))
                .Returns(seletedProjectIndices.Dequeue);

            var estimator = new TimeTillCompletionEstimator(inputMetrics, randomNumberGeneratorMock.Object, someMaximumNumberOfIterations);

            var estimations = estimator.Estimate(roadmap);

            Assert.Equal(4, estimations.Count);

            WorkEstimate roadmapWorkEstimate = estimations[0];
            Assert.Equal("Roadmap", roadmapWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedEstimateRoadmap, roadmapWorkEstimate);
            AssertEstimateIsDeterminate(roadmapWorkEstimate);

            WorkEstimate projectAWorkEstimate = estimations[1];
            Assert.Equal("A", projectAWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedEstimateProjectA, projectAWorkEstimate);
            AssertEstimateIsDeterminate(projectAWorkEstimate);

            WorkEstimate projectBWorkEstimate = estimations[2];
            Assert.Equal("B", projectBWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedEstimateProjectB, projectBWorkEstimate);
            AssertEstimateIsDeterminate(projectBWorkEstimate);

            WorkEstimate projectCWorkEstimate = estimations[3];
            Assert.Equal("C", projectCWorkEstimate.Identifier);
            AssertExpectedNumberOfWorkingDaysIsEqual(expectedEstimateProjectC, projectCWorkEstimate);
            AssertEstimateIsDeterminate(projectCWorkEstimate);
        }

        private static IEnumerable<ThroughputPerDay> ToThroughput(params double[] throughputValues)
        {
            foreach(var throughputValue in throughputValues)
            {
                yield return ToThroughput(throughputValue);
            }
        }

        private static ThroughputPerDay ToThroughput(double d) =>
            new ThroughputPerDay(d);

        private static IReadOnlyList<InputMetric> ToInputMetrics(IEnumerable<ThroughputPerDay> throughputs) =>
            throughputs.Select(ConvertToInputMetric).ToArray();

        private static InputMetric ConvertToInputMetric(ThroughputPerDay throughput) =>
            new InputMetric { Throughput = throughput };

        private static void AssertExpectedNumberOfWorkingDaysIsEqual(double expected, WorkEstimate actual)
        {
            Assert.Equal(expected, actual.EstimatedNumberOfWorkingDaysRequired, 6);
        }

        private void AssertExpectedNumberOfWorkingDaysIsIndeterminate(WorkEstimate estimate)
        {
            Assert.True(estimate.IsIndeterminate);
        }

        private void AssertEstimateIsDeterminate(WorkEstimate estimate)
        {
            Assert.False(estimate.IsIndeterminate);
        }
    }
}
