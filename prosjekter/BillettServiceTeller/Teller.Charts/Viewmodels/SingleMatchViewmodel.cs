using System;

namespace Teller.Charts.Viewmodels
{
    public class SingleMatchViewmodel : BaseViewModel
    {
        public string Title { get { return "Rosenborg - molde"; }}

        public SingleMatchViewmodel()
        {
        }

        public void UpdateAll()
        {
            OnPropertyChanged(String.Empty);
        }
    }
}
