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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ThroughputPerDay? estimatedMeanOfThroughputNew;
        private double? estimatedCorrectedSampleStandardDeviationOfThroughputNew;
        private int numberOfMonteCarloSimulations = 10;
        private int maximumNumberOfIterations = 25;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InputMetric> InputMetrics { get; } = new ObservableCollection<InputMetric>();

        public ThroughputPerDay? EstimatedMeanOfThroughput
        {
            get => estimatedMeanOfThroughputNew;
            private set
            {
                if (!Equals(value, estimatedMeanOfThroughputNew))
                {
                    estimatedMeanOfThroughputNew = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedMeanOfThroughput)));
                }
            }
        }

        public double? EstimatedCorrectedSampleStandardDeviationOfThroughput
        {
            get => estimatedCorrectedSampleStandardDeviationOfThroughputNew;
            private set
            {
                if (value != estimatedCorrectedSampleStandardDeviationOfThroughputNew)
                {
                    estimatedCorrectedSampleStandardDeviationOfThroughputNew = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedCorrectedSampleStandardDeviationOfThroughput)));
                }
            }
        }

        public RoadmapConfigurationViewModel RoadmapConfigurator { get; }

        public int NumberOfMonteCarloSimulations
        {
            get => numberOfMonteCarloSimulations;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Number of simulation should be at least 1.");
                }

                numberOfMonteCarloSimulations = value;
            }
        }

        public int MaximumNumberOfIterations
        {
            get => maximumNumberOfIterations;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum number of iterations should be at least 1.");
                }

                maximumNumberOfIterations = value;
            }
        }

        public ObservableCollection<WorkEstimate> NumberOfWorkingDaysTillCompletionEstimations { get; } = new ObservableCollection<WorkEstimate>();

        /// <summary>
        /// This property controls the <see cref="DataGrid"/> columns that is bound to <see cref="NumberOfWorkingDaysTillCompletionEstimations"/>.
        /// This has been done to allow for dynamic creation of columns in said <see cref="DataGrid"/>.
        /// </summary>
        /// TODO: This property should probably be defined in a View class instead of this ViewModel, as ViewModels should typically not have dependencies
        /// on View-style objects, like DataGridColumn.
        public ObservableCollection<DataGridColumn> WorkEstimationDataGridColumns { get; } = new ObservableCollection<DataGridColumn>();

        public ICommand ImportThroughputMetricsCommand { get; }

        public ICommand UpdateCycleTimeStatisticsCommand { get; }

        public ICommand EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand { get; }

        public ICommand ExportWorkEstimatesCommand { get; }

        public MainWindowViewModel(
            IFileLocationGetter fileLocationToSaveGetter,
            IFileToReadGetter fileToReadGetter,
            IWorkEstimationsFileExporter workEstimationsFileExporter,
            IInputMetricsFileImporter inputMetricsFileImporter,
            IAskUserForConfirmationToProceed confirmationAsker)
        {
            ImportThroughputMetricsCommand = new ImportThroughputMetricsFromFileCommand(InputMetrics, fileToReadGetter, inputMetricsFileImporter);
            UpdateCycleTimeStatisticsCommand = new CalculateThroughputStatisticsCommand(this);
            EstimateNumberOfWorkDaysTillWorkItemsCompletedCommand = new PerformMonteCarloEstimationOfNumberOfWorkDaysTillWorkItemsCompletedCommand(this);
            ExportWorkEstimatesCommand = new ExportWorkEstimatesToFileCommand(fileLocationToSaveGetter, workEstimationsFileExporter, NumberOfWorkingDaysTillCompletionEstimations);

            RoadmapConfigurator = new RoadmapConfigurationViewModel(confirmationAsker);
        }

        private void UpdateMeanOfThroughput()
        {
            if (InputMetrics.Count == 0)
            {
                EstimatedMeanOfThroughput = null;
            }
            else
            {
                var sum = new ThroughputPerDay(0);
                foreach (ThroughputPerDay throughput in InputMetrics.Select(i => i.Throughput))
                {
                    sum += throughput;
                }
                var mean = sum / InputMetrics.Count;
                EstimatedMeanOfThroughput = mean;
            }
        }

        private void UpdateCorrectedSampleStandardDeviationOfThroughput(ThroughputPerDay? estimatedMeanOfThroughput)
        {
            if (InputMetrics.Count == 0)
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = null;
            }
            else if (InputMetrics.Count == 1)
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = 0.0;
            }
            else
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = CalculateCorrectedSampleStandardDeviationOfThroughput(estimatedMeanOfThroughput);
            }
        }

        private double CalculateCorrectedSampleStandardDeviationOfThroughput(ThroughputPerDay? estimatedMeanOfThroughput)
        {
            double meanNumberOfDaysCompleted = estimatedMeanOfThroughput.Value.GetNumberOfWorkItemsPerDay();
            var sumSquaredDifferencesFromMean = InputMetrics
                .Select(e => e.Throughput.GetNumberOfWorkItemsPerDay() - meanNumberOfDaysCompleted)
                .Select(difference => Math.Pow(difference, 2))
                .Sum();
            var correctionFactor = 1.0 / (InputMetrics.Count - 1);
            return Math.Sqrt(sumSquaredDifferencesFromMean * correctionFactor);
        }

        private class ImportThroughputMetricsFromFileCommand : ICommand
        {
            private readonly IFileToReadGetter fileToReadGetter;
            private readonly IInputMetricsFileImporter inputMetricsFileImporter;
            private readonly ObservableCollection<InputMetric> inputMetrics;

            public ImportThroughputMetricsFromFileCommand(
                ObservableCollection<InputMetric> inputMetrics,
                IFileToReadGetter fileToReadGetter,
                IInputMetricsFileImporter inputMetricsFileImporter)
            {
                this.inputMetrics = inputMetrics ?? throw new ArgumentNullException(nameof(inputMetrics));
                this.fileToReadGetter = fileToReadGetter ?? throw new ArgumentNullException(nameof(fileToReadGetter));
                this.inputMetricsFileImporter = inputMetricsFileImporter ?? throw new ArgumentNullException(nameof(inputMetricsFileImporter));
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if(fileToReadGetter.TryGetFileToRead(out string filePath))
                {
                    foreach (InputMetric metric in inputMetricsFileImporter.Import(filePath))
                    {
                        inputMetrics.Add(metric);
                    }
                }
            }
        }

        private class CalculateThroughputStatisticsCommand : ICommand
        {
            private readonly MainWindowViewModel viewModel;

            public CalculateThroughputStatisticsCommand(MainWindowViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                viewModel.UpdateMeanOfThroughput();
                viewModel.UpdateCorrectedSampleStandardDeviationOfThroughput(viewModel.EstimatedMeanOfThroughput);
            }
        }

        private class PerformMonteCarloEstimationOfNumberOfWorkDaysTillWorkItemsCompletedCommand : ICommand
        {
            private readonly MainWindowViewModel viewModel;
            private bool canExecute = false;

            public PerformMonteCarloEstimationOfNumberOfWorkDaysTillWorkItemsCompletedCommand(MainWindowViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.InputMetrics.CollectionChanged += InputMetrics_CollectionChanged;
            }

            private void InputMetrics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var newValue = viewModel.InputMetrics.Count > 0;
                if (canExecute != newValue)
                {
                    canExecute = newValue;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return canExecute;
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    throw new InvalidOperationException();
                }

                var estimator = new MonteCarloTimeTillCompletionEstimator(
                    viewModel.NumberOfMonteCarloSimulations,
                    viewModel.MaximumNumberOfIterations,
                    viewModel.InputMetrics);
                TimeTillCompletionEstimationsCollection workEstimations = estimator.Estimate(viewModel.RoadmapConfigurator.Projects);

                UpdateNumberOfWorkingDaysTillCompletionEstimations(workEstimations.RoadmapEstimation);
                UpdateWorkEstimationDataGridColumns(workEstimations.RoadmapEstimation);
            }

            private void UpdateNumberOfWorkingDaysTillCompletionEstimations(IReadOnlyCollection<WorkEstimate> workEstimations)
            {
                viewModel.NumberOfWorkingDaysTillCompletionEstimations.Clear();
                foreach (WorkEstimate estimate in workEstimations)
                {
                    viewModel.NumberOfWorkingDaysTillCompletionEstimations.Add(estimate);
                }
            }

            private void UpdateWorkEstimationDataGridColumns(IReadOnlyCollection<WorkEstimate> workEstimations)
            {
                Debug.Assert(workEstimations.Count > 0, $"Precondition failed: Should guarantee at least 1 work estimation.");

                viewModel.WorkEstimationDataGridColumns.Clear();
                string identifier = workEstimations.First().Identifier;
                viewModel.WorkEstimationDataGridColumns.Add(new DataGridTextColumn
                {
                    Header = $"Number of days till completion of '{identifier}' in simulation",
                    Binding = new Binding(nameof(WorkEstimate.EstimatedNumberOfWorkingDaysRequired))
                });
                viewModel.WorkEstimationDataGridColumns.Add(new DataGridTextColumn
                {
                    Header = $"Is '{identifier}' estimation indeterminate",
                    Binding = new Binding(nameof(WorkEstimate.IsIndeterminate))
                });
            }
        }

        private class ExportWorkEstimatesToFileCommand : ICommand
        {
            private readonly IFileLocationGetter fileLocationGetter;
            private readonly IWorkEstimationsFileExporter workEstimationsFileExporter;
            private readonly ObservableCollection<WorkEstimate> workEstimations;
            private bool canExecute = false;

            public ExportWorkEstimatesToFileCommand(
                IFileLocationGetter fileLocationToSaveGetter,
                IWorkEstimationsFileExporter workEstimationsFileExporter,
                ObservableCollection<WorkEstimate> workEstimations)
            {
                this.fileLocationGetter = fileLocationToSaveGetter ?? throw new ArgumentNullException(nameof(fileLocationToSaveGetter));
                this.workEstimationsFileExporter = workEstimationsFileExporter ?? throw new ArgumentNullException(nameof(workEstimationsFileExporter));
                this.workEstimations = workEstimations ?? throw new ArgumentNullException(nameof(workEstimations));
                canExecute = HasWorkEstimations();
                workEstimations.CollectionChanged += InputMetrics_CollectionChanged;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return canExecute;
            }

            public void Execute(object parameter)
            {
                if(fileLocationGetter.TryGetFileLocation(out string filePath))
                {
                    workEstimationsFileExporter.Export(filePath, workEstimations);
                }
            }

            private bool HasWorkEstimations()
            {
                return workEstimations.Count > 0;
            }

            private void InputMetrics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var newValue = HasWorkEstimations();
                if (canExecute != newValue)
                {
                    canExecute = newValue;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
