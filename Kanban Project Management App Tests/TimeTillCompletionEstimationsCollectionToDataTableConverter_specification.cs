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
using KanbanProjectManagementApp.Views.ValueConverters;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class TimeTillCompletionEstimationsCollectionToDataTableConverter_specification
    {
        private readonly TimeTillCompletionEstimationsCollectionToDataTableConverter converter;

        public TimeTillCompletionEstimationsCollectionToDataTableConverter_specification()
        {
            converter = new TimeTillCompletionEstimationsCollectionToDataTableConverter();
        }

        [Fact]
        public void GIVEN_non_TimeTillCompletionEstimationsCollection_WHEN_converting_THEN_always_return_UnsetValue()
        {
            var convertedValue = converter.Convert(new[] { new object() }, null, null, null);
            Assert.Equal(DependencyProperty.UnsetValue, convertedValue);
        }

        [Fact]
        public void GIVEN_some_estimations_WHEN_converting_THEN_return_DataView()
        {
            var someEstimations = new TimeTillCompletionEstimationsCollection(1, 1);

            Project project = new Project(1);
            var roadmap = new Roadmap(new[] { project });
            project.CompleteWorkItem();

            someEstimations.AddEstimationsForSimulation(new WorkEstimate(roadmap, 1.1), new[] { new WorkEstimate(project, 1.1) });

            var convertedValue = converter.Convert(new[] { someEstimations }, null, null, new CultureInfo("en-US"));

            var dataView = Assert.IsType<DataView>(convertedValue);
            var rowView = (DataRowView)Assert.Single(dataView);
            Assert.Equal("1.1", rowView.Row.Field<string>("Number of days till completion of 'Roadmap' in simulation"));
            Assert.Equal("False", rowView.Row.Field<string>("Is 'Roadmap' estimation indeterminate"));
        }

        [Fact]
        public void WHEN_converting_back_THEN_always_throw_NotImplementedException()
        {
            void call() => converter.ConvertBack(null, null, null, null);
            Assert.Throws<NotImplementedException>(call);
        }
    }
}
