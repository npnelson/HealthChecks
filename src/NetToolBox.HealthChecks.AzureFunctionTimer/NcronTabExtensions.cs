using NCrontab;
using System;
using System.Linq;
using static MoreLinq.Extensions.PairwiseExtension;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    internal static class NcronTabExtensions
    {

        public static DateTime GetLastScheduledOccurence(this CrontabSchedule schedule, DateTime dateTime)
        {
            var occurences = schedule.GetNextOccurrences(dateTime, DateTime.MaxValue)
                  .Cast<DateTime?>()
                  .Prepend(default(DateTime?))
                .Pairwise((prev, curr) => new { Previous = prev, Current = curr }).Take(5);
            var occurence = occurences.Skip(1).Take(1).Single();
            var timespan = occurence.Current - occurence.Previous;

            DateTime lastOccurrence = (occurence.Previous ?? DateTime.MinValue) - (timespan ?? TimeSpan.FromSeconds(0));
            return lastOccurrence;
        }

        public static DateTime GetLastScheduledOccurrence(string ncronExpression, DateTime dateTime)
        {
            var cron = CrontabSchedule.Parse(ncronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            var retval = cron.GetLastScheduledOccurence(dateTime);
            return retval;

        }
    }

}
