using System.Text;

namespace CasinoAppBackend.Helpers
{
    /// <summary>
    /// Provides functionality for generating CSV-formatted strings from
    /// collections of objects using reflection.
    /// </summary>
    public static class CsvBuilder
    {
        /// <summary>
        /// Converts a collection of objects into a CSV-formatted string.
        /// </summary>
        /// <typeparam name="T">The type of objects being converted.</typeparam>
        /// <param name="items">The collection of objects to convert.</param>
        /// <returns>
        /// A CSV-formatted string containing a header row and one row per item in the collection.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses reflection to read all public properties of <typeparamref name="T"/>.
        /// The header row contains the property names, and each subsequent row contains the values.
        /// </para>
        ///
        /// <para>
        /// Basic sanitization is applied to avoid breaking CSV formatting:
        /// commas are replaced with spaces, and newlines are removed.
        /// If stronger CSV escaping is needed (quotes, embedded commas, etc.),
        /// this implementation can be extended.
        /// </para>
        ///
        /// <para>
        /// This method does not quote values or escape special characters.
        /// It is suitable for internal admin exports where data is predictable.
        /// </para>
        /// </remarks>
        public static string ToCsv<T>(IEnumerable<T> items)
        {
            var sb = new StringBuilder();
            var props = typeof(T).GetProperties();

            // Header row
            sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

            // Rows
            foreach (var item in items)
            {
                var values = props.Select(p =>
                {
                    var val = p.GetValue(item)?.ToString() ?? "";

                    // Clean the value
                    val = val.Replace(",", " "); // Prevent breaking columns
                    val = val.Replace("\n", " ").Replace("\r", " "); // Remove newlines

                    return val;
                });

                sb.AppendLine(string.Join(",", values));
            }

            return sb.ToString();
        }
    }
}
