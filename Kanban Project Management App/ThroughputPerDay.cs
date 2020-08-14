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

namespace KanbanProjectManagementApp
{
    public struct ThroughputPerDay : IEquatable<ThroughputPerDay>
    {
        private readonly double numberOfWorkItemsFinished;

        public ThroughputPerDay(double numberOfWorkItemsFinished)
        {
            if (numberOfWorkItemsFinished < 0.0 || double.IsNaN(numberOfWorkItemsFinished))
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfWorkItemsFinished), numberOfWorkItemsFinished, "Must be in range [0.0, PositiveInfinity).");
            }

            this.numberOfWorkItemsFinished = numberOfWorkItemsFinished;
        }

        public double GetNumberOfWorkItemsPerDay()
        {
            return numberOfWorkItemsFinished;
        }

        public override bool Equals(object obj) =>
            obj is ThroughputPerDay mys && Equals(mys);

        public bool Equals(ThroughputPerDay other) =>
            numberOfWorkItemsFinished == other.numberOfWorkItemsFinished;

        public override int GetHashCode() =>
            numberOfWorkItemsFinished.GetHashCode();

        public static ThroughputPerDay operator +(ThroughputPerDay a, ThroughputPerDay b) =>
            new ThroughputPerDay(a.numberOfWorkItemsFinished + b.numberOfWorkItemsFinished);

        public static ThroughputPerDay operator /(ThroughputPerDay numerator, double denominator)
        {
            if(double.IsPositiveInfinity(denominator) && double.IsPositiveInfinity(numerator.numberOfWorkItemsFinished))
            {
                throw new ArithmeticException("Cannot divide an infinite throughput by infinite, as the result is indeterminate.");
            }

            return new ThroughputPerDay(numerator.numberOfWorkItemsFinished / denominator);
        }

        public override string ToString() =>
            $"{numberOfWorkItemsFinished} / day";
    }
}
