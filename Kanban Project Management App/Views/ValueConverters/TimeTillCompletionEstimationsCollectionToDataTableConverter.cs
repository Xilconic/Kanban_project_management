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
using KanbanProjectManagementApp.Domain;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace KanbanProjectManagementApp.Views.ValueConverters
{
    public class TimeTillCompletionEstimationsCollectionToDataTableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(values.Length == 1, "Precondition violated: Must pass exactly 1 value into this converter.");

            if (values[0] is TimeTillCompletionEstimationsCollection estimations)
            {
                Debug.Assert(estimations.RoadmapEstimations.Count > 0, $"Invariant failed: Should guarantee at least 1 work estimation.");

                string identifier = estimations.RoadmapEstimations.First().Identifier;

                var dataTable = new DataTable();

                AddReadOnlyColumn($"Number of days till completion of '{identifier}' in simulation", dataTable);
                AddReadOnlyColumn($"Is '{identifier}' estimation indeterminate", dataTable);

                foreach (var roadmapCompletionEstimate in estimations.RoadmapEstimations)
                {
                    dataTable.Rows.Add(roadmapCompletionEstimate.EstimatedNumberOfWorkingDaysRequired.ToString(culture), roadmapCompletionEstimate.IsIndeterminate);
                }
                return dataTable.DefaultView;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void AddReadOnlyColumn(string header, DataTable dataTable)
        {
            var c = dataTable.Columns.Add(header);
            c.ReadOnly = true;
        }
    }
}
