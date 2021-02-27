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
using KanbanProjectManagementApp.ViewModels;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using KanbanProjectManagementApp.Application.DataImportExport;

namespace KanbanProjectManagementApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly CultureInfo applicationCultureInfo = new CultureInfo("en-US", false);

        public MainWindow(
            IInputMetricsFileImporter inputMetricsFileImporter,
            IWorkEstimationsFileExporter workEstimationsFileExporter)
        {
            InitializeComponent();

            CultureInfo.CurrentCulture = applicationCultureInfo;
            CultureInfo.CurrentUICulture = applicationCultureInfo;

            MainGrid.DataContext = new MainWindowViewModel(
                new SaveFileDialogDrivenFileLocationGetter(this),
                new OpenFileDialogDrivenFileToReadGetter(this),
                workEstimationsFileExporter,
                inputMetricsFileImporter,
                new AskUserForConfirmationToProceedUsingMessageBox(this)
            );
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) =>
            Process.Start(new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true,
            });

        private class SaveFileDialogDrivenFileLocationGetter : IExportFileLocator
        {
            private readonly Window owner;

            public SaveFileDialogDrivenFileLocationGetter(Window owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public bool TryGetFileLocation(IFileExporter exporter, out string filePath)
            {
                if (exporter is null) throw new ArgumentNullException(nameof(exporter));

                var saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    FileName = exporter.DefaultFileName,
                    DefaultExt = exporter.ExportFileExtension,
                    Filter = $"{exporter.FileTypeDescription} ({exporter.ExportFileExtension})|*{exporter.ExportFileExtension}"
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

        private class OpenFileDialogDrivenFileToReadGetter : IImportFileLocator
        {
            private readonly Window owner;

            public OpenFileDialogDrivenFileToReadGetter(Window owner)
            {
                this.owner = owner;
            }

            public bool TryGetFileToRead(IFileImporter importer, out string filePath)
            {
                if (importer is null) throw new ArgumentNullException(nameof(importer));

                var openFileDialog = new OpenFileDialog
                {
                    AddExtension = true,
                    DefaultExt = importer.ImportFileExtension,
                    Filter = $"{importer.FileTypeDescription} ({importer.ImportFileExtension})|*{importer.ImportFileExtension}"
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

        private class AskUserForConfirmationToProceedUsingMessageBox : IAskUserForConfirmationToProceed
        {
            private readonly Window owner;

            public AskUserForConfirmationToProceedUsingMessageBox(Window owner)
            {
                this.owner = owner;
            }

            public bool ConfirmToProceed(string questionToUser)
            {
                var result = MessageBox.Show(owner, questionToUser, "Are you sure?", MessageBoxButton.OKCancel);
                return result switch
                {
                    MessageBoxResult.OK => true,
                    _ => false,
                };
            }
        }
    }
}
