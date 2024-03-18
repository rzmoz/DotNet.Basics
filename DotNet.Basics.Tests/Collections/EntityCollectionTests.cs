#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class EntityCollectionTests
    {
        [Fact]
        public void Ctor_OverrideOrderBy_ItemsAreSortedByCustom()
        {
            var keys = Enumerable.Range(1, 10).Select(i => $"key{i}").ToList();//range high to low
            IEnumerable<Entity> Sorting(IEnumerable<Entity> items) => items.OrderByDescending(i => i.SortOrder);//reverse sorting
            var entList = new EntityCollection(orderBy: Sorting);

            //act
            foreach (var key in keys)
                entList.Add(new Entity { Key = key });

            //assert
            var first = entList.First();
            var last = entList.Last();
            string.Compare(keys.First(), keys.Last(), StringComparison.Ordinal).Should().BeLessThan(0);//first key sorts lower than last => keys are sorted in ascending order
            first.Key.Should().Be("key10");//keys are sorted respecting sort order
            last.Key.Should().Be("key1");
        }

        [Fact]
        public void Add_SortOrder_ItemsAreAddedLastRespectingSortOrder()
        {
            var entities = Enumerable.Range(1, 10).Reverse().Select(index => new Entity { Key = $"key{index}" }).ToArray();//range high to low

            //act
            var entList = new EntityCollection { entities };

            //assert
            var first = entList.First();
            var last = entList.Last();
            first.Key.Should().Be("key10");//keys are sorted respecting sort order
            last.Key.Should().Be("key1");
        }

        [Fact]
        public void Get_SimpleGet_EntryIsRetrieved()
        {
            var keys = Enumerable.Range(1, 10).Select(i => $"key{i}");
            var entList = new EntityCollection();
            foreach (var key in keys)
            {
                entList[key] = new Entity { Key = key };
            }
            var item5 = entList["key5"];

            item5.Key.Should().Be("key5");
        }


        [Fact]
        public void Index_ByIndex_EntryIsRetrieved()
        {
            var keys = Enumerable.Range(0, 9).Select(i => $"key{i}");
            var entList = new EntityCollection();
            foreach (var key in keys)
                entList[key] = new Entity { Key = key };

            //act
            var item5 = entList[5];//get by index

            item5.Key.Should().Be("key5");
        }

        [Fact]
        public void Sort_DefaultSorting_EntitiesItemsAreSortedBySortOrder()
        {
            var entList = new EntityCollection();

            var entLast = new Entity
            {
                Key = "key1",
                SortOrder = 500
            };
            var entMiddle = new Entity
            {
                Key = "key500",
                SortOrder = 150
            };
            var entFirst = new Entity
            {
                Key = "key250",
                SortOrder = 0
            };

            entList.Add(entLast, entFirst, entMiddle);

            entList.First().Should().Be(entFirst);
            entList.Skip(1).Take(1).Single().Should().Be(entMiddle);
            entList.Last().Should().Be(entLast);

            entList.Clear();
            entList.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("my-key", true)]
        [InlineData("my-KEY", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void ContainsKey_CheckForKey_KeyIsDetermined(string? key, bool expected)
        {
            var entList = new EntityCollection { new Entity { Key = "my-key" } };
            entList.ContainsKey(key).Should().Be(expected);
        }

        [Fact]
        public void Clear_Clear_DictionaryIsCleared()
        {
            var entList = new EntityCollection { new Entity() };

            entList.Count.Should().BeGreaterThan(0);

            entList.Clear();
            entList.Count.Should().Be(0);
        }
    }
}
