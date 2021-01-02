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
using System.Collections.Generic;

namespace KanbanProjectManagementApp.Domain
{
    internal class TimeTillCompletionEstimationsCollection
    {
        private readonly List<WorkEstimate> roadmapEstimatesOfEachSimulation;
        private readonly List<List<WorkEstimate>> projectEstimatesOfEachSimulation = new List<List<WorkEstimate>>();

        public TimeTillCompletionEstimationsCollection(int numberOfSimulations, int numberOfProjectsInRoadmap)
        {
            roadmapEstimatesOfEachSimulation = new List<WorkEstimate>(numberOfSimulations);

            projectEstimatesOfEachSimulation = new List<List<WorkEstimate>>(numberOfProjectsInRoadmap);
            for(int i = 0; i < numberOfProjectsInRoadmap; i++)
            {
                projectEstimatesOfEachSimulation.Add(new List<WorkEstimate>(numberOfSimulations));
            }
        }

        public void AddEstimationsForSimulation(WorkEstimate roadmapEstimate, IReadOnlyList<WorkEstimate> projectEstimates)
        {
            roadmapEstimatesOfEachSimulation.Add(roadmapEstimate);
            for(int i = 0; i < projectEstimates.Count; i++)
            {
                projectEstimatesOfEachSimulation[i].Add(projectEstimates[i]);
            }
        }

        public IReadOnlyCollection<WorkEstimate> RoadmapEstimation => roadmapEstimatesOfEachSimulation;
    }
}
