using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hotzerver.pilkki.calendar.Data;

public class HolidayService(HttpClient httpClient)
{
    private const string HolidaysUrl = "https://pyhäpäivä.fi/?output=json";
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static List<HolidayItem>? _cache;

    public async Task<List<HolidayItem>> GetHolidaysAsync()
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await CacheLock.WaitAsync();
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            using var stream = await httpClient.GetStreamAsync(HolidaysUrl);
            var response = await JsonSerializer.DeserializeAsync<List<HolidayApiResponse>>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _cache = response?
                .Where(x => !string.IsNullOrWhiteSpace(x.Date) && !string.IsNullOrWhiteSpace(x.Title))
                .Select(x => new HolidayItem
                {
                    StartDate = DateOnly.ParseExact(x.Date!, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EndDate = string.IsNullOrWhiteSpace(x.EndDate)
                        ? null
                        : DateOnly.ParseExact(x.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Title = x.Title!
                })
                .ToList() ?? [];

            return _cache;
        }
        finally
        {
            CacheLock.Release();
        }
    }

    public class HolidayItem
    {
        public required DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public required string Title { get; set; }
    }

    private class HolidayApiResponse
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
