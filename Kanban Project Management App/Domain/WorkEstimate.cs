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

namespace KanbanProjectManagementApp.Domain
{
    public class WorkEstimate
    {
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="estimatedNumberOfWorkingDaysRequiredToFinishWork"/> is not at least 0.
        /// </exception>
        public WorkEstimate(Project project, double estimatedNumberOfWorkingDaysRequiredToFinishWork) :
            this(estimatedNumberOfWorkingDaysRequiredToFinishWork, GetHasWorkToBeCompleted(project))
        {
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="estimatedNumberOfWorkingDaysRequiredToFinishWork"/> is not at least 0.
        /// </exception>
        public WorkEstimate(Roadmap roadmap, double estimatedNumberOfWorkingDaysRequiredToFinishWork) :
            this(estimatedNumberOfWorkingDaysRequiredToFinishWork, GetHasWorkToBeCompleted(roadmap))
        {
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="estimatedNumberOfWorkingDaysRequiredToFinishWork"/> is not at least 0.
        /// </exception>
        public WorkEstimate(double estimatedNumberOfWorkingDaysRequiredToFinishWork, bool isEstimateIndeterminate)
        {
            ValidateEstimatedNumberOfWorkingDaysToBeAtLeastZero(estimatedNumberOfWorkingDaysRequiredToFinishWork);

            EstimatedNumberOfWorkingDaysRequired = estimatedNumberOfWorkingDaysRequiredToFinishWork;
            IsIndeterminate = isEstimateIndeterminate;
        }

        public double EstimatedNumberOfWorkingDaysRequired { get; }
        public bool IsIndeterminate { get; }

        public override string ToString()
        {
            var determinacyString = IsIndeterminate ? " [Indeterminate]" : string.Empty;
            return $"{EstimatedNumberOfWorkingDaysRequired} working day(s){determinacyString}";
        }

        private static bool GetHasWorkToBeCompleted(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return project.HasWorkToBeCompleted;
        }

        private static bool GetHasWorkToBeCompleted(Roadmap roadmap)
        {
            if(roadmap is null)
            {
                throw new ArgumentNullException(nameof(roadmap));
            }

            return roadmap.HasWorkToBeCompleted;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="estimatedNumberOfWorkingDaysRequiredToFinishWork"/> is not at least 0.
        /// </exception>
        private static void ValidateEstimatedNumberOfWorkingDaysToBeAtLeastZero(double estimatedNumberOfWorkingDaysRequiredToFinishWork)
        {
            if (estimatedNumberOfWorkingDaysRequiredToFinishWork < 0.0 ||
                double.IsNaN(estimatedNumberOfWorkingDaysRequiredToFinishWork))
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedNumberOfWorkingDaysRequiredToFinishWork), "Estimate of working days should be greater or equal to 0.");
            }
        }
    }
}
