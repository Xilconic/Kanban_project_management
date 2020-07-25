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

        public TimeSpan? MeanOfCycleTime
        {
            get => meanOfCycleTime;
            private set
            {
                if(value != meanOfCycleTime)
                {
                    meanOfCycleTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeanOfCycleTime)));
                }
            }
        }

        public ICommand UpdateMeanOfCycleTimeCommand { get; }

        public MainWindowViewModel()
        {
            UpdateMeanOfCycleTimeCommand = new CalculateMeanOfCycleTimeCommand(this);
        }

        private void UpdateMeanOfCycleTime()
        {
            if (InputMetrics.Count == 0)
            {
                MeanOfCycleTime = null;
            }
            else
            {
                TimeSpan sum = TimeSpan.Zero;
                foreach (var cycleTime in InputMetrics.Select(e => e.CycleTime))
                {
                    sum += cycleTime;
                }
                var mean = sum / InputMetrics.Count;
                MeanOfCycleTime = mean;
            }
        }

        private class CalculateMeanOfCycleTimeCommand : ICommand
        {
            private readonly MainWindowViewModel viewModel;

            public CalculateMeanOfCycleTimeCommand(MainWindowViewModel viewModel)
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
            }
        }
    }
}
