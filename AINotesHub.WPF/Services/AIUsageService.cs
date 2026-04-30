using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace AINotesHub.WPF.Services
{
    public class AIUsageService
    {
        private const int DailyLimit = 5;
        private int _used = 0;

        private DateTime LastUsedDate
        {
            get => Properties.Settings.Default.LastAiDate;
            set
            {
                Properties.Settings.Default.LastAiDate = value;
                Properties.Settings.Default.Save();
            }
        }

        private int TodayCount
        {
            get => Properties.Settings.Default.TodayAiCount;
            set
            {
                Properties.Settings.Default.TodayAiCount = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool CanUseAI()
        {
            ResetIfNewDay();

            return TodayCount < DailyLimit;
        }

        public bool CanUse() => _used < DailyLimit;

        public void UseOnce() => _used++;

        //public int Remaining() => DailyLimit - _used;

        public int Remaining()
        {
            ResetIfNewDay();

            //return DailyLimit - TodayCount;
            return Math.Max(0, DailyLimit - TodayCount);
        }

        public void Increase()
        {
            ResetIfNewDay();

            TodayCount++;
        }

        private void ResetIfNewDay()
        {
            if (LastUsedDate.Date != DateTime.Today)
            {
                TodayCount = 0;
                LastUsedDate = DateTime.Today;
                
            }
        }
    }
}
