using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using AINotesHub.WPF.Models;
using Timer = System.Timers.Timer;

namespace AINotesHub.WPF.Services
{
    public class ReminderScheduler
    {
        private Timer _timer;

        public List<Note> Notes { get; set; } = new();

        //public void Start()
        //{
        //    _timer = new Timer(60000);
        //    _timer.Elapsed += Check;
        //    _timer.Start();
        //}

        private void Check(object? s, ElapsedEventArgs e)
        {
            foreach (var n in Notes)
            {
                if (n.ReminderTime != null &&
                    DateTime.Now >= n.ReminderTime)
                {
                    MessageBox.Show(n.Title, "⏰ Reminder");

                    n.ReminderTime = null; // mark done
                }
            }
        }
    }
}
