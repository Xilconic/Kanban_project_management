//  Copyright (c) 2020 S.L. des Bouvrie
// 
//  This file is part of 'Kanban Project Management App'.
// 
//  Kanban Project Management App is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Kanban Project Management App is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with Kanban Project Management App.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Globalization;
using KanbanProjectManagementApp.Application.DataFormatting;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit.Application
{
    public class DataViewFactory_specification
    {
        private readonly DataViewFactory factory = new DataViewFactory();

        [Fact]
        public void GIVEN_time_till_completion_estimations_null_WHEN_creating_data_view_THEN_ThrowArgumentException()
        {
            void Call() => factory.FromTimeTillCompletionEstimations(null, default, CultureInfo.InvariantCulture);

            Assert.Throws<ArgumentNullException>("estimations", Call);
        }
    }
}