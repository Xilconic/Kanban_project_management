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
using KanbanProjectManagementApp.ViewModels;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace KanbanProjectManagementApp.Views.ValueConverters
{
    public class TimeTillCompletionEstimationsCollectionToDataViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(values.Length == 2, "Precondition violated: Must pass exactly 1 value into this converter.");

            if (values[0] is TimeTillCompletionEstimationsCollection estimations &&
                values[1] is ConfigurationMode mode)
            {
                Debug.Assert(estimations.RoadmapEstimations.Count > 0, $"Invariant failed: Should guarantee at least 1 work estimation.");

                string identifier = estimations.RoadmapEstimations.First().Identifier;

                var dataTable = new DataTable();

                AddReadOnlyColumn($"Number of days till completion of '{identifier}' in simulation", dataTable);
                AddReadOnlyColumn($"Is '{identifier}' estimation indeterminate", dataTable);
                if(mode == ConfigurationMode.Advanced)
                {
                    for(int i = 0; i < estimations.NumberOfProjectsInRoadmap; i++)
                    {
                        var projectEstimations = estimations[i];
                        Debug.Assert(projectEstimations.Count > 0, $"Invariant failed: Should guarantee at least 1 work estimation for project.");

                        identifier = projectEstimations.First().Identifier;
                        AddReadOnlyColumn($"Number of days till completion of '{identifier}' in simulation", dataTable);
                        AddReadOnlyColumn($"Is '{identifier}' estimation indeterminate", dataTable);
                    }
                }

                for(int i = 0; i < estimations.NumberOfSimulations; i++)
                {
                    object[] rowData = GetRowData(estimations, i, mode, culture);
                    dataTable.Rows.Add(rowData);
                }

                return dataTable.DefaultView;
            }

            return DependencyProperty.UnsetValue;
        }

        private object[] GetRowData(
            TimeTillCompletionEstimationsCollection estimations,
            int simulationIndex,
            ConfigurationMode mode,
            CultureInfo culture)
        {
            WorkEstimate roadmapEstimate = estimations.GetRoadmapEstimationForSimulation(simulationIndex);
            var rowData = new object[]
            {
                FormatEstiatedNumberOfWorkingDays(roadmapEstimate, culture),
                roadmapEstimate.IsIndeterminate
            };
            if(mode == ConfigurationMode.Advanced)
            {
                var additionalRowData = new object[estimations.NumberOfProjectsInRoadmap*2];
                for(int projectIndex = 0; projectIndex < estimations.NumberOfProjectsInRoadmap; projectIndex++)
                {
                    WorkEstimate projectEstimate = estimations.GetProjectEstimationForSimulation(projectIndex, simulationIndex);
                    additionalRowData[projectIndex * 2] = FormatEstiatedNumberOfWorkingDays(projectEstimate, culture);
                    additionalRowData[projectIndex * 2 + 1] = projectEstimate.IsIndeterminate;
                }
                rowData = rowData.Concat(additionalRowData).ToArray();
            }

            return rowData;
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

        private string FormatEstiatedNumberOfWorkingDays(WorkEstimate estimate, CultureInfo culture) =>
            estimate.EstimatedNumberOfWorkingDaysRequired.ToString(culture);
    }
}
