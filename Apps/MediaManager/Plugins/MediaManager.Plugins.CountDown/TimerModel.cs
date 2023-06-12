﻿// Copyright 2012 lapthorn.net.
//
// This software is provided "as is" without a warranty of any kind. All
// express or implied conditions, representations and warranties, including
// any implied warranty of merchantability, fitness for a particular purpose
// or non-infringement, are hereby excluded. lapthorn.net and its licensors
// shall not be liable for any damages suffered by licensee as a result of
// using the software. In no event will lapthorn.net be liable for any
// lost revenue, profit or data, or for direct, indirect, special,
// consequential, incidental or punitive damages, however caused and regardless
// of the theory of liability, arising out of the use of or inability to use
// software, even if lapthorn.net has been advised of the possibility of
// such damages.
//
// You are free to fork this via github:  https://github.com/barrylapthorn/countdown_timer

using System;
using System.Windows.Threading;

namespace Btl.Models
{
    internal class TimerModel : ITimerModel
    {
        #region Fields
        /// <summary>
        /// The underlying timer
        /// </summary>
        readonly DispatcherTimer timer = new DispatcherTimer();


        #endregion

        #region Constructors
        /// <summary>
        /// Construct a timer with a default duration of 25 minutes.  This is a 
        /// 'pomodoro' time span.
        /// </summary>
        public TimerModel() : this(new TimeSpan(0, 25, 0))
        {
        }

        /// <summary>
        /// Create a timer of the specified duration.
        /// </summary>
        /// <param name="duration"></param>
        public TimerModel(TimeSpan duration)
        {
            //  use a coarse grained interval, as this timer isn't meant
            //  to be completely accurate.
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => OnDispatcherTimerTick();

            Duration = duration;
            //Reset();// TODO FRAN

        }
        #endregion

        #region Properties
        /// <summary>
        /// Return the timer interval that we're using.
        /// </summary>
        public TimeSpan Interval { get { return timer.Interval; } }

        /// <summary>
        /// Return the time remaining.
        /// </summary>
        public TimeSpan Remaining { get; private set; }

        /// <summary>
        /// Has the time completed?
        /// </summary>
        public bool Complete
        {
            get
            {
                return Remaining <= TimeSpan.Zero;
            }
        }

        private TimeSpan duration;

        /// <summary>
        /// Set or get the duration.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    Reset();
                }
            }
        }

        public TimerState Status { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Start the countdown.
        /// </summary>
        public void Start()
        {
            timer.Start();
            OnStarted();
        }

        /// <summary>
        /// Stop the countdown.
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            OnStopped();
        }

        /// <summary>
        /// Stop the current countdown and reset.
        /// </summary>
        public void Reset()
        {
            Stop();
            Remaining = Duration;
            OnReset();
        }

        #region Event Handlers
        /// <summary>
        /// Handle the ticking of the system timer.
        /// </summary>
        private void OnDispatcherTimerTick()
        {
            Remaining = Remaining - Interval;
            OnTick();
            if (Complete)
            {
                Stop();
                Remaining = TimeSpan.Zero;
                OnCompleted();
            }
        }
        #endregion
        #endregion

        #region Events

        public event EventHandler<TimerModelEventArgs> Tick;
        public event EventHandler<TimerModelEventArgs> Started;
        public event EventHandler<TimerModelEventArgs> Stopped;
        public event EventHandler<TimerModelEventArgs> TimerReset;
        public event EventHandler<TimerModelEventArgs> Completed;


        #region OnReset
        /// <summary>
        /// Triggers the TimerReset event
        /// </summary>
        private void OnReset()
        {
            Status = TimerState.Paused;

            if (TimerReset != null)
            {
                TimerReset(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Reset));
            }
        }
        #endregion

        #region OnCompleted
        /// <summary>
        /// Triggers the Completed event.
        /// </summary>
        private void OnCompleted()
        {
            Status = TimerState.Complete;

            if (Completed != null)
            {
                Completed(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Completed));
            }
        }
        #endregion

        #region OnStopped
        /// <summary>
        /// Triggers the Stopped event.
        /// </summary>
        private void OnStopped()
        {
            Status = TimerState.Paused;

            if (Stopped != null)
            {
                Stopped(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Stopped));
            }
        }
        #endregion

        #region OnStarted
        /// <summary>
        /// Triggers the Started event.
        /// </summary>
        private void OnStarted()
        {
            Status = TimerState.Running;

            if (Started != null)
            {
                Started(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Started));
            }
        }
        #endregion

        #region OnTick
        /// <summary>
        /// Triggers the Tick event.
        /// </summary>
        private void OnTick()
        {
            Status = TimerState.Running;

            if (Tick != null)
            {
                Tick(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Running));
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Trivial enum to describe the timer state.
    /// </summary>
    public enum TimerState
    {
        Running,
        Paused,
        Complete
    }

    /// <summary>
    /// Simple EventArgs class for the timer model.
    /// </summary>
    public class TimerModelEventArgs : EventArgs
    {
        public enum Status
        {
            NotSpecified,
            Stopped,
            Started,
            Running,
            Completed,
            Reset
        }

        private TimeSpan duration;
        private TimeSpan remaining;
        private Status state = Status.NotSpecified;

        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration == value)
                    return;
                duration = value;
            }
        }

        public TimeSpan Remaining
        {
            get
            {
                return remaining;
            }
            set
            {
                if (remaining == value)
                    return;
                remaining = value;
            }
        }

        public Status State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public TimerModelEventArgs(TimeSpan duration, TimeSpan remaining, Status state)
        {
            Duration = duration;
            Remaining = remaining;
            State = state;
        }

    }
}
