﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Collections
{
    [TestFixture]
    public class StringKeyCaseInsensitiveDictionaryTests
    {
        private const string _myKey = "MyKey";
        private const string _myValue = "MyValue";

        [Test]
        public void Add_AddItem_ItemIsAdded()
        {
            var dic = new StringKeyCaseInsensitiveDictionary<string>();
            dic.Count.Should().Be(0);
            dic.Add(_myKey, _myValue);
            dic.Count.Should().Be(1);
        }

        [Test]
        public void Get_GetValue_ValueIsRetrived()
        {
            var dic = new StringKeyCaseInsensitiveDictionary<string>();
            dic.Count.Should().Be(0);
            dic.Add(_myKey, _myValue);
            dic[_myKey].Should().Be(_myValue);
            dic[_myKey.ToLower()].Should().Be(_myValue);
            dic[_myKey.ToUpper()].Should().Be(_myValue);
        }

        [Test]
        public void Get_KeyDoesntExist_ExceptionIsThrown()
        {
            var dic = new StringKeyCaseInsensitiveDictionary<string>();
            Action action = () => { var temp = dic[_myKey]; };

            action.ShouldThrow<KeyNotFoundException>();
        }


        [Test]
        public void Set_SetValue_ValueIsSet()
        {
            var dic = new StringKeyCaseInsensitiveDictionary<string>();
            dic.Count.Should().Be(0);
            dic.Add(_myKey, _myValue);
            dic[_myKey].Should().Be(_myValue);
            dic[_myKey.ToLower()].Should().Be(_myValue);
            dic[_myKey.ToUpper()].Should().Be(_myValue);

            var myNewValue = _myValue + "12313213";
            dic[_myKey.ToUpper()] = myNewValue;

            dic[_myKey].Should().Be(myNewValue);
            dic[_myKey.ToLower()].Should().Be(myNewValue);
            dic[_myKey.ToUpper()].Should().Be(myNewValue);
        }
    }
}
