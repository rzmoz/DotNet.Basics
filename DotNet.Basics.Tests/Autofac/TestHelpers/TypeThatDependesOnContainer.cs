﻿using Autofac;

namespace DotNet.Basics.Tests.AutoFac.TestHelpers
{
    public class TypeThatDependesOnContainer
    {
        private readonly IContainer _container;

        public TypeThatDependesOnContainer(IContainer container)
        {
            _container = container;
        }

        public int Value => _container.Resolve<IMyType>().GetValue();
    }
}