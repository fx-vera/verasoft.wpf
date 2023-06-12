using Btl.Builders;
using Btl.Models;
using PropertyChanged;
using System;

namespace MediaManager.Plugins.CountDown
{
    [AddINotifyPropertyChangedInterface]
    public class CountDownViewModel
    {
        readonly ITimerModel _timer;

        public CountDownViewModel()
        {
            LoadSettings();
            _timer = TimerModelBuilder.GetNewTimer(CountDownModel.Duration);
            AddEventHandlers();
            _timer.Start();
        }

        public CountDownModel CountDownModel { get; set; }

        /// <summary>
        /// Update the timer view model properties based on the time span passed in.
        /// </summary>
        /// <param name="t"></param>
        private void UpdateTimerValues()
        {
            int years = _timer.Remaining.Days / 365;
            //int years = CountDownModel.Duration.Days / 365;
            int daysRemain = _timer.Remaining.Days % 365;
            //int daysRemain = CountDownModel.Duration.Days % 365;
            int months = (int)(daysRemain / 31.4);
            daysRemain = (int)(daysRemain % 31.4);

            CountDownModel.TimerValue =
                string.Format("Para el {0} te queda \r\n{1} años\r\n {2} meses\r\n {3} días \r\n [{4} hh : {5} mm : {6} ss]",
                CountDownModel.FinalDate.ToShortDateString(),
                 years.ToString("D2"),
                months.ToString("D2"),
                daysRemain.ToString("D2"),
                _timer.Remaining.Hours.ToString("D2"),
                _timer.Remaining.Minutes.ToString("D2"),
                _timer.Remaining.Seconds.ToString("D2")
                );
        }

        private void LoadSettings()
        {
            CountDownModel = new CountDownModel()
            {
                Description = Settings.Default.Description,
                InitialDate = Settings.Default.InitialDate,
                FinalDate = Settings.Default.FinalDate,
                CurrentDate = DateTime.Now,
                Years = Settings.Default.Years,
                Duration = Settings.Default.Duration
            };

            PrepareSettings();
        }

        private void PrepareSettings()
        {
            CountDownModel.FinalDate = CountDownModel.InitialDate.AddYears(CountDownModel.Years);
            CountDownModel.Duration = new TimeSpan(CountDownModel.FinalDate.Ticks - (DateTime.Now.Ticks));
            //UpdateTimerValues();
        }

        /// <summary>
        /// Add the event handlers.
        /// </summary>
        private void AddEventHandlers()
        {
            _timer.Tick += (sender, e) => OnTick(sender, e);
            //_timer.Completed += (sender, e) => OnCompleted(sender, e);
            _timer.Started += (sender, e) => OnStarted(sender, e);
            //_timer.Stopped += (sender, e) => OnStopped(sender, e);
            //_timer.TimerReset += (sender, e) => OnReset(sender, e);
        }

        /// <summary>
        /// Fires when the timer ticks.  Ticks out to be of the order of 
        /// tenths of a second or so to prevent excessive spamming of this method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);

        }

        /// <summary>
        /// Fires when the timer starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStarted(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);
        }

        /// <summary>
        /// Update the timer view model properties based on the time span passed in.
        /// </summary>
        /// <param name="t"></param>
        private void UpdateTimer(TimerModelEventArgs e)
        {
            UpdateTimerValues();
        }
    }
}
