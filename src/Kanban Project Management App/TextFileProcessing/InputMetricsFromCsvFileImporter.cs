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
using System.Collections.Generic;
using System.IO;

namespace KanbanProjectManagementApp.TextFileProcessing
{
    internal class InputMetricsFromCsvFileImporter : IInputMetricsFileImporter
	{
        public string ImportFileExtension => ".csv";
        public string FileTypeDescription => "Comma separated values file";

        public IReadOnlyCollection<InputMetric> Import(string filePath)
		{
			try
			{
				using var reader = new StreamReader(filePath);
				return InputMetricsCsvReader.Read(reader);
			}
			catch(Exception ex)
			{
				throw new FileImportException(
					$"Failed to import input metrics from file '{filePath}', due to an unexpected error.",
					ex);
			}
		}
	}
}
