using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace TypingMaster.Server.Data;

public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Extension method to configure a property to be stored as JSON in the database
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to convert</typeparam>
    /// <param name="propertyBuilder">The property builder</param>
    /// <returns>The property builder for method chaining</returns>
    public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : class
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        propertyBuilder
            .HasConversion(
                // Convert to string when saving to database
                v => JsonSerializer.Serialize(v, options),
                // Convert from string when reading from database
                v => v == null
                    ? null
                    : JsonSerializer.Deserialize<TProperty>(v, options)!
            );

        // Set the column type to jsonb for PostgreSQL
        propertyBuilder.HasColumnType("jsonb");

        return propertyBuilder;
    }
}