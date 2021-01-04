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

namespace KanbanProjectManagementApp.Domain
{
    public class TimeTillCompletionEstimationsCollection
    {
        private readonly List<WorkEstimate> roadmapEstimatesOfEachSimulation;
        private readonly List<List<WorkEstimate>> projectEstimatesOfEachSimulation = new List<List<WorkEstimate>>();

        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfSimulations"/> or <paramref name="numberOfProjectsInRoadmap"/>
        /// are not at least 1.</exception>
        public TimeTillCompletionEstimationsCollection(int numberOfSimulations, int numberOfProjectsInRoadmap)
        {
            if (numberOfSimulations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSimulations), "Number of simulations should be at least 1.");
            }
            if (numberOfProjectsInRoadmap <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfProjectsInRoadmap), "Number of projects in roadmap should be at least 1.");
            }

            NumberOfSimulations = numberOfSimulations;
            NumberOfProjectsInRoadmap = numberOfProjectsInRoadmap;

            roadmapEstimatesOfEachSimulation = new List<WorkEstimate>(NumberOfSimulations);

            projectEstimatesOfEachSimulation = new List<List<WorkEstimate>>(NumberOfProjectsInRoadmap);
            for(int i = 0; i < NumberOfProjectsInRoadmap; i++)
            {
                projectEstimatesOfEachSimulation.Add(new List<WorkEstimate>(NumberOfSimulations));
            }
        }

        public IReadOnlyCollection<WorkEstimate> RoadmapEstimations => roadmapEstimatesOfEachSimulation;

        public int NumberOfProjectsInRoadmap { get; }
        public int NumberOfSimulations { get; }
        public int Count => RoadmapEstimations.Count;

        /// <summary>
        /// Adds the time-till-completion estimates of a simulation to the collection.
        /// </summary>
        /// <param name="roadmapEstimate">The estimate for the whole roadmap.</param>
        /// <param name="projectEstimates">The estimates for each of the individual projects.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="projectEstimates"/> is not consistent with expected projects.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="roadmapEstimate"/> or <paramref name="projectEstimates"/> is null.</exception>
        public void AddEstimationsForSimulation(WorkEstimate roadmapEstimate, IReadOnlyList<WorkEstimate> projectEstimates)
        {
            if (roadmapEstimate is null)
            {
                throw new ArgumentNullException(nameof(roadmapEstimate));
            }
            if (projectEstimates is null)
            {
                throw new ArgumentNullException(nameof(projectEstimates));
            }
            if (roadmapEstimatesOfEachSimulation.Capacity == roadmapEstimatesOfEachSimulation.Count)
            {
                throw new InvalidOperationException("Adding these estimation would exceed the expected number of simulations.");
            }

            ValidateThatNumberOfEstimatesMatchExpectedCount(projectEstimates);

            roadmapEstimatesOfEachSimulation.Add(roadmapEstimate);

            for (int i = 0; i < projectEstimates.Count; i++)
            {
                var currentProjectEstimatesForProject = projectEstimatesOfEachSimulation[i];
                var newProjectEstimateForThisSimulation = projectEstimates[i];
                ValidateThatProjectIdentifiersAreConsistent(currentProjectEstimatesForProject, newProjectEstimateForThisSimulation);

                currentProjectEstimatesForProject.Add(newProjectEstimateForThisSimulation);
            }
        }

        /// <exception cref="ArgumentOutOfRangeException"/>
        internal WorkEstimate GetRoadmapEstimationForSimulation(int simulationIndex) =>
            roadmapEstimatesOfEachSimulation[simulationIndex];

        internal WorkEstimate GetProjectEstimationForSimulation(int projectIndex, int simulationIndex) =>
            projectEstimatesOfEachSimulation[projectIndex][simulationIndex];

        /// <summary>
        /// Gets the work estimates for a given project.
        /// </summary>
        /// <param name="projectIndex">The 0-based project that was added using <see cref="AddEstimationsForSimulation"/>.</param>
        /// <returns>The work estimates for the given project.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="projectIndex"/> is outside the domain of available projects.</exception>
        internal IReadOnlyCollection<WorkEstimate> this[int projectIndex]
        {
            get
            {
                if (projectIndex < 0 || projectIndex >= projectEstimatesOfEachSimulation.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(projectIndex), $"Project index must be in range [0, {projectEstimatesOfEachSimulation.Count-1}].");
                }

                return projectEstimatesOfEachSimulation[projectIndex];
            }
        }

        private void ValidateThatNumberOfEstimatesMatchExpectedCount(IReadOnlyList<WorkEstimate> projectEstimates)
        {
            if (projectEstimates.Count != projectEstimatesOfEachSimulation.Count)
            {
                throw new ArgumentException($"Expected {projectEstimatesOfEachSimulation.Count} project estimate(s), but got provided {projectEstimates.Count}.", nameof(projectEstimates));
            }
        }

        private void ValidateThatProjectIdentifiersAreConsistent(List<WorkEstimate> currentProjectEstimatesForProject, WorkEstimate newProjectEstimateForThisSimulation)
        {
            if (!IsProjectIdentifierConsistentWithPriorProjectSimulations(newProjectEstimateForThisSimulation, currentProjectEstimatesForProject))
            {
                throw new ArgumentException("A project estimate's identifier mismatches a the expected project's identifier.", "projectEstimates");
            }
        }

        private bool IsProjectIdentifierConsistentWithPriorProjectSimulations(WorkEstimate newProjectEstimate, IReadOnlyList<WorkEstimate> currentProjectEstimatesForProject)
        {
            return currentProjectEstimatesForProject.Count == 0 ||
                   newProjectEstimate.Identifier == currentProjectEstimatesForProject[0].Identifier;
        }
    }
}
