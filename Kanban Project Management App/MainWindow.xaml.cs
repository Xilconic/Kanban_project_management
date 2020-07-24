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
using System.Windows;

namespace Kanban_Project_Management_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<InputMetric> elements = new List<InputMetric>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = elements;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(elements.Count == 0)
            {
                calculatedMeanTextBox.Text = "-";
            }
            else
            {
                TimeSpan sum = TimeSpan.Zero;
                foreach (var cycleTime in elements.Select(e => e.CycleTime))
                {
                    sum += cycleTime;
                }
                var mean = sum / elements.Count;
                calculatedMeanTextBox.Text = mean.ToString();
            }
        }
    }
}
