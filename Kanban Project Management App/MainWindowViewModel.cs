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
        private TimeSpan? estimatedMeanOfCycleTime;
        private TimeSpan? estimatedCorrectedSampleStandardDeviationOfCycleTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InputMetric> InputMetrics { get; } = new ObservableCollection<InputMetric>();

        public TimeSpan? EstimatedMeanOfCycleTime
        {
            get => estimatedMeanOfCycleTime;
            private set
            {
                if (value != estimatedMeanOfCycleTime)
                {
                    estimatedMeanOfCycleTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedMeanOfCycleTime)));
                }
            }
        }

        public TimeSpan? EstimatedCorrectedSampleStandardDeviationOfCycleTime
        {
            get => estimatedCorrectedSampleStandardDeviationOfCycleTime;
            private set
            {
                if (value != estimatedCorrectedSampleStandardDeviationOfCycleTime)
                {
                    estimatedCorrectedSampleStandardDeviationOfCycleTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedCorrectedSampleStandardDeviationOfCycleTime)));
                }
            }
        }

        public ICommand UpdateCycleTimeStatisticsCommand { get; }

        public MainWindowViewModel()
        {
            UpdateCycleTimeStatisticsCommand = new CalculateCycleTimeStatisticsCommand(this);
        }

        private void UpdateMeanOfCycleTime()
        {
            if (InputMetrics.Count == 0)
            {
                EstimatedMeanOfCycleTime = null;
            }
            else
            {
                TimeSpan sum = SumOfTimeSpans(InputMetrics.Select(e => e.CycleTime));
                var mean = sum / InputMetrics.Count;
                EstimatedMeanOfCycleTime = mean;
            }
        }

        private void UpdateCorrectedSampleStandardDeviationOfCycleTime(TimeSpan? estimatedMeanOfCycleTime)
        {
            if (InputMetrics.Count == 0)
            {
                EstimatedCorrectedSampleStandardDeviationOfCycleTime = null;
            }
            else if(InputMetrics.Count == 1)
            {
                EstimatedCorrectedSampleStandardDeviationOfCycleTime = TimeSpan.Zero;
            }
            else
            {
                EstimatedCorrectedSampleStandardDeviationOfCycleTime = CalculateCorrectedSampleStandardDeviationOfCycleTimeAtDayScale(estimatedMeanOfCycleTime);
            }
        }

        private TimeSpan CalculateCorrectedSampleStandardDeviationOfCycleTimeAtDayScale(TimeSpan? estimatedMeanOfCycleTime)
        {
            // Doing calculations are hour resolution gave adequate accuracy with regards to limited precision of double calculations involved.
            var squaredDifferencesFromMean = InputMetrics.Select(e => GetSquaredTimeSpanAtHoursResolution(e.CycleTime - estimatedMeanOfCycleTime.Value));
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

        private class CalculateCycleTimeStatisticsCommand : ICommand
        {
            private readonly MainWindowViewModel viewModel;

            public CalculateCycleTimeStatisticsCommand(MainWindowViewModel viewModel)
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
                viewModel.UpdateMeanOfCycleTime();
                viewModel.UpdateCorrectedSampleStandardDeviationOfCycleTime(viewModel.EstimatedMeanOfCycleTime);
            }
        }
    }
}
