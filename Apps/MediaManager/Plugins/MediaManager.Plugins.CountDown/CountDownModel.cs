using PropertyChanged;
using System;

namespace MediaManager.Plugins.CountDown
{
    [AddINotifyPropertyChangedInterface]
    public class CountDownModel
    {
        public CountDownModel()
        {

        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime InitialDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime FinalDate { get; set; }
        public int Years { get; set; }
        public TimeSpan Duration { get; set; }
        public string TimerValue { get; set; }
    }
}