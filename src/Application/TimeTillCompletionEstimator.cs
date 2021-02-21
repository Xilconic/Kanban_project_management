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
using System;
using System.Collections.Generic;
using System.Linq;

namespace KanbanProjectManagementApp.Application
{
    public class TimeTillCompletionEstimator
    {
        private readonly IReadOnlyList<InputMetric> inputMetrics;
        private readonly IRandomNumberGenerator rng;
        private readonly int maximumNumberOfIterations;

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="maximumNumberOfIterations"/> is not greater than 0.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="inputMetrics"/> or <paramref name="rng"/> is null.
        /// </exception>
        public TimeTillCompletionEstimator(
            IReadOnlyList<InputMetric> inputMetrics,
            IRandomNumberGenerator rng,
            int maximumNumberOfIterations)
        {
            ValidateMaximumNumberOfIterationsIsGreaterThanZero(maximumNumberOfIterations);

            this.inputMetrics = inputMetrics ?? throw new ArgumentNullException(nameof(inputMetrics));
            this.rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.maximumNumberOfIterations = maximumNumberOfIterations;
        }

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="roadmap"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="roadmap"/> has no work to be completed.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="inputMetrics"/> doesn't have at least 1 element.</exception>
        /// <returns>A collection of work estimates. The first element is the estimate the complete the whole <paramref name="roadmap"/>.
        /// The remaining elements are the work estimates for the projects in <paramref name="roadmap"/>.</returns>
        public IReadOnlyList<WorkEstimate> Estimate(Roadmap roadmap)
        {
            ValidateThatRoadmapHasWorkToBeCompleted(roadmap);
            ValidateThatInputMetricsHaveAtLeastOneElement();

            return EstimateWorkRequiredForFinishWorkItems(roadmap);
        }

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="roadmap"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="roadmap"/> has no work to be completed.</exception>
        private void ValidateThatRoadmapHasWorkToBeCompleted(Roadmap roadmap)
        {
            if (roadmap is null)
            {
                throw new ArgumentNullException(nameof(roadmap));
            }

            if (!roadmap.HasWorkToBeCompleted)
            {
                throw new ArgumentOutOfRangeException(nameof(roadmap), "Roadmap should have work to be completed.");
            }
        }

        private static void ValidateMaximumNumberOfIterationsIsGreaterThanZero(int maximumNumberOfIterations)
        {
            if (maximumNumberOfIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfIterations));
            }
        }

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="inputMetrics"/> doesn't have at least 1 element.
        /// </exception>
        private void ValidateThatInputMetricsHaveAtLeastOneElement()
        {
            if (inputMetrics.Count == 0)
            {
                throw new InvalidOperationException("At least 1 datapoint of input metrics is required for estimation.");
            }
        }

        /// <returns>A collection of work estimates. The first element is the estimate the complete the whole <paramref name="roadmap"/>.
        /// The remaining elements are the work estimates for the projects in <paramref name="roadmap"/>.</returns>
        private IReadOnlyList<WorkEstimate> EstimateWorkRequiredForFinishWorkItems(Roadmap roadmap)
        {
            var estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject = roadmap.Projects.ToDictionary(p => p, p => 0.0);

            int iterationNumber = 0;
            double leftOverThroughput = 0.0;
            double numberOfDaysToFinishRoadmap = 0.0;

            var workedProjects = new HashSet<Project>(estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject.Count);
            var unworkedProjects = new HashSet<Project>(estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject.Keys);
            do
            {
                var throughputOfThatDay = GetThroughputOfThatDay() + leftOverThroughput;
                var amountOfWorkingDayConsumed = GetAmountOfWorkingDayConsumed(roadmap, throughputOfThatDay);

                double numberOfWorkItemsFinishedThisDay = Math.Floor(throughputOfThatDay);
                for (int i = 0; i < numberOfWorkItemsFinishedThisDay; i++)
                {
                    var availableProjects = roadmap.GetCurrentAvailableProjectsThatHaveWorkToBeCompleted();
                    if (availableProjects.Count == 0)
                    {
                        // Nothing to do any more, so early exit from the loop.
                        break;
                    }

                    var projectToWorkOn = GetRandomElement(availableProjects);

                    projectToWorkOn.CompleteWorkItem();

                    unworkedProjects.Remove(projectToWorkOn);
                    workedProjects.Add(projectToWorkOn);
                }

                // Projects actually worked on can finish sooner than 1 day:
                foreach (var p in workedProjects)
                {
                    estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject[p] += amountOfWorkingDayConsumed;
                }
                numberOfDaysToFinishRoadmap += amountOfWorkingDayConsumed;

                // Projects not worked on always complete 1 day later:
                foreach (var p in unworkedProjects)
                {
                    estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject[p] += 1;
                }

                leftOverThroughput = throughputOfThatDay - numberOfWorkItemsFinishedThisDay;
                iterationNumber++;

                foreach (var p in workedProjects.Where(p => p.HasWorkToBeCompleted))
                {
                    unworkedProjects.Add(p);
                }
                workedProjects.Clear();
            }
            while (roadmap.HasWorkToBeCompleted && !IsMaximumNumberOfIterationsReached(iterationNumber));

            return CreateWorkEstimatesResult(roadmap, numberOfDaysToFinishRoadmap, estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject);
        }

        private double GetThroughputOfThatDay()
        {
            return GetRandomInputMetric().Throughput.GetNumberOfWorkItemsPerDay();
        }

        private InputMetric GetRandomInputMetric()
        {
            return GetRandomElement(inputMetrics);
        }

        private T GetRandomElement<T>(IReadOnlyList<T> collection)
        {
            return collection[rng.GetRandomIndex(collection.Count)];
        }

        private double GetAmountOfWorkingDayConsumed(Roadmap roadmap, double throughputOfThatDay)
        {
            return throughputOfThatDay <= roadmap.TotalOfWorkRemaining ?
                1.0 :
                roadmap.TotalOfWorkRemaining / throughputOfThatDay;
        }

        private bool IsMaximumNumberOfIterationsReached(int iterationNumber)
        {
            return iterationNumber >= maximumNumberOfIterations;
        }

        private static IReadOnlyList<WorkEstimate> CreateWorkEstimatesResult(
            Roadmap roadmap,
            double numberOfDaysToFinishRoadmap,
            IReadOnlyDictionary<Project, double> estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject)
        {
            var workEstimates = new WorkEstimate[estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject.Count + 1];
            workEstimates[0] = new WorkEstimate(roadmap, numberOfDaysToFinishRoadmap);
            var index = 1;
            foreach (KeyValuePair<Project, double> keyValuePair in estimatedNumberOfWorkingDaysRequiredToFinishWorkPerProject)
            {
                workEstimates[index++] = new WorkEstimate(keyValuePair.Key, keyValuePair.Value);
            }
            return workEstimates;
        }
    }
}
