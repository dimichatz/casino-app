using System.Reflection;
using CasinoAppBackend.Data;

namespace CasinoAppBackend.Helpers
{
    /// <summary>
    /// Provides helper methods for building audit records that track changes
    /// between two instances of the same entity type.
    /// </summary>
    public static class EntityAuditBuilder
    {
        /// <summary>
        /// Compares two entity instances (of the same type) and generates a list of
        /// <see cref="PlayerDetailsAudit"/> records for all properties that have changed.
        /// </summary>
        /// <typeparam name="T">The entity type being compared (e.g., Player, Admin).</typeparam>
        /// <param name="oldEntity">The original entity before changes.</param>
        /// <param name="updatedEntity">The updated entity after changes.</param>
        /// <param name="changedByUser">The user who made the change.</param>
        /// <param name="playerId">The ID of the player associated with this audit record.</param>
        /// <param name="comment">An optional comment describing the reason for the change.</param>
        /// <returns>
        /// A list of <see cref="PlayerDetailsAudit"/> objects, each representing
        /// a single property that has changed.
        /// </returns>
        public static List<PlayerDetailsAudit> GetChangedFields<T>(T oldEntity, T updatedEntity,
            User changedByUser, Guid playerId, string? comment = null)
        {
            var audits = new List<PlayerDetailsAudit>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanRead ||
                    (prop.PropertyType.IsClass && prop.PropertyType != typeof(string) && !prop.PropertyType.IsValueType))
                    continue;

                var oldValue = prop.GetValue(oldEntity)?.ToString()?.Trim() ?? "";
                var newValue = prop.GetValue(updatedEntity)?.ToString()?.Trim() ?? "";

                if (oldValue != newValue)
                {
                    audits.Add(new PlayerDetailsAudit
                    {
                        ChangedByUserId = changedByUser.Id,
                        ChangedByUsername = changedByUser.Username,
                        FieldName = prop.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                        Comment = comment,
                        PlayerId = playerId
                    });
                }
            }
            return audits;
        }
    }
}
