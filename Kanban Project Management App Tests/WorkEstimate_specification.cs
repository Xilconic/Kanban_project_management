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
using Xunit;
using KanbanProjectManagementApp.Domain;
using System.Globalization;

namespace KanbanProjectManagementApp.Tests
{
    public class WorkEstimate_specification : IDisposable
    {
        private readonly CultureInfo originalThreadCulture;
        private readonly Project someFinishedProject;

        public WorkEstimate_specification()
        {
            originalThreadCulture = CultureInfo.DefaultThreadCurrentCulture;

            someFinishedProject = CreateFinishedProject();
        }

        public void Dispose()
        {
            CultureInfo.DefaultThreadCurrentCulture = originalThreadCulture;
        }

        public static IEnumerable<object[]> InvalidNumberOfWorkingDaysScenarios
        {
            get
            {
                yield return new object[] { double.MinValue };
                yield return new object[] { -double.Epsilon };
                yield return new object[] { double.NegativeInfinity };
                yield return new object[] { double.NaN };
            }
        }

        [Fact]
        public void GIVEN_project_null_WHEN_creating_new_instance_THEN_throw_ArgumentNullException()
        {
            static void call() => new WorkEstimate(null, 1.1);

            Assert.Throws<ArgumentNullException>("project", call);
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfWorkingDaysScenarios))]
        public void WHEN_constructing_new_instance_with_project_AND_with_negative_number_of_days_THEN_throw_ArgumentOutOfRangeException(
    double invalidNumberOfWorkingDays)
        {
            void call() => new WorkEstimate(someFinishedProject, invalidNumberOfWorkingDays);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("estimatedNumberOfWorkingDaysRequiredToFinishWork", call);
            Assert.StartsWith("Estimate of working days should be greater or equal to 0.", actualException.Message);
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfWorkingDaysScenarios))]
        public void WHEN_constructing_new_instance_with_negative_number_of_days_THEN_throw_ArgumentOutOfRangeException(
            double invalidNumberOfWorkingDays)
        {
            void call() => new WorkEstimate(invalidNumberOfWorkingDays, default);

            var actualException = Assert.Throws<ArgumentOutOfRangeException>("estimatedNumberOfWorkingDaysRequiredToFinishWork", call);
            Assert.StartsWith("Estimate of working days should be greater or equal to 0.", actualException.Message);
        }

        public static IEnumerable<object[]> WorkEstimateToStringScenarios
        {
            get
            {
                yield return new object[] { new WorkEstimate(CreateFinishedProject(), 1.1), "1.1 working day(s)" };

                Project unfinishedProject = new Project(1);
                yield return new object[] { new WorkEstimate(unfinishedProject, 2.2), "2.2 working day(s) [Indeterminate]" };
            }
        }

        [Theory]
        [MemberData(nameof(WorkEstimateToStringScenarios))]
        public void GIVEN_some_work_estimate_WHEN_converting_to_string_THEN_return_expected_text(
            WorkEstimate estimate, string expectedText)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);

            var text = estimate.ToString();

            Assert.Equal(expectedText, text);
        }

        private static Project CreateFinishedProject()
        {
            Project project = new Project(1);
            project.CompleteWorkItem();
            return project;
        }
    }
}
