﻿
namespace EzDotNetty.Service
{
    public class SchedulerService
    {
        private static SchedulerService? _instance;
        private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        private SchedulerService() { }
        public static SchedulerService Instance => _instance ??= new SchedulerService();
        public void ScheduleTask(string timerId, double intervalInHour, Action task)
        {
            var now = DateTime.Now;
            var firstRun = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0);
            var timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));
            timers.Add(timerId, timer);
        }

        public void RemoveScheduler(string timerId)
        {
            if (!timers.TryGetValue(timerId, out var timer)) return;
            
            timer.Dispose();
            timers.Remove(timerId);
        }
    }

    public static class Scheduler
    {
        public static void IntervalInSeconds(string timerId, double interval, Action task)
        {
            interval /= 3600;
            SchedulerService.Instance.ScheduleTask(timerId, interval, task);
        }
        public static void IntervalInMinutes(string timerId, double interval, Action task)
        {
            interval /= 60;
            SchedulerService.Instance.ScheduleTask(timerId, interval, task);
        }
        public static void IntervalInHours(string timerId, double interval, Action task)
        {
            SchedulerService.Instance.ScheduleTask(timerId, interval, task);
        }
        public static void IntervalInDays(string timerId, double interval, Action task)
        {
            interval *= 24;
            SchedulerService.Instance.ScheduleTask(timerId, interval, task);
        }

        public static void RemoveScheduler(string timerId)
        {
            SchedulerService.Instance.RemoveScheduler(timerId);
        }
    }
}
