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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KanbanProjectManagementApp.Views.AttachedProperties
{
    internal class DataGridAttachedProperties
    {
        private const string DynamicColumnsPropertyName = "DynamicColumns";
        private static readonly Dictionary<DataGrid, NotifyCollectionChangedEventHandler> dynamicColumnCollectionChangedEventHandlersPerDataGrid =
            new Dictionary<DataGrid, NotifyCollectionChangedEventHandler>();

        public static readonly DependencyProperty DynamicColumnsProperty =
            DependencyProperty.RegisterAttached(
                DynamicColumnsPropertyName,
                typeof(ObservableCollection<DataGridColumn>),
                typeof(DataGridAttachedProperties),
                new UIPropertyMetadata(null, DynamicColumnsPropertyChanged));

        [AttachedPropertyBrowsableForTypeAttribute(typeof(DataGrid))]
        public static ObservableCollection<DataGridColumn> GetDynamicColumns(UIElement target) =>
            (ObservableCollection<DataGridColumn>)target.GetValue(DynamicColumnsProperty);

        [AttachedPropertyBrowsableForTypeAttribute(typeof(DataGrid))]
        public static void SetDynamicColumns(UIElement target, ObservableCollection<DataGridColumn> newValue) =>
            target.SetValue(DynamicColumnsProperty, newValue);

        private static void DynamicColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is DataGrid dataGrid)
            {
                ClearDataGridColumns(dataGrid);

                UnsubscribeFromOldValue(dataGrid, e.OldValue);

                if (e.NewValue is ObservableCollection<DataGridColumn> newDataGridColumns)
                {
                    AddColumns(dataGrid, newDataGridColumns);
                    SubscribeToNewValue(dataGrid, newDataGridColumns);
                }
            }
            else
            {
                throw new InvalidOperationException($"Property should be attached to instance of type {nameof(DataGrid)}.");
            }
        }

        private static void SubscribeToNewValue(DataGrid dataGrid, ObservableCollection<DataGridColumn> newDataGridColumns)
        {
            void eventHandler(object _, NotifyCollectionChangedEventArgs collectionChangedEvent) =>
                NewDataGridColumns_CollectionChanged(dataGrid, collectionChangedEvent);
            dynamicColumnCollectionChangedEventHandlersPerDataGrid[dataGrid] = eventHandler;
            newDataGridColumns.CollectionChanged += eventHandler;
        }

        private static void UnsubscribeFromOldValue(DataGrid dataGrid, object oldValue)
        {
            if (oldValue is ObservableCollection<DataGridColumn> originalDataGridColumns &&
                dynamicColumnCollectionChangedEventHandlersPerDataGrid.TryGetValue(dataGrid, out var h))
            {
                originalDataGridColumns.CollectionChanged -= h;
                dynamicColumnCollectionChangedEventHandlersPerDataGrid.Remove(dataGrid);
            }
        }

        private static void NewDataGridColumns_CollectionChanged(DataGrid dataGridToUpdate, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    ResetColumns(dataGridToUpdate, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Add:
                    AddColumns(dataGridToUpdate, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveColumns(dataGridToUpdate, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    dataGridToUpdate.Columns.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    dataGridToUpdate.Columns[e.NewStartingIndex] = (DataGridColumn)e.NewItems[0];
                    break;
                default: throw new InvalidEnumArgumentException("e.Action", (int)e.Action, typeof(NotifyCollectionChangedAction));
            }
        }

        private static void ResetColumns(DataGrid dataGridToUpdate, IEnumerable columns)
        {
            ClearDataGridColumns(dataGridToUpdate);
            if(columns != null)
            {
                AddColumns(dataGridToUpdate, columns);
            }
        }

        private static void ClearDataGridColumns(DataGrid dataGridToUpdate)
        {
            dataGridToUpdate.Columns.Clear();
        }

        private static void AddColumns(DataGrid dataGridToUpdate, IEnumerable columns)
        {
            foreach (DataGridColumn c in columns)
            {
                dataGridToUpdate.Columns.Add(c);
            }
        }

        private static void RemoveColumns(DataGrid dataGridToUpdate, IEnumerable columns)
        {
            foreach (DataGridColumn c in columns)
            {
                dataGridToUpdate.Columns.Remove(c);
            }
        }
    }
}
