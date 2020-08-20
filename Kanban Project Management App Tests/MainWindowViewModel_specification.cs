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

namespace KanbanProjectManagementApp.Tests
{
    public class MainWindowViewModel_specification
    {
        private const string EstimatedMeanOfCycleTimePropertyName = nameof(MainWindowViewModel.EstimatedMeanOfCycleTime);
        private const string EstimatedCorrectedSampleStandardDeviationOfCycleTimePropertyName = nameof(MainWindowViewModel.EstimatedCorrectedSampleStandardDeviationOfCycleTime);

        public class GIVEN_a_newly_constructed_view_model
        {
            private readonly MainWindowViewModel newViewModel;

            public GIVEN_a_newly_constructed_view_model()
            {
                newViewModel = new MainWindowViewModel();
            }

            [Fact]
            public void THEN_the_mean_of_cycle_time_is_null()
            {
                AssertMeanOfCycleTimeNull(newViewModel);
            }

            [Fact]
            public void THEN_then_the_corrected_sample_standard_deviation_is_null()
            {
                AssertEstimatedCorrectedSampleStandardDeviationOfCycleTimeNull(newViewModel);
            }

            [Fact]
            public void THEN_the_input_metrics_is_an_empty_collection()
            {
                AssertInputMetricsAreEmpty(newViewModel);
            }

            [Fact]
            public void THEN_the_command_to_update_cycle_time_statistics_is_initialized_AND_able_to_execute()
            {
                Assert.NotNull(newViewModel.UpdateCycleTimeStatisticsCommand);
                Assert.True(newViewModel.UpdateCycleTimeStatisticsCommand.CanExecute(null));
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
            public void WHEN_updating_cycle_time_statistics_THEN_mean_of_cycle_time_is_null()
            {
                UpdateCycleTimeStatistics(viewModel);

                AssertMeanOfCycleTimeNull(viewModel);
            }

            [Fact]
            public void WHEN_updating_cycle_time_statistics_THEN_corrected_sample_standard_deviation_of_cycle_time_is_null()
            {
                UpdateCycleTimeStatistics(viewModel);

                AssertEstimatedCorrectedSampleStandardDeviationOfCycleTimeNull(viewModel);
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

                metric = CycleTimeToInputMetric(TimeSpan.FromHours(2));
                viewModelWithOneInputMetric.InputMetrics.Add(metric);

                propertyChangedEventTracker = new PropertyChangedEventTracker(viewModelWithOneInputMetric);
            }

            public void Dispose()
            {
                propertyChangedEventTracker.Dispose();
            }

            [Fact]
            public void WHEN_updating_cycle_time_statistics_THEN_mean_of_cycle_time_is_equals_to_the_cycle_time_of_that_metric_AND_change_has_been_notified()
            {
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                Assert.Equal(metric.CycleTime, viewModelWithOneInputMetric.EstimatedMeanOfCycleTime);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfCycleTimePropertyName);
            }

            [Fact]
            public void AND_mean_of_cylce_time_already_calculated_WHEN_clearing_input_metrics_and_recalculating_cycle_time_statistics_THEN_mean_of_cycle_time_is_null_AND_change_has_been_notified()
            {
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                viewModelWithOneInputMetric.InputMetrics.Clear();

                propertyChangedEventTracker.ClearAllRecordedEvents();

                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                AssertMeanOfCycleTimeNull(viewModelWithOneInputMetric);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfCycleTimePropertyName);
            }

            [Fact]
            public void WHEN_updating_cycle_time_statistics_THEN_corrected_sample_standard_deviation_is_zero_AND_change_has_been_notified()
            {
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                Assert.Equal(TimeSpan.Zero, viewModelWithOneInputMetric.EstimatedCorrectedSampleStandardDeviationOfCycleTime);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfCycleTimePropertyName);
            }

            [Fact]
            public void AND_corrected_sample_standard_deviation_of_cylce_time_already_calculated_WHEN_clearing_input_metrics_and_recalculating_cycle_time_statistics_THEN_corrected_sample_standard_deviation_of_cycle_time_is_null_AND_change_has_been_notified()
            {
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                viewModelWithOneInputMetric.InputMetrics.Clear();

                propertyChangedEventTracker.ClearAllRecordedEvents();

                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                AssertEstimatedCorrectedSampleStandardDeviationOfCycleTimeNull(viewModelWithOneInputMetric);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfCycleTimePropertyName);
            }

            [Fact]
            public void WHEN_updating_cycle_time_statistics_multiple_times_consequtively_THEN_change_notification_only_happens_once_for_each_statistics_property()
            {
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);
                UpdateCycleTimeStatistics(viewModelWithOneInputMetric);

                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfCycleTimePropertyName);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfCycleTimePropertyName);
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

            public static IEnumerable<object[]> MeanOfCycleTimeCalculationTestCases
            {
                get
                {
                    yield return new object[] { new[] { TimeSpan.Zero, TimeSpan.Zero }, TimeSpan.Zero };
                    yield return new object[] { new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, new TimeSpan(0, 0, 0, 1, 500) };
                    yield return new object[] { new[] { TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(4) }, new TimeSpan(0, 3, 30) };
                    yield return new object[] { new[] { TimeSpan.FromHours(5), TimeSpan.FromHours(6) }, new TimeSpan(5, 30, 0) };
                    yield return new object[] { new[] { TimeSpan.FromDays(7), TimeSpan.FromDays(8) }, new TimeSpan(7, 12, 0, 0) };
                }
            }

            public static IEnumerable<object[]> CorrectedSampleStandardDeviationOfCycleTimeCalculationTestCases
            {
                get
                {
                    // Note: Using Google Sheets STDEVA function as test oracle:
                    yield return new object[] { new[] { TimeSpan.Zero, TimeSpan.Zero }, TimeSpan.Zero };
                    yield return new object[] { new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) }, TimeSpan.Zero };
                    yield return new object[] { new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, new TimeSpan(0, 0, 0, 0, 707) };
                    yield return new object[] { new[] { TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5) }, new TimeSpan(0, 0, 1, 24, 853) };
                    yield return new object[] { new[] { TimeSpan.FromHours(6), TimeSpan.FromHours(9) }, new TimeSpan(0, 2, 7, 16, 753) };
                    yield return new object[] { new[] { TimeSpan.FromDays(10), TimeSpan.FromDays(20), TimeSpan.FromDays(30) }, new TimeSpan(10, 0, 0, 0) };
                    yield return new object[] { new[] { TimeSpan.FromDays(80), TimeSpan.FromDays(90), TimeSpan.FromDays(100) }, new TimeSpan(10, 0, 0, 0) };
                    yield return new object[] { new[] { TimeSpan.FromDays(3), TimeSpan.FromDays(7), TimeSpan.FromDays(4) }, new TimeSpan(2, 1, 57, 35, 942) };
                }
            }

            [Theory]
            [MemberData(nameof(MeanOfCycleTimeCalculationTestCases))]
            public void WHEN_updating_mean_of_cycle_time_THEN_mean_of_cycle_time_is_equals_to_the_mean_of_the_cycle_times_of_those_metrics_AND_change_has_been_notified(
                IReadOnlyCollection<TimeSpan> cycleTimes, TimeSpan expectedMeanOfCycleTimes)
            {
                AddInputMetricsToViewModel(cycleTimes.Select(CycleTimeToInputMetric));

                UpdateCycleTimeStatistics(viewModelWithMultipleInputMetrics);

                Assert.Equal(expectedMeanOfCycleTimes, viewModelWithMultipleInputMetrics.EstimatedMeanOfCycleTime);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfCycleTimePropertyName);
            }

            [Theory]
            [MemberData(nameof(CorrectedSampleStandardDeviationOfCycleTimeCalculationTestCases))]
            public void  WHEN_updating_cycle_time_statistics_THEN_corrected_sample_standard_deviation_of_the_cycle_time_has_been_calculated_correctly_AND_change_has_been_notified(
                IReadOnlyCollection<TimeSpan> cycleTimes, TimeSpan expectedCorrectedSampleStandardDeviation)
            {
                AddInputMetricsToViewModel(cycleTimes.Select(CycleTimeToInputMetric));

                UpdateCycleTimeStatistics(viewModelWithMultipleInputMetrics);

                // Note: Doing calculations with doubles, so need to take into account limited precision:
                var acceptedErrorMargin = TimeSpan.FromMilliseconds(1);
                var lowerBound = expectedCorrectedSampleStandardDeviation - acceptedErrorMargin;
                var upperBound = expectedCorrectedSampleStandardDeviation + acceptedErrorMargin;
                Assert.InRange(viewModelWithMultipleInputMetrics.EstimatedCorrectedSampleStandardDeviationOfCycleTime.Value, lowerBound, upperBound);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedCorrectedSampleStandardDeviationOfCycleTimePropertyName);
            }

            private void AddInputMetricsToViewModel(IEnumerable<InputMetric> metrics)
            {
                foreach(var metric in metrics)
                {
                    viewModelWithMultipleInputMetrics.InputMetrics.Add(metric);
                }
            }
        }

        private static InputMetric CycleTimeToInputMetric(TimeSpan cycleTime) => new InputMetric { CycleTime = cycleTime };

        private static void UpdateCycleTimeStatistics(MainWindowViewModel vm)
        {
            vm.UpdateCycleTimeStatisticsCommand.Execute(null);
        }

        private static void AssertMeanOfCycleTimeNull(MainWindowViewModel vm)
        {
            Assert.Null(vm.EstimatedMeanOfCycleTime);
        }

        private static void AssertEstimatedCorrectedSampleStandardDeviationOfCycleTimeNull(MainWindowViewModel vm)
        {
            Assert.Null(vm.EstimatedCorrectedSampleStandardDeviationOfCycleTime);
        }

        private static void AssertInputMetricsAreEmpty(MainWindowViewModel vm)
        {
            Assert.Empty(vm.InputMetrics);
        }
    }
}
