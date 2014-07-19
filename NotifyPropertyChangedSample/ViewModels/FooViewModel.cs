namespace NotifyPropertyChangedSample.ViewModels
{
    public class FooViewModel : ViewModel
    {
        public int HowManyFoos { get; protected set; }

        public void DoFoo()
        {
            HowManyFoos++;
        }
    }
}