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
using System.Globalization;
using System.Windows.Data;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.Views.ValueConverters
{
    public class ThroughputPerDayValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ThroughputPerDay t)
            {
                return t.ToString(null, culture);
            }

            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            double doubleValue = ParseDoubleFromString(value.ToString(), culture);
            return new ThroughputPerDay(doubleValue);
        }

        private static double ParseDoubleFromString(string stringValue, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(stringValue) ||
                !double.TryParse(stringValue, NumberStyles.Any, culture, out var doubleValue))
            {
                throw new FormatException("Value should represent a number.");
            }

            return doubleValue;
        }
    }
}
