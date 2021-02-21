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
using KanbanProjectManagementApp.Application;
using System;

namespace KanbanProjectManagementApp.ViewModels
{
    public interface IWorkEstimationsFileExporter : IFileExporter
    {
        /// <exception cref="FileExportException"/>
        void Export(string filePath, TimeTillCompletionEstimationsCollection workEstimates, ConfigurationMode configurationMode);
    }

    public class FileExportException : Exception
    {
        public FileExportException()
        {
        }

        public FileExportException(string message)
            : base(message)
        {
        }

        public FileExportException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
