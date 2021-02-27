//  Copyright (c) 2020 S.L. des Bouvrie
// 
//  This file is part of 'Kanban Project Management App'.
// 
//  Kanban Project Management App is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Kanban Project Management App is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with Kanban Project Management App.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using KanbanProjectManagementApp.Application.TimeTillCompletionForecasting;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.Application.DataFormatting
{
    public class DataViewFactory
    {
        public DataView FromTimeTillCompletionEstimations(
            TimeTillCompletionEstimationsCollection estimations,
            ConfigurationMode mode,
            CultureInfo culture)
        {
            if (estimations is null) throw new ArgumentNullException(nameof(estimations));
            
            Debug.Assert(estimations.RoadmapEstimations.Count > 0,
                $"Invariant failed: Should guarantee at least 1 work estimation.");

            string identifier = estimations.RoadmapEstimations.First().Identifier;

            var dataTable = new DataTable();

            AddReadOnlyColumn($"Number of days till completion of '{identifier}' in simulation", dataTable);
            AddReadOnlyColumn($"Is '{identifier}' estimation indeterminate", dataTable);
            if (mode == ConfigurationMode.Advanced)
            {
                for (int i = 0; i < estimations.NumberOfProjectsInRoadmap; i++)
                {
                    var projectEstimations = estimations[i];
                    Debug.Assert(projectEstimations.Count > 0,
                        $"Invariant failed: Should guarantee at least 1 work estimation for project.");

                    identifier = projectEstimations.First().Identifier;
                    AddReadOnlyColumn($"Number of days till completion of '{identifier}' in simulation", dataTable);
                    AddReadOnlyColumn($"Is '{identifier}' estimation indeterminate", dataTable);
                }
            }

            for (int i = 0; i < estimations.NumberOfSimulations; i++)
            {
                object[] rowData = GetRowData(estimations, i, mode, culture);
                dataTable.Rows.Add(rowData);
            }

            return dataTable.DefaultView;
        }
        
        private static void AddReadOnlyColumn(
            string header,
            DataTable dataTable)
        {
            var c = dataTable.Columns.Add(header);
            c.ReadOnly = true;
        }
        
        private static object[] GetRowData(
            TimeTillCompletionEstimationsCollection estimations,
            int simulationIndex,
            ConfigurationMode mode,
            CultureInfo culture)
        {
            WorkEstimate roadmapEstimate = estimations.GetRoadmapEstimationForSimulation(simulationIndex);
            var rowData = new object[]
            {
                FormatEstimatedNumberOfWorkingDays(roadmapEstimate, culture),
                roadmapEstimate.IsIndeterminate
            };
            if(mode == ConfigurationMode.Advanced)
            {
                var additionalRowData = new object[estimations.NumberOfProjectsInRoadmap*2];
                for(int projectIndex = 0; projectIndex < estimations.NumberOfProjectsInRoadmap; projectIndex++)
                {
                    WorkEstimate projectEstimate = estimations.GetProjectEstimationForSimulation(projectIndex, simulationIndex);
                    additionalRowData[projectIndex * 2] = FormatEstimatedNumberOfWorkingDays(projectEstimate, culture);
                    additionalRowData[projectIndex * 2 + 1] = projectEstimate.IsIndeterminate;
                }
                rowData = rowData.Concat(additionalRowData).ToArray();
            }

            return rowData;
        }
        
        private static string FormatEstimatedNumberOfWorkingDays(
            WorkEstimate estimate,
            CultureInfo culture) =>
            estimate.EstimatedNumberOfWorkingDaysRequired.ToString(culture);
    }
}