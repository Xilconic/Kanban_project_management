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
using KanbanProjectManagementApp.TextFileProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit
{
    public class InputMetricsCsvReader_specification
    {
        [Fact]
        public void WHEN_constructing_a_new_instance_AND_TextReader_is_null_THEN_throw_ArgumentNullException()
        {
            static void call() => InputMetricsCsvReader.Read(null);

            Assert.Throws<ArgumentNullException>("textReader", call);
        }

        [Fact]
        public void GIVEN_text_reader_returning_empty_string_WHEN_reading_THEN_return_empty_collection()
        {
            using var emptyStringReader = new StringReader(string.Empty);
            IReadOnlyCollection<InputMetric> inputMetrics = InputMetricsCsvReader.Read(emptyStringReader);
            Assert.Empty(inputMetrics);
        }

        [Fact]
        public void GIVEN_text_reader_returning_only_some_header_WHEN_reading_THEN_throw_FailedToReadInputMetricsException()
        {
            using var stringReader = new StringReader("someHeader");

            void call() => InputMetricsCsvReader.Read(stringReader);

            var actualException = Assert.Throws<FailedToReadInputMetricsException>(call);
            Assert.Equal("Invalid header. It must contain a column with the name 'NumberOfCompletedWorkItems' and use ';' as delimiter.", actualException.Message);
        }

        [Fact]
        public void GIVEN_text_reader_returning_only_header_with_throughput_column_WHEN_reading_THEN_return_empty_collection()
        {
            using var stringReader = new StringReader("NumberOfCompletedWorkItems");

            IReadOnlyCollection<InputMetric> inputMetrics = InputMetricsCsvReader.Read(stringReader);

            Assert.Empty(inputMetrics);
        }

        [Fact]
        public void GIVEN_text_reader_returning_data_without_throughput_column_WHEN_reading_THEN_throw_FailedToReadInputMetricsException()
        {
            string fileContents =
@"Date;Number
08/19/2020 00:00:00;4
08/18/2020 00:00:00;1
08/17/2020 00:00:00;4
";
            using var stringReader = new StringReader(fileContents);

            void call() => InputMetricsCsvReader.Read(stringReader);

            var actualException = Assert.Throws<FailedToReadInputMetricsException>(call);
            Assert.Equal("Invalid header. It must contain a column with the name 'NumberOfCompletedWorkItems' and use ';' as delimiter.", actualException.Message);
        }

        [Fact]
        public void GIVEN_text_reader_returning_data_with_throughput_column_AND_comma_as_delimiter_WHEN_reading_THEN_return_input_metrics_with_throughput_data()
        {
            string fileContents =
@"Date,NumberOfCompletedWorkItems
08/19/2020 00:00:00,4
08/18/2020 00:00:00,1
08/17/2020 00:00:00,3
";
            using var emptyStringReader = new StringReader(fileContents);

            void call() => InputMetricsCsvReader.Read(emptyStringReader);

            var actualException = Assert.Throws<FailedToReadInputMetricsException>(call);
            Assert.Equal("Invalid header. It must contain a column with the name 'NumberOfCompletedWorkItems' and use ';' as delimiter.", actualException.Message);
        }

        [Fact]
        public void GIVEN_text_reader_returning_data_with_throughput_column_WHEN_reading_THEN_return_input_metrics_with_throughput_data()
        {
            string fileContents =
@"Date;NumberOfCompletedWorkItems
08/19/2020 00:00:00;4
08/18/2020 00:00:00;1
08/17/2020 00:00:00;3
";
            using var emptyStringReader = new StringReader(fileContents);

            IReadOnlyCollection<InputMetric> inputMetrics = InputMetricsCsvReader.Read(emptyStringReader);

            var expectedInputMetrics = new[]
            {
                new InputMetric { Throughput = new ThroughputPerDay(4) },
                new InputMetric { Throughput = new ThroughputPerDay(1) },
                new InputMetric { Throughput = new ThroughputPerDay(3) },
            };
            Assert.Equal(inputMetrics, expectedInputMetrics, new InputMetricEqualtyComparer());
        }

        [Fact]
        public void GIVEN_text_reader_returning_data_with_throughput_column_AND_corrupt_throughput_data_WHEN_reading_THEN_throw_FailedToReadInputMetricsException()
        {
            string fileContents =
@"Date;NumberOfCompletedWorkItems
08/19/2020 00:00:00;4
08/18/2020 00:00:00;bug
08/17/2020 00:00:00;3
";
            using var emptyStringReader = new StringReader(fileContents);

            void call() => InputMetricsCsvReader.Read(emptyStringReader);

            var actualException = Assert.Throws<FailedToReadInputMetricsException>(call);
            Assert.Equal("Failed to parse a value in the 'NumberOfCompletedWorkItems' column. All elements must be a number.", actualException.Message);
        }

        [Fact]
        public void GIVEN_text_reader_returning_data_with_throughput_column_AND_missing_some_throughput_data_WHEN_reading_THEN_return_input_metrics_with_throughput_data()
        {
            string fileContents =
@"Date;NumberOfCompletedWorkItems
08/19/2020 00:00:00;4
08/18/2020 00:00:00;
08/17/2020 00:00:00;3
";
            using var emptyStringReader = new StringReader(fileContents);

            void call() => InputMetricsCsvReader.Read(emptyStringReader);

            var actualException = Assert.Throws<FailedToReadInputMetricsException>(call);
            Assert.Equal("Failed to parse a value in the 'NumberOfCompletedWorkItems' column. All elements must be a number.", actualException.Message);
        }

        private class InputMetricEqualtyComparer : IEqualityComparer<InputMetric>
        {
            public bool Equals([AllowNull] InputMetric x, [AllowNull] InputMetric y)
            {
                return x.Throughput.Equals(y.Throughput);
            }

            public int GetHashCode([DisallowNull] InputMetric obj)
            {
                return obj.Throughput.GetHashCode();
            }
        }
    }
}
