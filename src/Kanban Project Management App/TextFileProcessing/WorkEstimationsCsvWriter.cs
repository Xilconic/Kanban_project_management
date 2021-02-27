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
using System.Data;
using System.Globalization;
using System.IO;
using KanbanProjectManagementApp.Application.DataFormatting;
using KanbanProjectManagementApp.Application.RoadmapConfigurations;
using KanbanProjectManagementApp.Application.TimeTillCompletionForecasting;

namespace KanbanProjectManagementApp.TextFileProcessing
{
    internal class WorkEstimationsCsvWriter
    {
        private const string CsvDelimiter = ";";

        private readonly DataViewFactory factory = new DataViewFactory();
        private readonly TextWriter textWriter;

        /// <remarks>
        /// This class does not dispose of <paramref name="textWriter"/>.
        /// </remarks>
        public WorkEstimationsCsvWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="workEstimates"/> doesn't contain at least 1 simulation.</exception>
        public void Write(TimeTillCompletionEstimationsCollection workEstimates, ConfigurationMode configurationMode)
        {
            if (workEstimates.Count == 0)
            {
                throw new ArgumentException("Work estimations should have at least 1 simulation.", nameof(workEstimates));
            }

            DataView dataView = factory.FromTimeTillCompletionEstimations(
                workEstimates,
                configurationMode,
                CultureInfo.InvariantCulture);

            WriteHeader(dataView.Table.Columns);
            WriteRows(dataView);
        }

        private void WriteHeader(DataColumnCollection columns)
        {
            for(int i = 0; i < columns.Count; i++)
            {
                textWriter.Write(columns[i].ColumnName);
                if (i < columns.Count-1)
                {
                    WriteCsvDelimiter();
                }
            }
            textWriter.WriteLine();
        }

        private void WriteRows(DataView dataView)
        {
            foreach (DataRowView row in dataView)
            {
                for (int i = 0; i < row.Row.ItemArray.Length; i++)
                {
                    textWriter.Write(row.Row.ItemArray[i]);
                    if (i < row.Row.ItemArray.Length - 1)
                    {
                        WriteCsvDelimiter();
                    }
                }
                textWriter.WriteLine();
            }
        }

        private void WriteCsvDelimiter() => textWriter.Write(CsvDelimiter);
    }
}
