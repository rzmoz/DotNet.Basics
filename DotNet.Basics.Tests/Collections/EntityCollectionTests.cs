using System;
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
        public void Add_SortOrder_ItemsAreAddedLastRespectingSortOrder()
        {
            var keys = Enumerable.Range(1, 10).Select(i => $"key{i}");//range high to low
            var entList = new EntityCollection();

            //act
            foreach (var key in keys.Reverse())
                entList.Add(new Entity(e => key));

            //assert
            var first = entList.First();
            var last = entList.Last();
            string.Compare(keys.First(), keys.Last(), StringComparison.Ordinal).Should().BeLessThan(0);//first key sorts lower than last => keys are sorted in ascending order
            first.GetKey().Should().Be("key10");//keys are sorted respecting sort order
            last.GetKey().Should().Be("key1");
        }

        [Fact]
        public void Get_SimpleGet_EntryIsRetrieved()
        {
            var keys = Enumerable.Range(1, 10).Select(i => $"key{i}");
            var entList = new EntityCollection();
            foreach (var key in keys)
            {
                entList[key] = new Entity(e => key);
            }
            var item5 = entList["key5"];

            item5.GetKey().Should().Be("key5");
        }


        [Fact]
        public void Sort_Enumerator_ItemsAreSortedByCompareTo()
        {
            var entList = new EntityCollection();

            var entLast = new Entity(e => "key1")
            {
                SortOrder = 500
            };
            var entMiddle = new Entity(e => "key500")
            {
                SortOrder = 150
            };
            var entFirst = new Entity(e => "key150")
            {
                SortOrder = 0
            };

            entList.Add(entLast, entFirst, entMiddle);

            entList.First().Should().Be(entFirst);
            entList.Skip(1).Take(1).Single().Should().Be(entMiddle);
            entList.Last().Should().Be(entLast);

            entList.Clear();
            entList.Count.Should().Be(0);
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
