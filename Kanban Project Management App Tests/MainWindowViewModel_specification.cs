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
            public void THEN_then_the_corrected_sample_standard_deviation_is_null()
            {
                AssertEstimatedCorrectedSampleStandardDeviationOfThroughputNull(newViewModel);
            }

            [Fact]
            public void THEN_the_input_metrics_is_an_empty_collection()
            {
                AssertInputMetricsAreEmpty(newViewModel);
            }

            [Fact]
            public void THEN_the_command_to_update_throughput_statistics_is_initialized_AND_able_to_execute()
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
        }

        public class GIVEN_one_input_metric_in_the_collection : IDisposable
        {
            private readonly MainWindowViewModel viewModelWithOneInputMetric;
            private readonly InputMetric metric;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_one_input_metric_in_the_collection()
            {
                viewModelWithOneInputMetric = new MainWindowViewModel();

                metric = ThroughputToInputMetric(TimeSpan.FromHours(2));
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

                Assert.Equal(TimeSpan.Zero, viewModelWithOneInputMetric.EstimatedCorrectedSampleStandardDeviationOfThroughput);
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
                    yield return new object[] { new[] { TimeSpan.Zero, TimeSpan.Zero }, TimeSpan.Zero };
                    yield return new object[] { new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, new TimeSpan(0, 0, 0, 1, 500) };
                    yield return new object[] { new[] { TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(4) }, new TimeSpan(0, 3, 30) };
                    yield return new object[] { new[] { TimeSpan.FromHours(5), TimeSpan.FromHours(6) }, new TimeSpan(5, 30, 0) };
                    yield return new object[] { new[] { TimeSpan.FromDays(7), TimeSpan.FromDays(8) }, new TimeSpan(7, 12, 0, 0) };
                }
            }

            public static IEnumerable<object[]> CorrectedSampleStandardDeviationOfThroughputCalculationTestCases
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
            [MemberData(nameof(MeanOfThroughputCalculationTestCases))]
            public void WHEN_updating_mean_of_throughput_THEN_mean_of_throughput_is_equals_to_the_mean_of_the_throughputs_of_those_metrics_AND_change_has_been_notified(
                IReadOnlyCollection<TimeSpan> cycleTimes, TimeSpan expectedMeanOfCycleTimes)
            {
                AddInputMetricsToViewModel(cycleTimes.Select(ThroughputToInputMetric));

                UpdateThroughputStatistics(viewModelWithMultipleInputMetrics);

                Assert.Equal(expectedMeanOfCycleTimes, viewModelWithMultipleInputMetrics.EstimatedMeanOfThroughput);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(EstimatedMeanOfThroughputPropertyName);
            }

            [Theory]
            [MemberData(nameof(CorrectedSampleStandardDeviationOfThroughputCalculationTestCases))]
            public void  WHEN_updating_throughput_statistics_THEN_corrected_sample_standard_deviation_of_the_throughput_has_been_calculated_correctly_AND_change_has_been_notified(
                IReadOnlyCollection<TimeSpan> cycleTimes, TimeSpan expectedCorrectedSampleStandardDeviation)
            {
                AddInputMetricsToViewModel(cycleTimes.Select(ThroughputToInputMetric));

                UpdateThroughputStatistics(viewModelWithMultipleInputMetrics);

                // Note: Doing calculations with doubles, so need to take into account limited precision:
                var acceptedErrorMargin = TimeSpan.FromMilliseconds(1);
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

        private static InputMetric ThroughputToInputMetric(TimeSpan cycleTime) => new InputMetric { Throughput = cycleTime };

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
    }
}
