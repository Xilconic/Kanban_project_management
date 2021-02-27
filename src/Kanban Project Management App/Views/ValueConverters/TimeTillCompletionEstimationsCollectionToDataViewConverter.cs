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
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using KanbanProjectManagementApp.Application.TimeTillCompletionForecasting;
using KanbanProjectManagementApp.Application.DataFormatting;

namespace KanbanProjectManagementApp.Views.ValueConverters
{
    public class TimeTillCompletionEstimationsCollectionToDataViewConverter : IMultiValueConverter
    {
        private readonly DataViewFactory factory = new DataViewFactory();
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(values.Length == 2, "Precondition violated: Must pass exactly 1 value into this converter.");

            if (values[0] is TimeTillCompletionEstimationsCollection estimations &&
                values[1] is ConfigurationMode mode)
            {
                return factory.FromTimeTillCompletionEstimations(estimations, mode, culture);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
