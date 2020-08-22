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
using System.Globalization;
using Xunit;
using KanbanProjectManagementApp.Domain;

namespace KanbanProjectManagementApp.Tests
{
    public class ThroughputPerDayValueConverter_specification : IDisposable
    {
        private readonly ThroughputPerDayValueConverter converter;
        private static readonly CultureInfo culture = new CultureInfo("en-US", false);
        private readonly CultureInfo? originalDefaultThreadCulture;

        public ThroughputPerDayValueConverter_specification()
        {
            originalDefaultThreadCulture = CultureInfo.DefaultThreadCurrentCulture;

            converter = new ThroughputPerDayValueConverter();
        }

        public void Dispose()
        {
            CultureInfo.DefaultThreadCurrentCulture = originalDefaultThreadCulture;
        }

        [Fact]
        public void GIVEN_null_value_WHEN_converting_to_throughput_per_day_THEN_throw_ArgumentNullException()
        {
            object value = null;
            void call() => ConvertToThroughputPerDay(value);
            var actualException = Assert.Throws<ArgumentNullException>(call);
            Assert.Equal("value", actualException.ParamName);
        }

        public static IEnumerable<object[]> NonNumberStringScenarios
        {
            get
            {
                yield return new object[] { string.Empty };
                yield return new object[] { " " };
                yield return new object[] { "a" };
            }
        }

        [Theory]
        [MemberData(nameof(NonNumberStringScenarios))]
        public void GIVEN_empty_string_WHEN_converting_to_throughput_per_day_THEN_throw_FormatException(
            string text)
        {
            void call() => ConvertToThroughputPerDay(text);
            var actualException = Assert.Throws<FormatException>(call);
            Assert.Equal("Value should represent a number.", actualException.Message);
        }

        public static IEnumerable<object[]> InvalidThroughputNumberAsStringScenarios
        {
            get
            {
                yield return new object[] { DoubleToCulturedString(-double.Epsilon) };
                yield return new object[] { "-2" };
                yield return new object[] { DoubleToCulturedString(double.MinValue) };
                yield return new object[] { DoubleToCulturedString(double.NaN) };
                yield return new object[] { DoubleToCulturedString(double.NegativeInfinity) };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidThroughputNumberAsStringScenarios))]
        public void GIVEN_invalid_throughput_values_WHEN_converting_to_throughput_per_day_THEN_throw_ArgumentException(
            string text)
        {
            void call() => ConvertToThroughputPerDay(text);
            var actualException = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.Equal("numberOfWorkItemsFinished", actualException.ParamName);
        }

        public static IEnumerable<object[]> ValidThroughputNumberAsStringScenarios
        {
            get
            {
                static object[] createInputAndExpectedResultPair(double value) =>
                    new object[] { DoubleToCulturedString(value), new ThroughputPerDay(value) };

                yield return createInputAndExpectedResultPair(0.0);
                yield return createInputAndExpectedResultPair(double.Epsilon);
                yield return createInputAndExpectedResultPair(3.4);
                yield return createInputAndExpectedResultPair(double.MaxValue);
                yield return createInputAndExpectedResultPair(double.PositiveInfinity);
            }
        }

        [Theory]
        [MemberData(nameof(ValidThroughputNumberAsStringScenarios))]
        public void GIVEN_valid_throughput_values_WHEN_converting_to_throughput_per_day_THEN_return_throughput(
            string text, ThroughputPerDay expectedValue)
        {
            object result = ConvertToThroughputPerDay(text);
            Assert.Equal(expectedValue, result);
        }

        public static IEnumerable<object[]> ThroughputValueToStringScenarios
        {
            get
            {
                static object[] createInputAndExpectedResultPair(double value) =>
                    new object[] { new ThroughputPerDay(value), $"{DoubleToCulturedString(value)} / day" };

                yield return createInputAndExpectedResultPair(3.4);
                yield return createInputAndExpectedResultPair(double.PositiveInfinity);
            }
        }

        [Theory]
        [MemberData(nameof(ThroughputValueToStringScenarios))]
        public void GIVEN_some_throughput_value_WHEN_converting_to_string_THEN_return_string_representation(
            ThroughputPerDay inputThroughput, string expectedResult)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;

            object result = ConvertToString(inputThroughput);
            Assert.Equal(expectedResult, result);
        }

        private object ConvertToString(object value) =>
            converter.Convert(value, typeof(string), null, culture);

        private object ConvertToThroughputPerDay(object value) =>
            converter.ConvertBack(value, typeof(ThroughputPerDay), null, culture);

        private static string DoubleToCulturedString(double value) =>
            value.ToString(culture);
    }
}
