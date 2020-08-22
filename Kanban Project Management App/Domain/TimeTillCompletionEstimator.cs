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
    internal class TimeTillCompletionEstimator
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

        public WorkEstimate Estimate(int numberOfWorkItemsToBeCompleted)
        {
            ValidateThatAtLeastOneWorkItemsHasToBeCompleted(numberOfWorkItemsToBeCompleted);
            ValidateThatInputMetricsHaveAtLeastOneElement();

            return EstimateWorkRequiredForFinishWorkItems(numberOfWorkItemsToBeCompleted);
        }

        private static void ValidateMaximumNumberOfIterationsIsGreaterThanZero(int maximumNumberOfIterations)
        {
            if (maximumNumberOfIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfIterations));
            }
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfWorkItemsToBeCompleted"/> is not larger than 0.
        /// </exception>
        private static void ValidateThatAtLeastOneWorkItemsHasToBeCompleted(int numberOfWorkItemsToBeCompleted)
        {
            if (numberOfWorkItemsToBeCompleted <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfWorkItemsToBeCompleted), "Number of workitems to complete should be at least 1.");
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

        private WorkEstimate EstimateWorkRequiredForFinishWorkItems(double workRemainingToBeDone)
        {
            double estimatedNumberOfWorkingDaysRequiredToFinishWork = 0.0;
            int iterationNumber = 0;
            do
            {
                var throughputOfThatDay = GetThroughputOfThatDay();
                estimatedNumberOfWorkingDaysRequiredToFinishWork += GetAmountOfWorkingDayConsumed(workRemainingToBeDone, throughputOfThatDay);
                workRemainingToBeDone -= throughputOfThatDay;
                iterationNumber++;
            }
            while (IsThereStillWorkRemaining(workRemainingToBeDone) && !IsMaximumNumberOfIterationsReached(iterationNumber));

            return new WorkEstimate(estimatedNumberOfWorkingDaysRequiredToFinishWork, IsThereStillWorkRemaining(workRemainingToBeDone));
        }

        private double GetThroughputOfThatDay()
        {
            return GetRandomInputMetric().Throughput.GetNumberOfWorkItemsPerDay();
        }

        private InputMetric GetRandomInputMetric()
        {
            return inputMetrics[rng.GetRandomIndex(inputMetrics.Count)];
        }

        private static double GetAmountOfWorkingDayConsumed(double workRemainingToBeDone, double throughputOfThatDay)
        {
            return throughputOfThatDay <= workRemainingToBeDone ?
                1.0 :
                workRemainingToBeDone / throughputOfThatDay;
        }

        private static bool IsThereStillWorkRemaining(double workRemainingToBeDone)
        {
            return workRemainingToBeDone > 0.0;
        }

        private bool IsMaximumNumberOfIterationsReached(int iterationNumber)
        {
            return iterationNumber >= maximumNumberOfIterations;
        }
    }
}
