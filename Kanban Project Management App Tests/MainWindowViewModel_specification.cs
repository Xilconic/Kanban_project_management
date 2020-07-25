using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class MainWindowViewModel_specification
    {
        private const string MeanOfCycleTimePropertyName = nameof(MainWindowViewModel.MeanOfCycleTime);

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
            public void THEN_the_input_metrics_is_an_empty_collection()
            {
                AssertInputMetricsAreEmpty(newViewModel);
            }

            [Fact]
            public void THEN_the_command_to_update_the_mean_of_cycle_time_is_initialized_AND_able_to_execute()
            {
                Assert.NotNull(newViewModel.UpdateMeanOfCycleTimeCommand);
                Assert.True(newViewModel.UpdateMeanOfCycleTimeCommand.CanExecute(null));
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
            public void WHEN_updating_mean_of_cycle_time_THEN_mean_of_cycle_time_is_null()
            {
                UpdateMeanOfCycleTime(viewModel);

                AssertMeanOfCycleTimeNull(viewModel);
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
            public void WHEN_updating_mean_of_cycle_time_THEN_mean_of_cycle_time_is_equals_to_the_cycle_time_of_that_metric_AND_change_has_been_notified()
            {
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);

                Assert.Equal(metric.CycleTime, viewModelWithOneInputMetric.MeanOfCycleTime);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(MeanOfCycleTimePropertyName);
            }

            [Fact]
            public void WHEN_updating_mean_of_cycle_time_multiple_times_consequtively_THEN_change_notification_only_happens_once()
            {
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);

                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(MeanOfCycleTimePropertyName);
            }

            [Fact]
            public void AND_mean_of_cylce_time_already_calculated_WHEN_clearing_input_metrics_and_recalculating_mean_of_cycle_time_THEN_mean_of_cycle_time_is_null_AND_change_has_been_notified()
            {
                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);

                viewModelWithOneInputMetric.InputMetrics.Clear();

                propertyChangedEventTracker.ClearAllRecordedEvents();

                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);

                AssertMeanOfCycleTimeNull(viewModelWithOneInputMetric);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(MeanOfCycleTimePropertyName);
            }
        }

        public class GIVEN_multiple_input_metrics_in_the_collection : IDisposable
        {
            private readonly MainWindowViewModel viewModelWithOneInputMetric;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_multiple_input_metrics_in_the_collection()
            {
                viewModelWithOneInputMetric = new MainWindowViewModel();

                propertyChangedEventTracker = new PropertyChangedEventTracker(viewModelWithOneInputMetric);
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

            [Theory]
            [MemberData(nameof(MeanOfCycleTimeCalculationTestCases))]
            public void WHEN_updating_mean_of_cycle_time_THEN_mean_of_cycle_time_is_equals_to_the_mean_of_the_cycle_times_of_those_metrics_AND_change_has_been_notified(
                IReadOnlyCollection<TimeSpan> cycleTimes, TimeSpan expectedMeanOfCycleTimes)
            {
                AddInputMetricsToViewModel(cycleTimes.Select(CycleTimeToInputMetric));

                UpdateMeanOfCycleTime(viewModelWithOneInputMetric);

                Assert.Equal(expectedMeanOfCycleTimes, viewModelWithOneInputMetric.MeanOfCycleTime);
                propertyChangedEventTracker.AssertOnlyOnePropertyChangeNotificationHappenedForName(MeanOfCycleTimePropertyName);
            }

            private void AddInputMetricsToViewModel(IEnumerable<InputMetric> metrics)
            {
                foreach(var metric in metrics)
                {
                    viewModelWithOneInputMetric.InputMetrics.Add(metric);
                }
            }
        }

        private static InputMetric CycleTimeToInputMetric(TimeSpan cycleTime) => new InputMetric { CycleTime = cycleTime };

        private static void UpdateMeanOfCycleTime(MainWindowViewModel vm)
        {
            vm.UpdateMeanOfCycleTimeCommand.Execute(null);
        }

        private static void AssertMeanOfCycleTimeNull(MainWindowViewModel vm)
        {
            Assert.Null(vm.MeanOfCycleTime);
        }

        private static void AssertInputMetricsAreEmpty(MainWindowViewModel vm)
        {
            Assert.Empty(vm.InputMetrics);
        }
    }
}
