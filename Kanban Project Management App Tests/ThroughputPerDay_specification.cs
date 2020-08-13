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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xunit;

namespace KanbanProjectManagementApp.Tests
{
    public class ThroughputPerDay_specification
    {
        public class WHEN_trying_to_construct_a_throughput
        {
            [Theory]
            [InlineData(0.0 - double.Epsilon)]
            [InlineData(-1)]
            [InlineData(double.MinValue)]
            [InlineData(double.NegativeInfinity)]
            public void AND_a_negative_value_is_used_THEN_throw_ArgumentOutOfRangeException(double invalidThroughput)
            {
                Action call = () => new ThroughputPerDay(invalidThroughput);
                AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(call, invalidThroughput);
            }

            [Fact]
            public void AND_a_NaN_value_is_used_THEN_throw_ArgumentOutOfRangeException()
            {
                var invalidThroughput = double.NaN;
                Action call = () => new ThroughputPerDay(invalidThroughput);
                AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(call, invalidThroughput);
            }

            [Fact]
            public void AND_default_constructor_is_used_THEN_it_has_a_value_of_zero()
            {
                Assert.Equal(new ThroughputPerDay(0), default(ThroughputPerDay));
            }

            private static void AssertThatArgumentOutOfRangeExceptionIfThrownForInvalidThroughputValue(Action call, double invalidThroughput)
            {
                var actualException = Assert.Throws<ArgumentOutOfRangeException>(call);
                Assert.Equal("numberOfWorkItemsFinished", actualException.ParamName);
                Assert.Equal(invalidThroughput, actualException.ActualValue);
                Assert.Contains("Must be in range [0.0, PositiveInfinity).", actualException.Message);
            }
        }

        public class GIVEN_some_throughput : IDisposable
        {
            private readonly ThroughputPerDay someThroughput;
            private readonly CultureInfo? originalDefaultThreadCulture;

            public GIVEN_some_throughput()
            {
                someThroughput = new ThroughputPerDay(4.0);

                originalDefaultThreadCulture = CultureInfo.DefaultThreadCurrentCulture;
            }

            public void Dispose()
            {
                CultureInfo.DefaultThreadCurrentCulture = originalDefaultThreadCulture;
            }

            [Fact]
            public void WHEN_compared_to_itself_THEN_it_is_considered_equal()
            {
                Assert.True(someThroughput.Equals(someThroughput));
            }

            [Fact]
            public void WHEN_compared_to_some_other_object_THEN_it_is_considered_different()
            {
                object obj = new object();
                AssertObjectEqualsReturnsFalseCommutatively(someThroughput, obj);
            }

            public static IEnumerable<object[]> ToStringScenarios
            {
                get
                {
                    yield return new object[] { new ThroughputPerDay(0), "0 / day" };
                    yield return new object[] { new ThroughputPerDay(1.1), "1.1 / day" };
                    yield return new object[] { new ThroughputPerDay(double.PositiveInfinity), "∞ / day" };
                }
            }

            [Theory]
            [MemberData(nameof(ToStringScenarios))]
            public void WHEN_converting_to_string_THEN_string_is_as_expected(
                ThroughputPerDay a, string expectedString)
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
                string actualString = a.ToString();
                Assert.Equal(expectedString, actualString);
            }
        }

        public class GIVEN_some_throughput_AND_another_throughput_with_the_same_value
        {
            private readonly ThroughputPerDay someThroughput;
            private readonly ThroughputPerDay anotherThroughputWithSameValue;
            private readonly object anotherThroughputBoxedAsObject;

            public GIVEN_some_throughput_AND_another_throughput_with_the_same_value()
            {
                const double someThroughputValue = 5.0;
                someThroughput = new ThroughputPerDay(someThroughputValue);
                anotherThroughputWithSameValue = new ThroughputPerDay(someThroughputValue);
                anotherThroughputBoxedAsObject = (object)anotherThroughputWithSameValue;
            }

            [Fact]
            public void WHEN_compared_to_each_other_THEN_it_is_considered_equal()
            {
                AssertThroughputEqualsReturnsTrueCommutatively(someThroughput, anotherThroughputWithSameValue);

                AssertObjectEqualsReturnsTrueCommutatively(someThroughput, anotherThroughputBoxedAsObject);
            }

            [Fact]
            public void WHEN_comparing_hash_codes_THEN_hash_codes_are_equal()
            {
                AssertHashCodesAreEqual(someThroughput, anotherThroughputWithSameValue);
                AssertHashCodesAreEqual(someThroughput, anotherThroughputBoxedAsObject);
            }

            private static void AssertHashCodesAreEqual<T1, T2>(T1 a, T2 b)
            {
                Assert.Equal(a.GetHashCode(), b.GetHashCode());
            }

            private static void AssertThroughputEqualsReturnsTrueCommutatively(ThroughputPerDay a, ThroughputPerDay b)
            {
                AssertThroughputEqualsReturnsTrue(a, b);
                AssertThroughputEqualsReturnsTrue(b, a);
            }

            private static void AssertThroughputEqualsReturnsTrue(ThroughputPerDay a, ThroughputPerDay b) =>
                AssertThroughputEqualsReturnsExpectedValue(a, b, true);

            private static void AssertObjectEqualsReturnsTrueCommutatively(object a, object b)
            {
                AssertObjectEqualsReturnsTrue(a, b);
                AssertObjectEqualsReturnsTrue(b, a);
            }

            private static void AssertObjectEqualsReturnsTrue(object a, object b) =>
                AssertObjectEqualsReturnsExpectedValue(a, b, true);
        }

        public class GIVEN_some_throughput_AND_another_throughput_with_a_different_value
        {
            private readonly ThroughputPerDay someThroughput;
            private readonly ThroughputPerDay anotherThroughputWithDifferentValue;

            public GIVEN_some_throughput_AND_another_throughput_with_a_different_value()
            {
                someThroughput = new ThroughputPerDay(5.0);
                anotherThroughputWithDifferentValue = new ThroughputPerDay(6.0);
            }

            [Fact]
            public void WHEN_compared_to_each_other_THEN_it_is_considered_different()
            {
                var anotherThroughputBoxedAsObject = (object)anotherThroughputWithDifferentValue;

                AssertThroughputEqualsReturnsFalseCommutatively(someThroughput, anotherThroughputWithDifferentValue);

                AssertObjectEqualsReturnsFalseCommutatively(someThroughput, anotherThroughputBoxedAsObject);
            }

            public static IEnumerable<object[]> AdditionScenarios
            {
                get
                {
                    yield return new object[] { new ThroughputPerDay(0), new ThroughputPerDay(0), new ThroughputPerDay(0) };
                    yield return new object[] { new ThroughputPerDay(1), new ThroughputPerDay(2), new ThroughputPerDay(3) };
                    yield return new object[] { new ThroughputPerDay(2), new ThroughputPerDay(1), new ThroughputPerDay(3) };
                    yield return new object[] { new ThroughputPerDay(double.MaxValue), new ThroughputPerDay(double.MaxValue), new ThroughputPerDay(double.PositiveInfinity) };
                    yield return new object[] { new ThroughputPerDay(double.MaxValue), new ThroughputPerDay(double.Epsilon), new ThroughputPerDay(double.MaxValue) };
                    yield return new object[] { new ThroughputPerDay(double.PositiveInfinity), new ThroughputPerDay(double.Epsilon), new ThroughputPerDay(double.PositiveInfinity) };
                    yield return new object[] { new ThroughputPerDay(double.PositiveInfinity), new ThroughputPerDay(double.MaxValue), new ThroughputPerDay(double.PositiveInfinity) };
                    yield return new object[] { new ThroughputPerDay(double.PositiveInfinity), new ThroughputPerDay(double.PositiveInfinity), new ThroughputPerDay(double.PositiveInfinity) };
                }
            }

            [Theory]
            [MemberData(nameof(AdditionScenarios))]
            public void WHEN_adding_two_throughputs_THEN_the_sum_of_both_is_returned(
                ThroughputPerDay a, ThroughputPerDay b, ThroughputPerDay expectedResult)
            {
                var actualSum = a + b;

                Assert.Equal(expectedResult, actualSum);
            }

            public static IEnumerable<object[]> DivisionScenarios
            {
                get
                {
                    yield return new object[] { new ThroughputPerDay(1), 1.0, new ThroughputPerDay(1) };
                    yield return new object[] { new ThroughputPerDay(0), 1.0, new ThroughputPerDay(0) };
                    yield return new object[] { new ThroughputPerDay(5), 5.0, new ThroughputPerDay(1) };
                    yield return new object[] { new ThroughputPerDay(double.MaxValue), double.MaxValue, new ThroughputPerDay(1) };
                    yield return new object[] { new ThroughputPerDay(5), double.PositiveInfinity, new ThroughputPerDay(0) };
                    yield return new object[] { new ThroughputPerDay(5), 0.0, new ThroughputPerDay(double.PositiveInfinity) };
                }
            }

            [Theory]
            [MemberData(nameof(DivisionScenarios))]
            public void WHEN_dividing_two_throughputs_THEN_the_result_is_as_expected(
                ThroughputPerDay numerator, double denominator, ThroughputPerDay expectedResult)
            {
                ThroughputPerDay actualResult = numerator / denominator;

                Assert.Equal(expectedResult, actualResult);
            }

            [Fact]
            public void WHEN_dividing_an_infinite_throughput_by_infinite_THEN_ArithmeticException_is_thrown()
            {
                Action call = () => _ = new ThroughputPerDay(double.PositiveInfinity) / double.PositiveInfinity;
                var actualException = Assert.Throws<ArithmeticException>(call);
                Assert.Equal("Cannot divide an infinite throughput by infinite, as the result is indeterminate.", actualException.Message);
            }

            private static void AssertThroughputEqualsReturnsFalseCommutatively(ThroughputPerDay a, ThroughputPerDay b)
            {
                AssertThroughputEqualsReturnsFalse(a, b);
                AssertThroughputEqualsReturnsFalse(b, a);
            }

            private static void AssertThroughputEqualsReturnsFalse(ThroughputPerDay a, ThroughputPerDay b) =>
                AssertThroughputEqualsReturnsExpectedValue(a, b, false);
        }

        private static void AssertThroughputEqualsReturnsExpectedValue(ThroughputPerDay a, ThroughputPerDay b, bool expectedReturnValue) =>
            Assert.Equal(expectedReturnValue, a.Equals(b));

        private static void AssertObjectEqualsReturnsFalseCommutatively(object a, object b)
        {
            AssertObjectEqualsReturnsFalse(a, b);
            AssertObjectEqualsReturnsFalse(b, a);
        }

        private static void AssertObjectEqualsReturnsFalse(object a, object b) =>
            AssertObjectEqualsReturnsExpectedValue(a, b, false);

        private static void AssertObjectEqualsReturnsExpectedValue(object a, object b, bool expectedReturnValue) =>
            Assert.Equal(expectedReturnValue, a.Equals(b));
    }
}
