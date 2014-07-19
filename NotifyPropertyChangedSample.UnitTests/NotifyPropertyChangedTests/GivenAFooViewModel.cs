using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NotifyPropertyChangedSample.ViewModels;
using NUnit.Framework;
using Shouldly;

namespace NotifyPropertyChangedSample.UnitTests.NotifyPropertyChangedTests
{
    public abstract class GivenAFooViewModel : Test
    {
        private FooViewModel _viewModel;
        private List<Tuple<object, PropertyChangedEventArgs>> _propertyChangedEvents;

        protected override void Given()
        {
            _propertyChangedEvents = new List<Tuple<object, PropertyChangedEventArgs>>();

            _viewModel = new FooViewModel();

            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _propertyChangedEvents.Add(new Tuple<object, PropertyChangedEventArgs>(sender, propertyChangedEventArgs));
        }

        public class WhenDoingFoo : GivenAFooViewModel
        {
            protected override void When()
            {
                _viewModel.DoFoo();
            }

            [Test]
            public void ThereShouldBeAtLeastOnePropertyChangedEvent()
            {
                _propertyChangedEvents.Count.ShouldBeGreaterThanOrEqualTo(1);
            }

            [Test]
            public void ThereShouldBeAnEventFromOurViewModelWithTheCorrectPropertyName()
            {
                _propertyChangedEvents
                    .Where(tuple => tuple.Item1 == _viewModel)
                    .Where(tuple => string.CompareOrdinal(tuple.Item2.PropertyName, "HowManyFoos") == 0)
                    .Count()
                    .ShouldBe(1);
            }
        }
    }
}