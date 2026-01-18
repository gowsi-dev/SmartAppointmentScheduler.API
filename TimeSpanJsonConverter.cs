using System.Text.Json.Serialization;
using System.Text.Json;

namespace SmartAppointmentScheduler
{
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string timeString = reader.GetString()?.Trim();

                    if (string.IsNullOrEmpty(timeString))
                        return TimeSpan.Zero;

                    // Handle "9.30" format (with dot)
                    if (timeString.Contains('.'))
                    {
                        return ParseDotFormat(timeString);
                    }

                    // Handle "09:30" format (with colon)
                    if (timeString.Contains(':'))
                    {
                        if (TimeSpan.TryParse(timeString, out TimeSpan result))
                            return result;
                    }

                    // Handle plain number like "9" or "9.5"
                    if (double.TryParse(timeString, out double hours))
                    {
                        return TimeSpan.FromHours(hours);
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    // Handle JSON number like 9.5
                    double totalHours = reader.GetDouble();
                    return TimeSpan.FromHours(totalHours);
                }

                throw new JsonException($"Invalid time format: {reader.GetString()}");
            }
            catch (Exception ex)
            {
                throw new JsonException($"Failed to parse TimeSpan: {ex.Message}");
            }
        }

        private TimeSpan ParseDotFormat(string timeString)
        {
            try
            {
                // Remove any extra spaces
                timeString = timeString.Trim();

                // Split by dot
                string[] parts = timeString.Split('.');

                if (parts.Length != 2)
                    throw new FormatException("Time should be in format 'hours.minutes' like '9.30'");

                // Parse hours
                if (!int.TryParse(parts[0], out int hours))
                    throw new FormatException($"Invalid hours: {parts[0]}");

                // Parse minutes - handle different cases
                string minutesStr = parts[1];

                // If single digit, assume it's tens of minutes
                if (minutesStr.Length == 1)
                {
                    minutesStr += "0"; // "3" becomes "30"
                }
                // If more than 2 digits, take first 2
                else if (minutesStr.Length > 2)
                {
                    minutesStr = minutesStr.Substring(0, 2);
                }

                if (!int.TryParse(minutesStr, out int minutes))
                    throw new FormatException($"Invalid minutes: {parts[1]}");

                // Validate
                if (hours < 0 || hours > 23)
                    throw new FormatException($"Hours must be 0-23, got {hours}");

                if (minutes < 0 || minutes > 59)
                    throw new FormatException($"Minutes must be 0-59, got {minutes}");

                return new TimeSpan(hours, minutes, 0);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse '{timeString}': {ex.Message}");
            }
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // Write as string in "hh:mm" format
            writer.WriteStringValue(value.ToString(@"hh\:mm"));
        }
    }
}

