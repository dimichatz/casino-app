namespace CasinoAppBackend.Helpers
{
    /// <summary>
    /// Provides helper methods for sanitizing input data,
    /// including trimming whitespace from string properties in DTOs.
    /// </summary>
    public static class InputSanitizer
    {
        /// <summary>
        /// Recursively trims whitespaces from all string properties
        /// of the given object and its nested objects or collections.
        /// </summary>
        /// <param name="obj">The object to sanitize recursively.</param>
        /// <remarks>
        /// Use this method for complex DTOs that contain nested objects or lists.
        /// It will skip primitive, enum, and value types to avoid unwanted recursion.
        /// </remarks>
        public static void TrimStringsDeep(object? obj)
        {
            if (obj == null) return;

            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                if (prop.PropertyType == typeof(string))
                {
                    var value = (string?)prop.GetValue(obj);
                    if (!string.IsNullOrWhiteSpace(value))
                        prop.SetValue(obj, value.Trim());
                }
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                         prop.PropertyType != typeof(string))
                {
                    // Handle lists or collections of nested DTOs
                    var items = prop.GetValue(obj) as System.Collections.IEnumerable;
                    if (items != null)
                    {
                        foreach (var item in items)
                            TrimStringsDeep(item);
                    }
                }
                else if (!prop.PropertyType.IsPrimitive &&
                         !prop.PropertyType.IsEnum &&
                         !prop.PropertyType.IsValueType)
                {
                    // Recurse into nested objects
                    var nested = prop.GetValue(obj);
                    TrimStringsDeep(nested);
                }
            }
        }
    }
}
