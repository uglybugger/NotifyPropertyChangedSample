using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace NotifyPropertyChangedSample.UnitTests.Conventions
{
    [TestFixture]
    public class AllTypesImplementingIViewModel
    {
        [Test]
        [TestCaseSource(typeof (TestCases))]
        public void ShouldBeInANamespaceThatWillBeDiscoveredByPostSharp(Type viewModelType)
        {
            viewModelType.Namespace.ShouldEndWith(".ViewModels");
        }

        public class TestCases : IEnumerable<TestCaseData>
        {
            public IEnumerator<TestCaseData> GetEnumerator()
            {
                return typeof (IViewModel).Assembly
                                          .DefinedTypes
                                          .Where(t => !t.IsInterface)
                                          .Where(t => !t.IsAbstract)
                                          .Where(t => typeof (IViewModel).IsAssignableFrom(t))
                                          .Select(t => new TestCaseData(t)
                                                           .SetName(t.FullName))
                                          .OrderBy(tc => tc.TestName)
                                          .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}