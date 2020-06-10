using System;
using System.Timers;

namespace SME.SR.Infra.Utilitarios
{
    public static class UtilTimer
    {
        public static IInterruptable SetInterval(int interval, Action function)
        {
            return StartTimer(interval, function, true);
        }

        public static IInterruptable SetTimeout(int miliseconds, Action function)
        {
            return StartTimer(miliseconds, function, false);
        }

        private static IInterruptable StartTimer(int miliseconds, Action function, bool autoReset)
        {
            Action functionCopy = (Action)function.Clone();
            Timer timer = new Timer { Interval = miliseconds, AutoReset = autoReset };
            timer.Elapsed += (sender, e) => functionCopy();
            timer.Start();

            return new TimerInterrupter(timer);
        }
    }


    public class TimerInterrupter : IInterruptable
    {
        private readonly Timer _timer;

        public TimerInterrupter(Timer timer)
        {
            if (timer == null) throw new ArgumentNullException(nameof(timer));
            _timer = timer;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public interface IInterruptable
    {
        void Stop();
    }
}
