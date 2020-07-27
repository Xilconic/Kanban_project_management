using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace KanbanProjectManagementApp
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private TimeSpan? meanOfCycleTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InputMetric> InputMetrics { get; } = new ObservableCollection<InputMetric>();

        public TimeSpan? EstimatedMeanOfCycleTime
        {
            get => meanOfCycleTime;
            private set
            {
                if(value != meanOfCycleTime)
                {
                    meanOfCycleTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedMeanOfCycleTime)));
                }
            }
        }

        public TimeSpan? EstimatedCorrectedSampleStandardDeviationOfCycleTime
        {
            get;
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
                TimeSpan sum = TimeSpan.Zero;
                foreach (var cycleTime in InputMetrics.Select(e => e.CycleTime))
                {
                    sum += cycleTime;
                }
                var mean = sum / InputMetrics.Count;
                EstimatedMeanOfCycleTime = mean;
            }
        }

        private void UpdateCorrectedSampleStandardDeviationOfCycleTime()
        {
            // TODO;
        }

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
                viewModel.UpdateCorrectedSampleStandardDeviationOfCycleTime();
            }
        }
    }
}
