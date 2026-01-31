using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Helpers
{
    /// <summary>
    /// Provides helper methods for creating downloadable CSV files from
    /// collections of data, including player audit-specific exports.
    /// </summary>
    public static class CsvFileBuilder
    {
        /// <summary>
        /// Generates a CSV file for the specified data collection and returns it as a downloadable file.
        /// </summary>
        /// <typeparam name="T">The type of items being converted to CSV.</typeparam>
        /// <param name="data">The collection of items to export.</param>
        /// <param name="filenamePrefix">The prefix used in the generated CSV filename.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the CSV file.</returns>
        public static FileContentResult ExportCsv<T>(IEnumerable<T> data, string filenamePrefix)
        {
            var csv = CsvBuilder.ToCsv(data);
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = $"{filenamePrefix}-{date}.csv"
            };
        }

        /// <summary>
        /// Generates a CSV file for player-related audit records and returns it as a downloadable file.
        /// </summary>
        /// <typeparam name="T">The audit DTO type being exported.</typeparam>
        /// <param name="playerId">The ID of the player whose audits are exported.</param>
        /// <param name="data">The collection of audit entries.</param>
        /// <param name="suffix">A suffix describing the audit type (e.g., "bans", "limits").</param>
        /// <returns>A <see cref="FileContentResult"/> containing the CSV file.</returns>
        public static FileContentResult ExportPlayerAuditCsv<T>(Guid playerId, IEnumerable<T> data, string suffix)
        {
            var csv = CsvBuilder.ToCsv(data);
            var shortId = playerId.ToString()[..8];
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = $"player-{shortId}-{suffix}-{date}.csv"
            };
        }
    }
}
