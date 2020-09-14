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
using System.Linq;

namespace KanbanProjectManagementApp.Domain
{
    internal class MonteCarloTimeTillCompletionEstimator
    {
        private readonly int numberOfSimulations;
        private readonly int maximumNumberOfIterations;
        private readonly IReadOnlyList<InputMetric> inputMetrics;

        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maximumNumberOfIterations"/> or <paramref name="numberOfSimulations"/> is not at least 1.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputMetrics"/> is null.</exception>
        public MonteCarloTimeTillCompletionEstimator(int numberOfSimulations, int maximumNumberOfIterations, IReadOnlyList<InputMetric> inputMetrics)
        {
            ValidateAtLeastOneSimulation(numberOfSimulations);
            ValidateAtLeastOneIteration(maximumNumberOfIterations);

            this.numberOfSimulations = numberOfSimulations;
            this.maximumNumberOfIterations = maximumNumberOfIterations;
            this.inputMetrics = inputMetrics ?? throw new ArgumentNullException(nameof(inputMetrics));
        }

        /// <exception cref="InvalidOperationException">Thrown when <see cref="inputMetrics"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfWorkItemsToComplete"/> is not at least 1.</exception>
        public IReadOnlyCollection<WorkEstimate> Estimate(int numberOfWorkItemsToComplete)
        {
            ValidateAtLeastOneInputMetric();
            ValidateAtLeastOneWorkItemToComplete(numberOfWorkItemsToComplete);

            var simulationEstimator = new TimeTillCompletionEstimator(inputMetrics, new RandomNumberGenerator(), maximumNumberOfIterations);
            var simulationResults = new WorkEstimate[numberOfSimulations];
            for (int i = 0; i < numberOfSimulations; i++)
            {
                var roadmap = new Roadmap(new[] { new Project(numberOfWorkItemsToComplete) });
                simulationResults[i] = simulationEstimator.Estimate(roadmap).First();
            }

            return simulationResults;
        }

        private static void ValidateAtLeastOneSimulation(int numberOfSimulations)
        {
            if (numberOfSimulations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSimulations), "Number of simulations should be at least 1.");
            }
        }

        private static void ValidateAtLeastOneIteration(int maximumNumberOfIterations)
        {
            if (maximumNumberOfIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfIterations), "Maximum number of iterations should be at least 1.");
            }
        }

        private void ValidateAtLeastOneInputMetric()
        {
            if (inputMetrics.Count == 0)
            {
                throw new InvalidOperationException("At least 1 datapoint of input metrics is required for estimation.");
            }
        }

        private static void ValidateAtLeastOneWorkItemToComplete(int numberOfWorkItemsToComplete)
        {
            if (numberOfWorkItemsToComplete <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfWorkItemsToComplete), "Number of workitems to complete should be at least 1.");
            }
        }
    }
}
