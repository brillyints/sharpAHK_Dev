using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Telerik.WinControls.UI;
using System.Timers;

namespace sharpAHK_Dev
{
    public class _TimerWatch
    {
        // Must Populate label For StopWatch Display
        public static RadLabel label { get; set; }


        Stopwatch stopwatch = new Stopwatch();
        _TelerikLib tel = new _TelerikLib();


        public static System.Timers.Timer _timer { get; set; }

        public static Stopwatch _stopwatch { get; set; }



        public void StartWatch(int WaitMS = 2000)
        {
            stopwatch = new Stopwatch();


            _timer = new System.Timers.Timer(1000); // Set up the timer for X seconds
            _timer.Elapsed += new ElapsedEventHandler(UpdateTimeDisplay);
            _timer.Enabled = true; // Enable it
        }

        public void UpdateTimeDisplay(object sender = null, ElapsedEventArgs e = null)
        {
            if (label != null) { tel.Update(label, "Run Time: " + StopWatchTime(stopwatch, true)); }
        }

        public void StopWatch()
        {
            // Stop timing.
            stopwatch.Stop();
            string line = "Total Time Elapsed: " + StopWatchTime(stopwatch, false);
            _timer.Enabled = false;

            if (label != null) { tel.Update(label, "Total Run Time: " + StopWatchTime(stopwatch, true)); }
        }

        /// <summary>
        /// Converts Time from Stopwatch into HH:MM:SS Format
        /// </summary>
        /// <param name="watch"></param>
        /// <param name="IncludeHours"></param>
        /// <returns></returns>
        public string StopWatchTime(Stopwatch watch, bool IncludeHours = true)
        {
            int hours = watch.Elapsed.Hours;
            string Hours = hours.AddLeadingZeros(2);

            int min = watch.Elapsed.Minutes;
            string Min = min.AddLeadingZeros(2);

            int sec = watch.Elapsed.Seconds;
            string Sec = sec.AddLeadingZeros(2);

            string time = Hours + ":" + Min + ":" + Sec;
            if (!IncludeHours)
            {
                time = Min + ":" + Sec;

                if (Hours != "00") { time = Hours + ":" + Min + ":" + Sec; }
            }

            return time;
        }

    }


}
