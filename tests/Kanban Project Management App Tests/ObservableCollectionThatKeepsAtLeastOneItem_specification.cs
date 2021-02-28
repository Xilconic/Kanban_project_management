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
using System.Collections.Generic;
using KanbanProjectManagementApp.ViewModels;
using Xunit;

namespace KanbanProjectManagementApp.Tests.Unit
{
    public class ObservableCollectionThatKeepsAtLeastOneItem_specification
    {
        [Fact]
        public void GIVEN_source_collection_null_WHEN_constructing_THEN_throw_ArgumentNullException()
        {
            static void Call() => new ObservableCollectionThatKeepsAtLeastOneItem<int>(null, "number");

            Assert.Throws<ArgumentNullException>("collection", Call);
        }

        [Fact]
        public void GIVEN_source_collection_with_2_elements_WHEN_removing_elements_THEN_throw_InvalidOperationException_when_deleting_the_last_one()
        {
            var source = new List<int>
            {
                1,
                2
            };
            const string elementName = "number";
            var collection = new ObservableCollectionThatKeepsAtLeastOneItem<int>(source, elementName);

            var result = collection.Remove(1);
            Assert.True(result);
            Assert.Single(collection);
            
            void Call() => collection.Remove(2);
            var actualException = Assert.Throws<InvalidOperationException>(Call);
            Assert.Equal($"Cannot delete last {elementName}.", actualException.Message);
        }

        [Fact]
        public void GIVEN_source_collection_with_2_elements_WHEN_clearing_THEN_throw_InvalidOperationException_and_collection_remains_unchanged()
        {
            var source = new List<int>
            {
                1,
                2
            };
            const string elementName = "number";
            var collection = new ObservableCollectionThatKeepsAtLeastOneItem<int>(source, elementName);

            void Call() => collection.Clear();
            var actualException = Assert.Throws<InvalidOperationException>(Call);
            Assert.Equal($"Cannot delete last {elementName}.", actualException.Message);
            
            Assert.Equal(2, collection.Count);
        }
    }
}