using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicalDecisionSupportService.IntegrationTests;

public sealed class NewsScoreEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NewsScoreEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task post_returns_3_for_spec_example()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 37 },
                new { type = "HR", value = 60 },
                new { type = "RR", value = 5 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ScoreResponse>();
        Assert.NotNull(body);
        Assert.Equal(3, body.Score);
    }

    [Fact]
    public async Task post_returns_0_for_all_normal_values()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 37 },
                new { type = "HR", value = 60 },
                new { type = "RR", value = 15 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ScoreResponse>();
        Assert.NotNull(body);
        Assert.Equal(0, body.Score);
    }

    [Fact]
    public async Task post_returns_9_for_all_max_scores()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 32 },
                new { type = "HR", value = 30 },
                new { type = "RR", value = 5 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ScoreResponse>();
        Assert.NotNull(body);
        Assert.Equal(9, body.Score);
    }

    [Fact]
    public async Task post_returns_400_for_missing_measurement()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 37 },
                new { type = "HR", value = 60 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task post_returns_400_for_out_of_range_value()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 99 },
                new { type = "HR", value = 60 },
                new { type = "RR", value = 15 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task post_returns_400_for_invalid_measurement_type()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "INVALID", value = 37 },
                new { type = "HR", value = 60 },
                new { type = "RR", value = 15 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task post_returns_400_for_empty_measurements()
    {
        var request = new
        {
            measurements = Array.Empty<object>()
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task post_returns_400_for_duplicate_measurement_type()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "TEMP", value = 37 },
                new { type = "TEMP", value = 38 },
                new { type = "HR", value = 60 },
                new { type = "RR", value = 15 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task post_accepts_lowercase_measurement_types()
    {
        var request = new
        {
            measurements = new[]
            {
                new { type = "temp", value = 37 },
                new { type = "hr", value = 60 },
                new { type = "rr", value = 15 },
            }
        };

        var response = await _client.PostAsJsonAsync("/news-score", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ScoreResponse>();
        Assert.NotNull(body);
        Assert.Equal(0, body.Score);
    }

    private sealed record ScoreResponse(int Score);
}
