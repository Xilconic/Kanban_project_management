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
    }
}
