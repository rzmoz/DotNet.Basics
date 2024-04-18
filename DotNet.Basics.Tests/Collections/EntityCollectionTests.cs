#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class EntityCollectionTests
    {
        [Fact]
        public void CopyTo_Copy_EntitiesAreCopied()
        {
            //arrange
            var col = new EntityCollection { Enumerable.Range(1, 10).Select(i => new Entity
            {
                DisplayName = i.ToString()
            }).ToArray()};
            var destinationArray = new Entity[col.Count];

            //act
            col.CopyTo(destinationArray, 0);


            //assert
            for (var i = 0; i < col.Count; i++)//iterate on source array
                destinationArray[i].Key.Should().Be(col[i].Key);
        }
        [Fact]
        public void Anonymous_OotbSerialization_JsonSerializationWorks()
        {
            //arrange
            var col = new EntityCollection { Enumerable.Range(1, 10).Select(i => new Entity
            {
                DisplayName = i.ToString()
            }).ToArray()};

            //act
            var json = col.ToJson();
            var deSerCol = json.FromJson<EntityCollection>();

            //assert
            deSerCol.Count.Should().Be(10);
            deSerCol.First().Key.Should().Be("1");
            deSerCol.Last().Key.Should().Be("10");
        }
        [Fact]
        public void Generic_OotbSerialization_JsonSerializationWorks()
        {
            //arrange
            var col = new EntityCollection<Entity> { Enumerable.Range(1, 10).Select(i => new Entity
            {
                DisplayName = i.ToString()
            }).ToArray()};

            //act
            var json = col.ToJson();
            var deSerCol = json.FromJson<EntityCollection<Entity>>();

            //assert
            deSerCol.Count.Should().Be(10);
            deSerCol.First().Key.Should().Be("1");
            deSerCol.Last().Key.Should().Be("10");
        }
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
        public void Add_KeyExists_ExceptionIsThrown()
        {
            var entity = new Entity
            {
                Key = "my-key"
            };
            var entList = new EntityCollection
            {
                entity
            };

            //act
            Action add = () => entList.Add(entity);

            //assert
            add.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void Add_KeyExists_EntityIsUpdated()
        {
            var key = "my-key";
            var displayname1 = "11111";
            var displayname2 = "22222";

            var entity = new Entity
            {
                DisplayName = displayname1,
                Key = key,
            };
            var entList = new EntityCollection(addKeyExists: KeyExists.Update)
            {
                entity
            };

            entList.Single().DisplayName.Should().Be(displayname1);

            //act

            Action add = () => entList.Add(new Entity
            {
                DisplayName = displayname2,
                Key = key
            });

            //assert
            add.Should().NotThrow();
            entList.Count.Should().Be(1);
            entList.Single().DisplayName.Should().Be(displayname2);
        }

        [Fact]
        public void Add_SortOrder_SortOrderIsOverridenWithAddedOrder()
        {
            var entities = Enumerable.Range(1, 10).Reverse().Select(index => new Entity { SortOrder = index, Key = index.ToString() }).ToArray();//range high to low
            var additionalEntities = Enumerable.Range(11, 20).Reverse().Select(index => new Entity { SortOrder = index, Key = index.ToString() }).ToArray();//range high to low

            //act
            var entList = new EntityCollection
            {
                entities,
                additionalEntities
            };

            //assert
            var first = entList.First();
            var last = entList.Last();
            first.Key.Should().Be("10");//keys are sorted respecting add order
            first.SortOrder.Should().Be(0);
            last.Key.Should().Be("11");
            last.SortOrder.Should().Be(entList.Count - 1);
        }


        [Fact]
        public void Ctor_SortOrder_SortOrderIsOverridenWithAddedOrder()
        {
            var entities = Enumerable.Range(1, 10).Reverse().Select(index => new Entity { SortOrder = index, Key = index.ToString() }).ToArray();//range high to low

            //act
            var entList = new EntityCollection(entities);

            //assert
            var first = entList.First();
            var last = entList.Last();
            first.Key.Should().Be("10");//keys are sorted respecting add order
            first.SortOrder.Should().Be(0);
            last.Key.Should().Be("1");
            last.SortOrder.Should().Be(entList.Count - 1);
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
        public void Set_RespectSortOrder_ExistingSortOrderIsKept()
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

            //act
            entList.Set(entLast, entFirst, entMiddle);

            //assert
            entList.First().Should().Be(entFirst);
            entList.Skip(1).Take(1).Single().Should().Be(entMiddle);
            entList.Last().Should().Be(entLast);

            entList.Clear();
            entList.Count.Should().Be(0);
        }
        [Fact]
        public void Sort_SortCollection_CollectionIsSorted()
        {
            var ent1 = new Entity
            {
                Key = "1"
            };
            var ent2 = new Entity
            {
                Key = "2"
            };

            var entList = new EntityCollection
            {
                ent1,
                ent2
            };

            entList.First().Key.Should().Be("1");
            entList.Last().Key.Should().Be("2");

            //act
            entList.Sort(ents => ents.OrderByDescending(e => e.Key));

            //assert
            entList.First().Key.Should().Be("2");
            entList.Last().Key.Should().Be("1");
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
