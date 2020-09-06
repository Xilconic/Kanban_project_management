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

            void call() => estimator.Estimate(1);
            var actualException = Assert.Throws<InvalidOperationException>(call);
            Assert.Equal("At least 1 datapoint of input metrics is required for estimation.", actualException.Message);
        }

        public static IEnumerable<object[]> InvalidNumberOfWorkItemsToCompleteScenarios
        {
            get
            {
                yield return new object[] { int.MinValue };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfWorkItemsToCompleteScenarios))]
        public void GIVEN_invalid_number_of_work_items_to_be_completed_WHEN_estimating_THEN_throw_ArgumentOutOfRangeException(
            int invalidNumberOfWorkItemsToComplete)
        {
            var inputMetrics = new[] { new InputMetric { Throughput = new ThroughputPerDay(2) } };
            var estimator = new MonteCarloTimeTillCompletionEstimator(1, 1, inputMetrics);

            void call() => estimator.Estimate(invalidNumberOfWorkItemsToComplete);
            AssertActionThrowsArgumentOutOfRangeException(call, "numberOfWorkItemsToComplete", "Number of workitems to complete should be at least 1.");
        }

        [Fact]
        public void GIVEN_one_input_metric_AND_two_simulations_WHEN_estimating_THEN_return_two_work_estimates()
        {
            var inputMetrics = new[]
            {
                new InputMetric { Throughput = new ThroughputPerDay(2) }
            };
            var estimator = new MonteCarloTimeTillCompletionEstimator(2, 10, inputMetrics);

            var estimations = estimator.Estimate(10);
            Assert.Equal(2, estimations.Count);
            Assert.Collection(estimations,
                estimate1 => Assert.False(estimate1.IsIndeterminate),
                estimate2 => Assert.False(estimate2.IsIndeterminate));
            Assert.Collection(estimations,
                estimate1 => Assert.Equal(5.0, estimate1.EstimatedNumberOfWorkingDaysRequired),
                estimate2 => Assert.Equal(5.0, estimate2.EstimatedNumberOfWorkingDaysRequired));
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
