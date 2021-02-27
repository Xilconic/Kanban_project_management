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
using System.Collections.Generic;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.Application.DataImportExport
{
    public class ImportThroughputMetricsFromFileUseCase
    {
        private readonly IImportFileLocator locator;
        private readonly IInputMetricsFileImporter importer;

        public ImportThroughputMetricsFromFileUseCase(
            IImportFileLocator locator,
            IInputMetricsFileImporter importer)
        {
            this.locator = locator ?? throw new ArgumentNullException(nameof(locator));
            this.importer = importer ?? throw new ArgumentNullException(nameof(importer));
        }

        public void AppendThroughputMetricsFromImportFile(ICollection<InputMetric> inputMetrics)
        {
            if (locator.TryGetFileToRead(importer, out string filePath))
            {
                foreach (InputMetric metric in importer.Import(filePath))
                {
                    inputMetrics.Add(metric);
                }
            }
        }
    }
}
