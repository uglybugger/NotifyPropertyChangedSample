INotifyPropertyChanged Sample
=============================

This is a very quick demo of one way of implementing INPC using only [PostSharp](http://www.postsharp.net/)'s Express (i.e. free) features.

# Why not just use the licensed features?

**I suggest that you *should* use the licensed features.** They're great. That said, they're also not cheap and it's heaps easier (especially if you're a consultant) to persuade a development team to drop in a new tool if they know it's not going to bite them in licence fees in a month's time.

Before you do it this way, have a look at the [published example of how to do INPC using the licensed features](http://www.postsharp.net/aspects/examples/inotifypropertychanged). It's much nicer. And then use the one from the [Model Pattern Library](http://www.postsharp.net/model), which is for-realsies production-ready.

# Key stuff to note
## We use a base ViewModel type

Anything implementing INPC like this needs a RaisePropertyChanged method. This is because PostSharp requires a licence to do aspect inheritance, which is a much nicer way to do stuff. Did I mention the example above?

We don't strictly need to have everything inherit from the one base class; just that everything that's going to have the NotifyPropertyChanged aspect injected into it needs to have a RaisePropertyChanged method and this is an easy way to do it. If you're already inheriting from Caliburn.Micro's Screen or PropertyChangedBase then you might want to change the way you raise the events slightly. That's entirely up to you :)

Regardless, here's how we're doing it in this example:

    public interface IViewModel: INotifyPropertyChanged
    {
    }
    
    public abstract class ViewModel : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null) return;

            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

This lets us write a viewmodel that looks like this:

    public class FooViewModel : ViewModel
    {
        public int HowManyFoos { get; protected set; }

        public void DoFoo()
        {
            HowManyFoos++;
        }
    }
    
## Globally-applied aspects

We're applying aspects using a namespace wildcard rather than on individual viewmodels.

    using NotifyPropertyChangedSample;

    [assembly: NotifyPropertyChanged(AttributeTargetTypes = "*.ViewModels.*")]

Be careful once you have multiple aspects in a project. Injecting aspects into aspects is not a good idea and will go bang.

## Convention tests to assert that aspects will be applied correctly

We're also using NUnit-based convention tests to assert that all of our viewmodels are in namespaces that will have the relevant aspect applied to them:

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

## Our aspect will exit early if the new value is the same as the old value

    public override void OnSetValue(LocationInterceptionArgs args)
    {
        // ...

        var oldValue = args.GetCurrentValue();
        var newValue = args.Value;
        if (Equals(oldValue, newValue)) return; // NOTE: bail early if the values are the same

        /// ...
    }

This is by design. PropertyChanged is supposed to fire when a property has changed, which means *its value is now different*.

    Bert: My favourite colour has changed.
    Ernie: What was it before?
    Bert: Blue.
    Ernie: Okay. What is it now?
    Bert: Blue.
    Ernie: @#$^#^*&

If you're relying on PropertyChanged events to fire when you set a property to the same value as it had before then you have a bug :)

# What about derived/computed properties

Nope. Sorry; this example doesn't do that. If you want them, you could either roll your own or just go and buy a licence and use the grown-up version.

# But my way is better!

Then, by all means, use your way :)
