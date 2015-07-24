using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;

namespace SwipeProject.Universal.Sample.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            DataSource = new ObservableCollection<int>(Enumerable.Range(1, 25));
            ClickCommand = new RelayCommand<int>(ClickExecute);
        }

        private void ClickExecute(int item)
        {
            DataSource.Remove(item);
        }

        public ObservableCollection<int> DataSource { get; set; }

        public RelayCommand<int> ClickCommand { get; set; }
    }
}