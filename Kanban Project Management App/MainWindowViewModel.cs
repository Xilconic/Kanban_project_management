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
        private TimeSpan? estimatedMeanOfThroughput;
        private TimeSpan? estimatedCorrectedSampleStandardDeviationOfThroughput;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InputMetric> InputMetrics { get; } = new ObservableCollection<InputMetric>();

        public TimeSpan? EstimatedMeanOfThroughput
        {
            get => estimatedMeanOfThroughput;
            private set
            {
                if (value != estimatedMeanOfThroughput)
                {
                    estimatedMeanOfThroughput = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedMeanOfThroughput)));
                }
            }
        }

        public TimeSpan? EstimatedCorrectedSampleStandardDeviationOfThroughput
        {
            get => estimatedCorrectedSampleStandardDeviationOfThroughput;
            private set
            {
                if (value != estimatedCorrectedSampleStandardDeviationOfThroughput)
                {
                    estimatedCorrectedSampleStandardDeviationOfThroughput = value;
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
                TimeSpan sum = SumOfTimeSpans(InputMetrics.Select(e => e.Throughput));
                var mean = sum / InputMetrics.Count;
                EstimatedMeanOfThroughput = mean;
            }
        }

        private void UpdateCorrectedSampleStandardDeviationOfThroughput(TimeSpan? estimatedMeanOfCycleTime)
        {
            if (InputMetrics.Count == 0)
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = null;
            }
            else if(InputMetrics.Count == 1)
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = TimeSpan.Zero;
            }
            else
            {
                EstimatedCorrectedSampleStandardDeviationOfThroughput = CalculateCorrectedSampleStandardDeviationOfThroughputAtHourScale(estimatedMeanOfCycleTime);
            }
        }

        private TimeSpan CalculateCorrectedSampleStandardDeviationOfThroughputAtHourScale(TimeSpan? estimatedMeanOfCycleTime)
        {
            // Doing calculations are hour resolution gave adequate accuracy with regards to limited precision of double calculations involved.
            var squaredDifferencesFromMean = InputMetrics.Select(e => GetSquaredTimeSpanAtHoursResolution(e.Throughput - estimatedMeanOfCycleTime.Value));
            var sumSquaredDifferencesFromMean = SumOfTimeSpans(squaredDifferencesFromMean);
            var correctionFactor = 1.0 / (InputMetrics.Count - 1);
            return GetSquareRootOfTimeSpanAtHoursResolution(sumSquaredDifferencesFromMean * correctionFactor);
        }

        private TimeSpan SumOfTimeSpans(IEnumerable<TimeSpan> timeSpans)
        {
            TimeSpan sum = TimeSpan.Zero;
            foreach (var cycleTime in timeSpans)
            {
                sum += cycleTime;
            }

            return sum;
        }

        private static TimeSpan GetSquaredTimeSpanAtHoursResolution(TimeSpan ts) => TimeSpan.FromHours(Math.Pow(ts.TotalHours, 2));

        private static TimeSpan GetSquareRootOfTimeSpanAtHoursResolution(TimeSpan ts) => TimeSpan.FromHours(Math.Sqrt(ts.TotalHours));

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
