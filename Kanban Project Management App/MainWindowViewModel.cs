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
