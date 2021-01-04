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
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using KanbanProjectManagementApp.Domain;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KanbanProjectManagementApp.Views.InterfaceImplementations
{
    internal class WorkEstimationsCsvWriter
    {
        private readonly TextWriter textWriter;

        public WorkEstimationsCsvWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        public void Write(TimeTillCompletionEstimationsCollection workEstimates)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                SanitizeForInjection = true,
            };
            using var csvWriter = new CsvWriter(textWriter, configuration, true);
            csvWriter.WriteRecords(workEstimates.RoadmapEstimations.Select(WorkEstimateRow.FromDomain));
        }

        private class WorkEstimateRow
        {
            [Index(0)]
            [Name("Number of days till completion in simulation")]
            public double EstimatedNumberOfWorkingDaysRequired { get; set; }

            [Index(1)]
            [Name("Is indeterminate")]
            public bool IsIndeterminate { get; set; }

            public static WorkEstimateRow FromDomain(WorkEstimate we) =>
                new WorkEstimateRow
                {
                    EstimatedNumberOfWorkingDaysRequired = we.EstimatedNumberOfWorkingDaysRequired,
                    IsIndeterminate = we.IsIndeterminate,
                };
        }
    }
}
