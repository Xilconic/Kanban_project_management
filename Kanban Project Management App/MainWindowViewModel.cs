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
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace KanbanProjectManagementApp
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ThroughputPerDay? estimatedMeanOfThroughputNew;
        private double? estimatedCorrectedSampleStandardDeviationOfThroughputNew;

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

        public ICommand UpdateCycleTimeStatisticsCommand { get; }

        public MainWindowViewModel()
        {
            UpdateCycleTimeStatisticsCommand = new CalculateThroughputStatisticsCommand(this);
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
                foreach(ThroughputPerDay throughput in InputMetrics.Select(i => i.Throughput))
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
    }
}
