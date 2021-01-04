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
using System.Globalization;
using System.IO;
using System.Linq;

namespace KanbanProjectManagementApp.Views.InterfaceImplementations
{
    internal class WorkEstimationsCsvWriter
    {
        private const string csvDelimiter = ";";
        private readonly TextWriter textWriter;

        /// <remarks>
        /// This class does not dispose of <paramref name="textWriter"/>.
        /// </remarks>
        public WorkEstimationsCsvWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="workEstimates"/> doesn't contain at least 1 simulation.</exception>
        public void Write(TimeTillCompletionEstimationsCollection workEstimates)
        {
            if (workEstimates.Count == 0)
            {
                throw new ArgumentException("Work estimations should have at least 1 simulation.", nameof(workEstimates));
            }

            WriteHeader(workEstimates);
            WriteRows(workEstimates);
        }

        private void WriteHeader(TimeTillCompletionEstimationsCollection workEstimates)
        {
            var roadmapIdentifier = workEstimates.RoadmapEstimations.First().Identifier;
            textWriter.Write($"Number of days till completion of '{roadmapIdentifier}' in simulation");
            WriteCsvDelimiter();
            textWriter.Write($"Is '{roadmapIdentifier}' estimation indeterminate");
            textWriter.WriteLine();
        }

        private void WriteRows(TimeTillCompletionEstimationsCollection workEstimates)
        {
            foreach (var estimate in workEstimates.RoadmapEstimations)
            {
                WriteRow(estimate);
            }
        }

        private void WriteRow(WorkEstimate estimate)
        {
            WriteFormattableValue(estimate.EstimatedNumberOfWorkingDaysRequired);
            WriteCsvDelimiter();
            textWriter.Write(estimate.IsIndeterminate);
            textWriter.WriteLine();
        }

        private void WriteCsvDelimiter() => textWriter.Write(csvDelimiter);
        private void WriteFormattableValue(IFormattable formattable) => textWriter.Write(formattable.ToString(null, CultureInfo.InvariantCulture));
    }
}
