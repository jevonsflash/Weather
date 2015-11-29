using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Weather.App.ViewModel
{
    public class ClockViewModel : ViewModelBase
    {
        private DispatcherTimer _timer;

        private string time;

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChanged("Time");

            }

        }

        public ClockViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick; 
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            DateTime now = DateTime.Now;
            Time = now.ToString("HH:mm");
        }
    }
}
