using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class EntityDictionaryTests
    {
        [Fact]
        public void Sort_Enumerator_ItemsAreSortedByCompareTo()
        {
            var entList = new EntityDictionary();

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
                Key = "key150",
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
            var entList = new EntityDictionary { new Entity() };

            entList.Count.Should().BeGreaterThan(0);

            entList.Clear();
            entList.Count.Should().Be(0);
        }
    }
}
