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
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using KanbanProjectManagementApp.Domain;
using System.Windows.Input;

namespace KanbanProjectManagementApp.Tests
{
    public class MainWindowViewModel_specification
    {
        private const string EstimatedMeanOfThroughputPropertyName = nameof(MainWindowViewModel.EstimatedMeanOfThroughput);
        private const string EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName = nameof(MainWindowViewModel.EstimatedCorrectedSampleStandardDeviationOfThroughput);

        public class GIVEN_a_newly_constructed_view_model
        {
            private readonly MainWindowViewModel newViewModel;

            public GIVEN_a_newly_constructed_view_model()
            {
                newViewModel = new MainWindowViewModel();
            }

            [Fact]
            public void THEN_the_mean_of_throughput_is_null()
            {
                AssertMeanOfThroughputNull(newViewModel);
            }

            [Fact]
            public void THEN_the_corrected_sample_standard_deviation_is_null()
            {
                AssertEstimatedCorrectedSampleStandardDeviationOfThroughputNull(newViewModel);
            }

            [Fact]
            public void THEN_the_input_metrics_is_an_empty_collection()
            {
                AssertInputMetricsAreEmpty(newViewModel);
            }

            [Fact]
            public void THEN_the_number_of_work_items_to_be_completed_is_ten()
            {
                Assert.Equal(10, newViewModel.NumberOfWorkItemsToBeCompleted);
            }

            [Fact]
            public void THEN_the_number_of_simulations_is_ten()
            {
                Assert.Equal(10, newViewModel.NumberOfMonteCarloSimulations);
            }

            [Fact]
            public void THEN_the_maximum_number_of_iterations_is_twentyfive()
            {
                Assert.Equal(25, newViewModel.MaximumNumberOfIterations);
            }

            [Fact]
            public void THEN_the_number_of_working_days_till_completion_estimations_is_an_empty_collection()
            {
                Assert.Empty(newViewModel.NumberOfWorkingDaysTillCompletionEstimations);
            }

            [Fact]
            public void THEN_the_command_to_update_throughput_statistics_is_initialized_AND_able_to_execute()
            {
                Assert.NotNull(newViewModel.UpdateCycleTimeStatisticsCommand);
                Assert.True(newViewModel.UpdateCycleTimeStatisticsCommand.CanExecute(null));
            }

            [Fact]
            public void THEN_the_command_to_estimate_to_complete_work_items_is_initialized_AND_not_able_to_execute()
            {
                Assert.NotNull(newViewModel.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand);
                Assert.False(newViewModel.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.CanExecute(null));
            }

            public static IEnumerable<object[]> InvalidNumberOfWorkItemsToBecompletedScenarios
            {
                get
                {
                    yield return new object[] { int.MinValue };
                    yield return new object[] { 0 };
                }
            }

            [Theory]
            [MemberData(nameof(InvalidNumberOfWorkItemsToBecompletedScenarios))]
            public void WHEN_setting_invalid_number_of_work_items_to_be_completed_THEN_throw_ArgumentOutOfRangeException(
                int invalidNumberOfWorkItems)
            {
                void call() => newViewModel.NumberOfWorkItemsToBeCompleted = invalidNumberOfWorkItems;

                AssertActionThrowsArgumentOutOfRangeException(call, "value", "Number of work items to be completed must be at least 1.");
            }

            public static IEnumerable<object[]> InvalidNumberOfMonteCarloSimulationsScenarions
            {
                get
                {
                    yield return new object[] { int.MinValue };
                    yield return new object[] { 0 };
                }
            }

            [Theory]
            [MemberData(nameof(InvalidNumberOfMonteCarloSimulationsScenarions))]
            public void WHEN_setting_invalid_number_of_simulations_THEN_throw_ArgumentOutOfRangeException(
                int invalidNumberOfMonteCarloSimulations)
            {
                void call() => newViewModel.NumberOfMonteCarloSimulations = invalidNumberOfMonteCarloSimulations;

                AssertActionThrowsArgumentOutOfRangeException(call, "value", "Number of simulation should be at least 1.");
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
            public void WHEN_setting_invalid_maximum_number_of_iterations_THEN_throw_ArgumentOutOfRangeException(
                int invalidMaximumNumberOfIterations)
            {
                void call() => newViewModel.MaximumNumberOfIterations = invalidMaximumNumberOfIterations;

                AssertActionThrowsArgumentOutOfRangeException(call, "value", "Maximum number of iterations should be at least 1.");
            }

            private static void AssertActionThrowsArgumentOutOfRangeException(
                Action call,
                string expectedParamName,
                string expectedMessage)
            {
                var actualException = Assert.Throws<ArgumentOutOfRangeException>(call);
                Assert.Equal(expectedParamName, actualException.ParamName);
                Assert.StartsWith(expectedMessage, actualException.Message);
            }
        }

        public class GIVEN_an_empty_InputMetrics_collection
        {
            private readonly MainWindowViewModel viewModel;

            public GIVEN_an_empty_InputMetrics_collection()
            {
                viewModel = new MainWindowViewModel();
                AssertInputMetricsAreEmpty(viewModel);
            }

            [Fact]
            public void WHEN_updating_throughput_statistics_THEN_mean_of_throughput_is_null()
            {
                UpdateThroughputStatistics(viewModel);

                AssertMeanOfThroughputNull(viewModel);
            }

            [Fact]
            public void WHEN_updating_throughput_statistics_THEN_corrected_sample_standard_deviation_of_throughput_is_null()
            {
                UpdateThroughputStatistics(viewModel);

                AssertEstimatedCorrectedSampleStandardDeviationOfThroughputNull(viewModel);
            }

            [Fact]
            public void WHEN_estimating_completion_time_of_work_items_THEN_throw_InvalidOperationException()
            {
                void call() => viewModel.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                Assert.Throws<InvalidOperationException>(call);
            }

            [Fact]
            public void WHEN_adding_input_metric_THEN_command_to_estimate_completion_time_of_work_items_becomes_enabled()
            {
                AssertThatCommandStateIsAsExpectedWhenActionIsPerformed(
                    viewModel.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand,
                    () => viewModel.InputMetrics.Add(new InputMetric()),
                    true,
                    1);
            }
        }

        public class GIVEN_one_input_metric_in_the_collection : IDisposable
        {
            private readonly MainWindowViewModel viewModelWithOneInputMetric;
            private readonly InputMetric metric;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_one_input_metric_in_the_collection()
            {
                viewModelWithOneInputMetric = new MainWindowViewModel();

                metric = ThroughputToInputMetric(new ThroughputPerDay(2));
                viewModelWithOneInputMetric.InputMetrics.Add(metric);

                propertyChangedEventTracker = new PropertyChangedEventTracker(viewModelWithOneInputMetric);
            }

            public void Dispose()
            {
                propertyChangedEventTracker.Dispose();
            }

            [Fact]
            public void WHEN_updating_throughput_statistics_THEN_mean_of_throughput_is_equals_to_the_throughput_of_that_metric_AND_change_has_been_notified()
            {
                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                Assert.Equal(metric.Throughput, viewModelWithOneInputMetric.EstimatedMeanOfThroughput);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfThroughputPropertyName);
            }

            [Fact]
            public void AND_mean_of_cylce_time_already_calculated_WHEN_clearing_input_metrics_and_recalculating_throughput_statistics_THEN_mean_of_throughput_is_null_AND_change_has_been_notified()
            {
                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                viewModelWithOneInputMetric.InputMetrics.Clear();

                propertyChangedEventTracker.ClearAllRecordedEvents();

                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                AssertMeanOfThroughputNull(viewModelWithOneInputMetric);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfThroughputPropertyName);
            }

            [Fact]
            public void WHEN_updating_throughput_statistics_THEN_corrected_sample_standard_deviation_is_zero_AND_change_has_been_notified()
            {
                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                Assert.Equal(0.0, viewModelWithOneInputMetric.EstimatedCorrectedSampleStandardDeviationOfThroughput);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName);
            }

            [Fact]
            public void AND_corrected_sample_standard_deviation_of_cylce_time_already_calculated_WHEN_clearing_input_metrics_and_recalculating_throughput_statistics_THEN_corrected_sample_standard_deviation_of_throughput_is_null_AND_change_has_been_notified()
            {
                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                viewModelWithOneInputMetric.InputMetrics.Clear();

                propertyChangedEventTracker.ClearAllRecordedEvents();

                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                AssertEstimatedCorrectedSampleStandardDeviationOfThroughputNull(viewModelWithOneInputMetric);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName);
            }

            [Fact]
            public void WHEN_updating_throughput_statistics_multiple_times_consequtively_THEN_change_notification_only_happens_once_for_each_statistics_property()
            {
                UpdateThroughputStatistics(viewModelWithOneInputMetric);
                UpdateThroughputStatistics(viewModelWithOneInputMetric);
                UpdateThroughputStatistics(viewModelWithOneInputMetric);
                UpdateThroughputStatistics(viewModelWithOneInputMetric);

                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfThroughputPropertyName);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName);
            }

            [Fact]
            public void WHEN_adding_another_input_metric_THEN_the_enabled_state_of_the_command_to_estimate_completion_time_of_work_items_remains_unchanged()
            {
                AssertThatCommandStateIsAsExpectedWhenActionIsPerformed(
                    viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand,
                    () => viewModelWithOneInputMetric.InputMetrics.Add(new InputMetric()),
                    true,
                    0);
            }

            [Fact]
            public void WHEN_removing_the_input_metric_THEN_the_command_to_estimate_completion_time_of_work_items_becomes_disabled()
            {
                AssertThatCommandStateIsAsExpectedWhenActionIsPerformed(
                    viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand,
                    () => viewModelWithOneInputMetric.InputMetrics.Clear(),
                    false,
                    1);
            }

            public static IEnumerable<object[]> EstimateTimeTillCompletionScenarios
            {
                get
                {
                    yield return new object[] { 10, new ThroughputPerDay(2), new WorkEstimate(5.0, false) };
                }
            }

            [Theory]
            [MemberData(nameof(EstimateTimeTillCompletionScenarios))]
            public void WHEN_estimating_completion_time_of_work_items_THEN_estimated_number_of_working_days_till_completion_updated(
                int numberOfWorkItemsToBeCompleted, ThroughputPerDay throughput, WorkEstimate expectedEstimate)
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = throughput };
                viewModelWithOneInputMetric.NumberOfWorkItemsToBeCompleted = numberOfWorkItemsToBeCompleted;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                void assertEstimateHasExpectedNumberOfWorkingDays(WorkEstimate estimate) => Assert.Equal(expectedEstimate.EstimatedNumberOfWorkingDaysRequired, estimate.EstimatedNumberOfWorkingDaysRequired);
                Assert.Collection(viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays,
                    assertEstimateHasExpectedNumberOfWorkingDays
                );

                static void assertEstimateIsDeterminate(WorkEstimate estimate) => Assert.False(estimate.IsIndeterminate);
                Assert.Collection(viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate,
                    assertEstimateIsDeterminate
                );
            }

            [Fact]
            public void WHEN_estimating_completion_time_of_work_items_multiple_times_THEN_the_number_of_working_days_till_completion_are_updated()
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                viewModelWithOneInputMetric.NumberOfWorkItemsToBeCompleted = 12;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                Assert.Equal(viewModelWithOneInputMetric.NumberOfMonteCarloSimulations, viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations.Count);
            }
        }

        public class GIVEN_multiple_input_metrics_in_the_collection : IDisposable
        {
            private readonly MainWindowViewModel viewModelWithMultipleInputMetrics;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_multiple_input_metrics_in_the_collection()
            {
                viewModelWithMultipleInputMetrics = new MainWindowViewModel();

                propertyChangedEventTracker = new PropertyChangedEventTracker(viewModelWithMultipleInputMetrics);
            }

            public void Dispose()
            {
                propertyChangedEventTracker.Dispose();
            }

            public static IEnumerable<object[]> MeanOfThroughputCalculationTestCases
            {
                get
                {
                    yield return new object[] { new[] { new ThroughputPerDay(0), new ThroughputPerDay(0) }, new ThroughputPerDay(0) };
                    yield return new object[] { new[] { new ThroughputPerDay(1), new ThroughputPerDay(2) }, new ThroughputPerDay(1.5) };
                }
            }

            [Theory]
            [MemberData(nameof(MeanOfThroughputCalculationTestCases))]
            public void WHEN_updating_mean_of_throughput_THEN_mean_of_throughput_is_equals_to_the_mean_of_the_throughputs_of_those_metrics_AND_change_has_been_notified(
                IReadOnlyCollection<ThroughputPerDay> throughput, ThroughputPerDay expectedMeanOfThroughput)
            {
                AddInputMetricsToViewModel(throughput.Select(ThroughputToInputMetric));

                UpdateThroughputStatistics(viewModelWithMultipleInputMetrics);

                Assert.Equal(expectedMeanOfThroughput, viewModelWithMultipleInputMetrics.EstimatedMeanOfThroughput);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfThroughputPropertyName);
            }

            public static IEnumerable<object[]> CorrectedSampleStandardDeviationOfThroughputCalculationTestCases
            {
                get
                {
                    // Note: Using Google Sheets STDEVA function as test oracle:
                    yield return new object[] { new[] { new ThroughputPerDay(0), new ThroughputPerDay(0) }, 0.0 };
                    yield return new object[] { new[] { new ThroughputPerDay(1), new ThroughputPerDay(2) }, 0.7071067812 };
                    yield return new object[] { new[] { new ThroughputPerDay(0), new ThroughputPerDay(4) }, 2.828427125 };
                    yield return new object[] { new[] { new ThroughputPerDay(1), new ThroughputPerDay(2), new ThroughputPerDay(3) }, 1.0 };
                    yield return new object[] { Enumerable.Range(1, 10).Select(i => new ThroughputPerDay(i)).ToArray(), 3.027650354 };
                }
            }

            [Theory]
            [MemberData(nameof(CorrectedSampleStandardDeviationOfThroughputCalculationTestCases))]
            public void  WHEN_updating_throughput_statistics_THEN_corrected_sample_standard_deviation_of_the_throughput_has_been_calculated_correctly_AND_change_has_been_notified(
                IReadOnlyCollection<ThroughputPerDay> throughputs, double expectedCorrectedSampleStandardDeviation)
            {
                AddInputMetricsToViewModel(throughputs.Select(ThroughputToInputMetric));

                UpdateThroughputStatistics(viewModelWithMultipleInputMetrics);

                // Note: Doing calculations with doubles, so need to take into account limited precision:
                var acceptedErrorMargin = 1e-6;
                var lowerBound = expectedCorrectedSampleStandardDeviation - acceptedErrorMargin;
                var upperBound = expectedCorrectedSampleStandardDeviation + acceptedErrorMargin;
                Assert.InRange(viewModelWithMultipleInputMetrics.EstimatedCorrectedSampleStandardDeviationOfThroughput.Value, lowerBound, upperBound);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName);
            }

            private void AddInputMetricsToViewModel(IEnumerable<InputMetric> metrics)
            {
                foreach(var metric in metrics)
                {
                    viewModelWithMultipleInputMetrics.InputMetrics.Add(metric);
                }
            }
        }

        private static InputMetric ThroughputToInputMetric(ThroughputPerDay throughput) => new InputMetric { Throughput = throughput };

        private static void UpdateThroughputStatistics(MainWindowViewModel vm)
        {
            vm.UpdateCycleTimeStatisticsCommand.Execute(null);
        }

        private static void AssertMeanOfThroughputNull(MainWindowViewModel vm)
        {
            Assert.Null(vm.EstimatedMeanOfThroughput);
        }

        private static void AssertEstimatedCorrectedSampleStandardDeviationOfThroughputNull(MainWindowViewModel vm)
        {
            Assert.Null(vm.EstimatedCorrectedSampleStandardDeviationOfThroughput);
        }

        private static void AssertInputMetricsAreEmpty(MainWindowViewModel vm)
        {
            Assert.Empty(vm.InputMetrics);
        }

        private static void AssertThatCommandStateIsAsExpectedWhenActionIsPerformed(
            ICommand command,
            Action call,
            bool expectedEnabledState,
            int numberOfExpectedEnabledStateChanges)
        {
            int numberOfInvocations = 0;
            void CountNumberOfInvocations(object sender, EventArgs args)
            {
                numberOfInvocations++;
            }

            command.CanExecuteChanged += CountNumberOfInvocations;
            try
            {
                call();

                Assert.Equal(numberOfExpectedEnabledStateChanges, numberOfInvocations);
                Assert.Equal(expectedEnabledState, command.CanExecute(null));
            }
            finally
            {
                command.CanExecuteChanged -= CountNumberOfInvocations;
            }
        }
    }
}
