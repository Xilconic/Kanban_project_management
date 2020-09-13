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
using KanbanProjectManagementApp.Views.InterfaceImplementations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace KanbanProjectManagementApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CultureInfo applicationCultureInfo = new CultureInfo("en-US", false);

        public MainWindow()
        {
            InitializeComponent();

            CultureInfo.CurrentCulture = applicationCultureInfo;
            CultureInfo.CurrentUICulture = applicationCultureInfo;

            MainGrid.DataContext = new MainWindowViewModel(
                new SaveFileDialogDrivenFileLocationGetter(this),
                new OpenFileDialogDrivenFileToReadGetter(this),
                new WorkEstimationsToCsvFileExporter(),
                new InputMetricsFromCsvFileImporter(),
                new IAskUserForConfirmationToProceedStub()
            );
        }

        private class SaveFileDialogDrivenFileLocationGetter : IFileLocationGetter
        {
            private readonly Window owner;

            public SaveFileDialogDrivenFileLocationGetter(Window owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public bool TryGetFileLocation(out string filePath)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    FileName = "work_estimations",
                    DefaultExt = ".csv",
                    Filter = "Comma separated values file (.csv)|*.csv"
                };
                var dialogResult = saveFileDialog.ShowDialog(owner);
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    filePath = saveFileDialog.FileName;
                    return true;
                }

                filePath = string.Empty;
                return false;
            }
        }

        private class OpenFileDialogDrivenFileToReadGetter : IFileToReadGetter
        {
            private readonly Window owner;

            public OpenFileDialogDrivenFileToReadGetter(Window owner)
            {
                this.owner = owner;
            }

            public bool TryGetFileToRead(out string filePath)
            {
                var openFileDialog = new OpenFileDialog
                {
                    AddExtension = true,
                    DefaultExt = ".csv",
                    Filter = "Comma separated values file (.csv)|*.csv"
                };
                var dialogResult = openFileDialog.ShowDialog(owner);
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    filePath = openFileDialog.FileName;
                    return true;
                }

                filePath = string.Empty;
                return false;
            }
        }

        private class WorkEstimationsToCsvFileExporter : IWorkEstimationsFileExporter
        {
            public void Export(string filePath, IReadOnlyCollection<WorkEstimate> workEstimates)
            {
                try
                {
                    using var writer = new StreamWriter(filePath);
                    var csvWriter = new WorkEstimationsCsvWriter(writer);
                    csvWriter.Write(workEstimates);
                }
                catch (Exception ex)
                {
                    throw new FileExportException(
                        $"Failed to export work estimation to file '{filePath}', due to an unexpected error.",
                        ex);
                }
            }
        }

        private class InputMetricsFromCsvFileImporter : IInputMetricsFileImporter
        {
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

        private class IAskUserForConfirmationToProceedStub : IAskUserForConfirmationToProceed
        {
            public bool ConfirmToProceed(string questionToUser)
            {
                return true;
            }
        }
    }
}
