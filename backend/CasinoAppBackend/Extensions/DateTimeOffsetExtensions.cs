namespace CasinoAppBackend.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DateTimeOffset"/> to calculate
    /// the start of day, week, and month in UTC.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Returns the first moment (00:00:00) of the current UTC day.
        /// </summary>
        /// <param name="dateTime">The UTC date to evaluate.</param>
        /// <returns>
        /// A <see cref="DateTimeOffset"/> representing the beginning of the same day in UTC.
        /// </returns>
        /// <example>
        /// <code>
        /// var startOfDay = DateTimeOffset.UtcNow.StartOfDayUtc();
        /// // e.g., 2025-11-05 00:00:00 +00:00
        /// </code>
        /// </example>
        public static DateTimeOffset StartOfDayUtc(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                0, 0, 0,
                TimeSpan.Zero
            );
        }

        /// <summary>
        /// Returns the first moment (Monday 00:00:00) of the current UTC week.
        /// Assumes weeks start on Monday.
        /// </summary>
        /// <param name="dateTime">The UTC date to evaluate.</param>
        /// <returns>
        /// A <see cref="DateTimeOffset"/> representing the start of the current week in UTC.
        /// </returns>
        /// <example>
        /// <code>
        /// var startOfWeek = DateTimeOffset.UtcNow.StartOfWeekUtc();
        /// // e.g., if today is Wednesday 2025-11-05,
        /// // returns Monday 2025-11-03 00:00:00 +00:00
        /// </code>
        /// </example>
        public static DateTimeOffset StartOfWeekUtc(this DateTimeOffset dateTime)
        {
            var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            var monday = dateTime.Date.AddDays(-diff);

            return new DateTimeOffset(monday, TimeSpan.Zero);
        }

        /// <summary>
        /// Returns the first moment (00:00:00) of the current UTC month.
        /// </summary>
        /// <param name="dateTime">The UTC date to evaluate.</param>
        /// <returns>
        /// A <see cref="DateTimeOffset"/> representing the beginning of the month.
        /// </returns>
        /// <example>
        /// <code>
        /// var startOfMonth = DateTimeOffset.UtcNow.StartOfMonthUtc();
        /// // e.g., for November 2025, returns 2025-11-01 00:00:00 +00:00
        /// </code>
        /// </example>
        public static DateTimeOffset StartOfMonthUtc(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                1,
                0, 0, 0,
                TimeSpan.Zero
            );
        }
    }
}
