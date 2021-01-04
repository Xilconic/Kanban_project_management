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
using KanbanProjectManagementApp.Views.InterfaceImplementations;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class WorkEstimationsCsvWriter_specification
    {
        [Fact]
        public void WHEN_constructing_writer_AND_text_writer_is_null_THEN_throw_ArgumentNullException()
        {
            static void call() => new WorkEstimationsCsvWriter(null);
            var actualException = Assert.Throws<ArgumentNullException>("textWriter", call);
        }

        public static IEnumerable<object[]> WriterScenarios
        {
            get
            {
                yield return new object[]
                {
                    new TimeTillCompletionEstimationsCollection(1, 1),
@"Number of days till completion in simulation;Is indeterminate
"
                };

                var unfinishedProject = new Project(1);
                var unfinishedRoadmap = new Roadmap(new[] { unfinishedProject });
                var estimationsForUncompletedRoadmap = new TimeTillCompletionEstimationsCollection(1, 1);
                estimationsForUncompletedRoadmap.AddEstimationsForSimulation(
                    new WorkEstimate(unfinishedRoadmap, 1.1),
                    new[] { new WorkEstimate(unfinishedProject, 1.1) });
                yield return new object[]
                {
                    estimationsForUncompletedRoadmap,
@"Number of days till completion in simulation;Is indeterminate
1.1;True
"
                };

                var finishedProject = new Project(1);
                var finishedRoadmap = new Roadmap(new[] { finishedProject });
                finishedProject.CompleteWorkItem();
                var estimationsForMultipleSimulations = new TimeTillCompletionEstimationsCollection(2, 1);
                estimationsForMultipleSimulations.AddEstimationsForSimulation(
                    new WorkEstimate(unfinishedRoadmap, 2.2),
                    new[] { new WorkEstimate(unfinishedProject, 2.2) });
                estimationsForMultipleSimulations.AddEstimationsForSimulation(
                    new WorkEstimate(finishedRoadmap, 3.3),
                    new[] { new WorkEstimate(finishedProject, 3.3) });
                yield return new object[]
                {
                    estimationsForMultipleSimulations,
@"Number of days till completion in simulation;Is indeterminate
2.2;True
3.3;False
"
                };
            }
        }

        [Theory]
        [MemberData(nameof(WriterScenarios))]
        public void GIVEN_empty_work_estimations_WHEN_writing_THEN_only_header_has_been_written(
            TimeTillCompletionEstimationsCollection workEstimates,
            string expectedOutput)
        {
            using var stringWriter = new StringWriter();
            var workEstimationsCsvWriter = new WorkEstimationsCsvWriter(stringWriter);
            workEstimationsCsvWriter.Write(workEstimates);

            var output = stringWriter.ToString();
            Assert.Equal(expectedOutput, output);
        }
    }
}
