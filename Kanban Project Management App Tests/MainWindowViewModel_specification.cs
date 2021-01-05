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
using Moq;
using System.Collections.Specialized;
using KanbanProjectManagementApp.Tests.TestUtilities;
using KanbanProjectManagementApp.ViewModels;

namespace KanbanProjectManagementApp.Tests
{
    public class MainWindowViewModel_specification
    {
        private const string EstimatedMeanOfThroughputPropertyName = nameof(MainWindowViewModel.EstimatedMeanOfThroughput);
        private const string EstimatedCorrectedSampleStandardDeviationOfThroughputPropertyName = nameof(MainWindowViewModel.EstimatedCorrectedSampleStandardDeviationOfThroughput);
        private const string NumberOfWorkingDaysTillCompletionEstimationsPropertyName = nameof(MainWindowViewModel.NumberOfWorkingDaysTillCompletionEstimations);

        public class WHEN_constructing_view_model
        {
            [Fact]
            public void AND_file_location_getter_is_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new MainWindowViewModel(
                    null,
                    new Mock<IFileToReadGetter>().Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

                Assert.Throws<ArgumentNullException>("fileLocationToSaveGetter", call);
            }

            [Fact]
            public void AND_file_to_read_getter_is_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    null,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

                Assert.Throws<ArgumentNullException>("fileToReadGetter", call);
            }

            [Fact]
            public void AND_work_estimation_file_exporter_is_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    new Mock<IFileToReadGetter>().Object,
                    null,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

                Assert.Throws<ArgumentNullException>("workEstimationsFileExporter", call);
            }

            [Fact]
            public void AND_input_metrics_file_importer_is_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    new Mock<IFileToReadGetter>().Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    null,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

                Assert.Throws<ArgumentNullException>("inputMetricsFileImporter", call);
            }

            [Fact]
            public void AND_user_confirmation_asker_is_null_THEN_throw_ArgumentNullException()
            {
                static void call() => new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    new Mock<IFileToReadGetter>().Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    null);

                Assert.Throws<ArgumentNullException>("confirmationAsker", call);
            }
        }

        public class GIVEN_a_newly_constructed_view_model
        {
            private readonly MainWindowViewModel newViewModel;

            public GIVEN_a_newly_constructed_view_model()
            {
                newViewModel = new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    new Mock<IFileToReadGetter>().Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);
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
                Assert.Equal(10, newViewModel.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted);
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
            public void THEN_the_number_of_working_days_till_completion_estimations_is_null()
            {
                Assert.Null(newViewModel.NumberOfWorkingDaysTillCompletionEstimations);
            }

            [Fact]
            public void THEN_the_command_to_import_throughput_metrics_is_initialized_AND_able_to_execute()
            {
                Assert.NotNull(newViewModel.ImportThroughputMetricsCommand);
                Assert.True(newViewModel.ImportThroughputMetricsCommand.CanExecute(null));
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

            [Fact]
            public void THEN_the_command_to_export_work_estimates_is_initialized_AND_not_able_to_execute()
            {
                Assert.NotNull(newViewModel.ExportWorkEstimatesCommand);
                Assert.False(newViewModel.ExportWorkEstimatesCommand.CanExecute(null));
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
                void call() => newViewModel.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = invalidNumberOfWorkItems;

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
                var actualException = Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, call);
                Assert.StartsWith(expectedMessage, actualException.Message);
            }
        }

        public class GIVEN_an_empty_InputMetrics_collection
        {
            private readonly Mock<IFileToReadGetter> fileToReadGetterMock = new Mock<IFileToReadGetter>();
            private readonly Mock<IInputMetricsFileImporter> inputMetricsFileImporterMock = new Mock<IInputMetricsFileImporter>();
            private readonly MainWindowViewModel viewModel;

            public GIVEN_an_empty_InputMetrics_collection()
            {
                viewModel = new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    fileToReadGetterMock.Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    inputMetricsFileImporterMock.Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);
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

            [Fact]
            public void WHEN_importing_some_metrics_THEN_input_metrics_are_changed()
            {
                string filePath = "c:/some/folder/and/someFile.csv";
                fileToReadGetterMock
                    .Setup(g => g.TryGetFileToRead(out filePath))
                    .Returns(true);

                var expectedMetrics = new[]
                {
                    new InputMetric { Throughput = new ThroughputPerDay(1.1) },
                    new InputMetric { Throughput = new ThroughputPerDay(2.2) },
                };

                inputMetricsFileImporterMock
                    .Setup(i => i.Import(filePath))
                    .Returns(expectedMetrics);

                using var eventTracker = new CollectionChangedEventTracker(viewModel.InputMetrics);
                viewModel.ImportThroughputMetricsCommand.Execute(null);

                Assert.Equal(expectedMetrics, viewModel.InputMetrics);
                eventTracker.AssertCollectionChangeNotificationsHappenedForAction(NotifyCollectionChangedAction.Add, 2);
            }

            [Fact]
            public void WHEN_importing_some_metrics_AND_file_importer_throws_FileImportException_THEN_bubble_that_exception()
            {
                string filePath = "c:/some/folder/and/someFile.csv";
                fileToReadGetterMock
                    .Setup(g => g.TryGetFileToRead(out filePath))
                    .Returns(true);

                var expectedException = new FileImportException();
                inputMetricsFileImporterMock
                    .Setup(i => i.Import(filePath))
                    .Throws(expectedException);

                void call() => viewModel.ImportThroughputMetricsCommand.Execute(null);

                var actualException = Assert.Throws<FileImportException>(call);
                Assert.Same(expectedException, actualException);
            }
        }

        public class GIVEN_one_input_metric_in_the_collection : IDisposable
        {
            private readonly Mock<IFileLocationGetter> fileLocationGetterMock;
            private readonly Mock<IWorkEstimationsFileExporter> workEstimationsFileExporterMock;
            private readonly MainWindowViewModel viewModelWithOneInputMetric;
            private readonly InputMetric metric;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_one_input_metric_in_the_collection()
            {
                fileLocationGetterMock = new Mock<IFileLocationGetter>();
                workEstimationsFileExporterMock = new Mock<IWorkEstimationsFileExporter>();
                viewModelWithOneInputMetric = new MainWindowViewModel(
                    fileLocationGetterMock.Object,
                    new Mock<IFileToReadGetter>().Object,
                    workEstimationsFileExporterMock.Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

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
            public void WHEN_estimating_completion_time_of_work_items_THEN_estimated_number_of_working_days_till_completion_estimations_updated(
                int numberOfWorkItemsToBeCompleted, ThroughputPerDay throughput, WorkEstimate expectedEstimate)
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = throughput };
                viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = numberOfWorkItemsToBeCompleted;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                Assert.NotNull(viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations);
                var roadmapEstimations = viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations.RoadmapEstimations;
                void assertEstimateHasExpectedNumberOfWorkingDays(WorkEstimate estimate) => Assert.Equal(expectedEstimate.EstimatedNumberOfWorkingDaysRequired, estimate.EstimatedNumberOfWorkingDaysRequired);
                Assert.Collection(roadmapEstimations,
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
                Assert.Collection(roadmapEstimations,
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
            public void WHEN_estimating_completion_time_of_work_items_multiple_times_THEN_the_number_of_working_days_till_completion_estimations_are_updated()
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = 12;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                propertyChangedEventTracker.AssertPropertyChangeNotificationHappenedGivenNumberOfTimesForName(4, NumberOfWorkingDaysTillCompletionEstimationsPropertyName);
                Assert.NotNull(viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations);
                var roadmapEstimations = viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations.RoadmapEstimations;
                Assert.Equal(viewModelWithOneInputMetric.NumberOfMonteCarloSimulations, roadmapEstimations.Count);
            }

            [Fact]
            public void WHEN_estimating_completion_time_of_work_items_THEN_export_work_estimates_command_becomes_enabled()
            {
                AssertThatCommandStateIsAsExpectedWhenActionIsPerformed(
                    viewModelWithOneInputMetric.ExportWorkEstimatesCommand,
                    () =>
                    {
                        viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                        viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = 12;

                        viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);
                    },
                    true,
                    1);
            }

            [Theory]
            [InlineData(ConfigurationMode.Simple)]
            [InlineData(ConfigurationMode.Advanced)]
            public void AND_completion_time_of_work_items_estimated_AND_file_path_returned_WHEN_exporting_work_estimates_THEN_export_to_file_was_performed(ConfigurationMode configurationMode)
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = 12;
                if(configurationMode == ConfigurationMode.Advanced)
                {
                    viewModelWithOneInputMetric.RoadmapConfigurator.SwitchToAdvancedConfigurationMode();
                }

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                string expectedFilePath = "c:/some/folder/and/someFile.csv";
                fileLocationGetterMock
                    .Setup(g => g.TryGetFileLocation(out expectedFilePath))
                    .Returns(true);

                viewModelWithOneInputMetric.ExportWorkEstimatesCommand.Execute(null);

                workEstimationsFileExporterMock.Verify(e => e.Export(expectedFilePath, viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations, configurationMode));
            }

            [Fact]
            public void AND_completion_time_of_work_items_estimated_AND_no_file_path_returned_WHEN_exporting_work_estimates_THEN_nothing_happens()
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = 12;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                string expectedFilePath = null;
                fileLocationGetterMock
                    .Setup(g => g.TryGetFileLocation(out expectedFilePath))
                    .Returns(false);

                viewModelWithOneInputMetric.ExportWorkEstimatesCommand.Execute(null);

                workEstimationsFileExporterMock.Verify(
                    e => e.Export(It.IsAny<string>(), It.IsAny<TimeTillCompletionEstimationsCollection>(), It.IsAny<ConfigurationMode>()),
                    Times.Never
                );
            }

            [Fact]
            public void AND_completion_time_of_work_items_estimated_AND_file_path_returned_AND_file_exporter_throws_FileExportException_WHEN_exporting_work_estimates_THEN_FileExportException_bubbles()
            {
                viewModelWithOneInputMetric.InputMetrics[0] = new InputMetric { Throughput = new ThroughputPerDay(3) };
                viewModelWithOneInputMetric.RoadmapConfigurator.NumberOfWorkItemsToBeCompleted = 12;

                viewModelWithOneInputMetric.EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand.Execute(null);

                string expectedFilePath = "c:/some/folder/and/someFile.csv";
                fileLocationGetterMock
                    .Setup(g => g.TryGetFileLocation(out expectedFilePath))
                    .Returns(true);

                var expectedException = new FileExportException();
                workEstimationsFileExporterMock
                    .Setup(e => e.Export(expectedFilePath, viewModelWithOneInputMetric.NumberOfWorkingDaysTillCompletionEstimations, ConfigurationMode.Simple))
                    .Throws(expectedException);

                void call() => viewModelWithOneInputMetric.ExportWorkEstimatesCommand.Execute(null);

                var actualException = Assert.Throws<FileExportException>(call);
                Assert.Same(expectedException, actualException);
            }
        }

        public class GIVEN_multiple_input_metrics_in_the_collection : IDisposable
        {
            private readonly Mock<IFileToReadGetter> fileToReadGetterMock = new Mock<IFileToReadGetter>();
            private readonly MainWindowViewModel viewModelWithMultipleInputMetrics;
            private readonly PropertyChangedEventTracker propertyChangedEventTracker;

            public GIVEN_multiple_input_metrics_in_the_collection()
            {
                viewModelWithMultipleInputMetrics = new MainWindowViewModel(
                    new Mock<IFileLocationGetter>().Object,
                    fileToReadGetterMock.Object,
                    new Mock<IWorkEstimationsFileExporter>().Object,
                    new Mock<IInputMetricsFileImporter>().Object,
                    new Mock<IAskUserForConfirmationToProceed>().Object);

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

            [Fact]
            public void WHEN_importing_some_metrics_and_file_selection_was_cancelled_THEN_input_metrics_unaffected()
            {
                string filePath = string.Empty;
                fileToReadGetterMock
                    .Setup(g => g.TryGetFileToRead(out filePath))
                    .Returns(false);

                var expectedMetrics = new[]
                {
                    ThroughputToInputMetric(new ThroughputPerDay(1.1)),
                    ThroughputToInputMetric(new ThroughputPerDay(2.2)),
                };
                AddInputMetricsToViewModel(expectedMetrics);

                using var eventTracker = new CollectionChangedEventTracker(viewModelWithMultipleInputMetrics.InputMetrics);
                viewModelWithMultipleInputMetrics.ImportThroughputMetricsCommand.Execute(null);

                Assert.Equal(expectedMetrics, viewModelWithMultipleInputMetrics.InputMetrics);
                eventTracker.AssertNoCollectionChangeNotificationsHappened();
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
