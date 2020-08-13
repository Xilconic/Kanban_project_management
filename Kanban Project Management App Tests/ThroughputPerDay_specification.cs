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
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class ThroughputPerDay_specification
    {
        [Theory]
        [InlineData(0.0-double.Epsilon)]
        [InlineData(-1)]
        [InlineData(double.MinValue)]
        [InlineData(double.NegativeInfinity)]
        public void WHEN_trying_to_construct_a_throughput_with_a_negative_value_THEN_throw_ArgumentOutOfRangeException(
            double invalidThroughput)
        {
            Action call = () => new ThroughputPerDay(invalidThroughput);
            AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(call, invalidThroughput);
        }

        [Fact]
        public void WHEN_trying_to_construct_a_throughput_with_a_NaN_value_THEN_throw_ArgumentOutOfRangeException()
        {
            var invalidThroughput = double.NaN;
            Action call = () => new ThroughputPerDay(invalidThroughput);
            AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(call, invalidThroughput);
        }

        [Fact]
        public void GIVEN_some_throughput_WHEN_compared_to_itself_THEN_it_is_considered_equal()
        {
            var someThroughput = new ThroughputPerDay(3.0);

            Assert.True(someThroughput.Equals(someThroughput));
        }

        [Fact]
        public void GIVEN_some_throughput_AND_another_throughput_with_the_same_value_WHEN_compared_to_each_other_THEN_it_is_considered_equal()
        {
            const double someThroughputValue = 4.0;
            var someThroughput = new ThroughputPerDay(someThroughputValue);
            var anotherThroughputWithSameValue = new ThroughputPerDay(someThroughputValue);
            var anotherThroughputBoxedAsObject = (object)anotherThroughputWithSameValue;

            Assert.True(someThroughput.Equals(anotherThroughputWithSameValue));
            Assert.True(anotherThroughputWithSameValue.Equals(someThroughput));

            Assert.True(someThroughput.Equals(anotherThroughputBoxedAsObject));
            Assert.True(anotherThroughputBoxedAsObject.Equals(someThroughput));
        }

        [Fact]
        public void GIVEN_some_throughput_AND_another_throughput_with_a_different_value_WHEN_compared_to_each_other_THEN_it_is_considered_different()
        {
            var someThroughput = new ThroughputPerDay(5.0);
            var anotherThroughputWithDifferentValue = new ThroughputPerDay(6.0);
            var anotherThroughputBoxedAsObject = (object)anotherThroughputWithDifferentValue;

            Assert.False(someThroughput.Equals(anotherThroughputWithDifferentValue));
            Assert.False(anotherThroughputWithDifferentValue.Equals(someThroughput));

            Assert.False(someThroughput.Equals(anotherThroughputBoxedAsObject));
            Assert.False(anotherThroughputBoxedAsObject.Equals(someThroughput));
        }

        [Fact]
        public void GIVEN_some_throughput_AND_another_throughput_with_the_same_value_WHEN_comparing_hash_codes_THEN_hash_codes_are_equal()
        {
            const double someThroughputValue = 4.0;
            var someThroughput = new ThroughputPerDay(someThroughputValue);
            var anotherThroughputWithSameValue = new ThroughputPerDay(someThroughputValue);
            var anotherThroughputBoxedAsObject = (object)anotherThroughputWithSameValue;

            Assert.Equal(someThroughput.GetHashCode(), anotherThroughputWithSameValue.GetHashCode());
            Assert.Equal(someThroughput.GetHashCode(), anotherThroughputBoxedAsObject.GetHashCode());
        }

        private static void AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(Action call, double invalidThroughput)
        {
            var actualException = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.Equal("numberOfWorkItemsFinished", actualException.ParamName);
            Assert.Equal(invalidThroughput, actualException.ActualValue);
            Assert.Contains("Must be in range [0.0, PositiveInfinity).", actualException.Message);
        }
    }
}
