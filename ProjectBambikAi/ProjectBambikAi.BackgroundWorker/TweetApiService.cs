using ProjectBambikDraft.Shared.Tweets.JsonEntities;
using System.Text.Json;

namespace ProjectBambikDraft.BackgroundService
{
    public interface ITweetApiService
    {
        Task<List<Tweet>> GetTweets(DateTime? dateTime);
    }

    public class TweetApiService : ITweetApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public TweetApiService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Tweet>> GetTweets(DateTime? dateTime)
        {
            if(dateTime is null)
            {
                dateTime = DateTime.Now.AddMinutes(-15);
            }

            string date = dateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            var response = await _httpClient.GetAsync($"/tweets?from_date={date}");

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                
                return JsonSerializer.Deserialize<List<Tweet>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }

            _logger.LogError($"Failed to get tweets from the API, error: {content}");
            return null;
        }
    }
}
