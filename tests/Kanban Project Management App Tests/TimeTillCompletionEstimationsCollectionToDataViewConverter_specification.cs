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
using KanbanProjectManagementApp.Domain;
using KanbanProjectManagementApp.Views.ValueConverters;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using Xunit;
using static KanbanProjectManagementApp.Application.RoadmapConfigurator;

namespace KanbanProjectManagementApp.Tests.Unit
{
    public class TimeTillCompletionEstimationsCollectionToDataViewConverter_specification
    {
        private readonly TimeTillCompletionEstimationsCollectionToDataViewConverter converter;

        public TimeTillCompletionEstimationsCollectionToDataViewConverter_specification()
        {
            converter = new TimeTillCompletionEstimationsCollectionToDataViewConverter();
        }

        [Fact]
        public void GIVEN_non_TimeTillCompletionEstimationsCollection_WHEN_converting_THEN_always_return_UnsetValue()
        {
            var convertedValue = converter.Convert(new[] { new object(), ConfigurationMode.Simple }, null, null, null);
            Assert.Equal(DependencyProperty.UnsetValue, convertedValue);
        }

        [Fact]
        public void GIVEN_non_ConfigurationModel_WHEN_converting_THEN_always_return_UnsetValue()
        {
            TimeTillCompletionEstimationsCollection someEstimations = CreateSomeRoadmapEstimations();

            var convertedValue = converter.Convert(new[] { someEstimations, new object() }, null, null, null);
            Assert.Equal(DependencyProperty.UnsetValue, convertedValue);
        }

        [Fact]
        public void GIVEN_some_estimations_and_simple_configuration_WHEN_converting_THEN_return_DataView()
        {
            TimeTillCompletionEstimationsCollection someEstimations = CreateSomeRoadmapEstimations();

            var convertedValue = converter.Convert(new object[] { someEstimations, ConfigurationMode.Simple }, null, null, new CultureInfo("en-US"));

            var dataView = Assert.IsType<DataView>(convertedValue);
            var rowView = (DataRowView)Assert.Single(dataView);
            Assert.Equal("3.3", rowView.Row.Field<string>("Number of days till completion of 'Roadmap' in simulation"));
            Assert.Equal("False", rowView.Row.Field<string>("Is 'Roadmap' estimation indeterminate"));
        }

        [Fact]
        public void GIVEN_some_estimations_and_advanced_configuration_WHEN_converting_THEN_return_DataView()
        {
            TimeTillCompletionEstimationsCollection someEstimations = CreateSomeRoadmapEstimations();

            var convertedValue = converter.Convert(new object[] { someEstimations, ConfigurationMode.Advanced }, null, null, new CultureInfo("en-US"));

            var dataView = Assert.IsType<DataView>(convertedValue);
            var rowView = (DataRowView)Assert.Single(dataView);
            Assert.Equal("3.3", rowView.Row.Field<string>("Number of days till completion of 'Roadmap' in simulation"));
            Assert.Equal("False", rowView.Row.Field<string>("Is 'Roadmap' estimation indeterminate"));
            Assert.Equal("1.1", rowView.Row.Field<string>("Number of days till completion of 'A' in simulation"));
            Assert.Equal("False", rowView.Row.Field<string>("Is 'A' estimation indeterminate"));
            Assert.Equal("2.2", rowView.Row.Field<string>("Number of days till completion of 'B' in simulation"));
            Assert.Equal("False", rowView.Row.Field<string>("Is 'B' estimation indeterminate"));
        }

        [Fact]
        public void WHEN_converting_back_THEN_always_throw_NotImplementedException()
        {
            void Call() => converter.ConvertBack(null, null, null, null);

            Assert.Throws<NotImplementedException>(Call);
        }

        private static TimeTillCompletionEstimationsCollection CreateSomeRoadmapEstimations()
        {
            Project projectA = new Project(1, default, "A");
            Project projectB = new Project(1, default, "B");
            Project[] projects = new[] { projectA, projectB };
            var roadmap = new Roadmap(projects);
            projectA.CompleteWorkItem();
            projectB.CompleteWorkItem();

            var someEstimations = new TimeTillCompletionEstimationsCollection(1, projects.Length);
            someEstimations.AddEstimationsForSimulation(
                new WorkEstimate(roadmap, 3.3),
                new[]
                {
                    new WorkEstimate(projectA, 1.1),
                    new WorkEstimate(projectB, 2.2)
                });
            return someEstimations;
        }
    }
}
