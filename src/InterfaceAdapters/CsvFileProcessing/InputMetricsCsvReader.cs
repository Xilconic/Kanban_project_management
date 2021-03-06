﻿// Copyright (c) 2020 S.L. des Bouvrie
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
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.InterfaceAdapters.CsvFileProcessing
{
    public class InputMetricsCsvReader
    {
        private static readonly CsvConfiguration Configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };

        /// <exception cref="FailedToReadInputMetricsException"/>
        public static IReadOnlyCollection<InputMetric> Read(TextReader textReader)
        {
            if (textReader is null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }

            using var csvReader = new CsvReader(textReader, Configuration, true);
            try
            {
                var records = csvReader.GetRecords<MetricPropertyRecord>();
                return records.Select(r => r.ToDomain()).ToArray();
            }
            catch(HeaderValidationException ex)
            {
                throw new FailedToReadInputMetricsException(
                    "Invalid header. It must contain a column with the name 'NumberOfCompletedWorkItems' and use ';' as delimiter.",
                    ex);
            }
            catch(TypeConverterException ex)
            {
                throw new FailedToReadInputMetricsException(
                    "Failed to parse a value in the 'NumberOfCompletedWorkItems' column. All elements must be a number.",
                    ex);
            }
            catch(Exception ex) when(!(ex is FailedToReadInputMetricsException))
            {
                // Covers any unknown Exceptions that CsvReader could possibly throw
                throw new FailedToReadInputMetricsException(
                    "Failed to read the file due to unexpected reasons.",
                    ex);
            }
        }

        private class MetricPropertyRecord
        {
            public double NumberOfCompletedWorkItems { get; set; }

            /// <exception cref="FailedToReadInputMetricsException">Thrown when conversion was failed.</exception>
            public InputMetric ToDomain()
            {
                try
                {
                    return new InputMetric {Throughput = new ThroughputPerDay(NumberOfCompletedWorkItems)};
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new FailedToReadInputMetricsException(
                        $"Invalid data was provided. Details: {e.Message}",
                        e);
                }
            }
        }
    }
}
