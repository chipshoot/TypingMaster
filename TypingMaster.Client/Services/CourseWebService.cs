using System.Net.Http.Json;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services;

public class CourseWebService(HttpClient httpClient, IApiConfiguration apiConfig, Serilog.ILogger logger) : ICourseWebService
{
    private const string BaseUrl = "api/course";

    public async Task<CourseDto?> GetCourse(Guid id)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/{id}");
            return await httpClient.GetFromJsonAsync<CourseDto>(url);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to get course");
            return null;
        }
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesByType(int accountId, TrainingType type)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/by-type?accountId={accountId}&type={(int)type}");
            var response = await httpClient.GetFromJsonAsync<IEnumerable<CourseDto>>(url);
            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to generate beginner course");
            return null;
        }
    }

    public async Task<CourseDto?> GetCoursesByTypeForGuest(TrainingType type)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/by-type-guest?type={(int)type}");
            var response = await httpClient.GetFromJsonAsync<CourseDto>(url);
            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to generate beginner course");
            return null;
        }
    }

    public async Task<CourseDto?> GenerateBeginnerCourse(CourseSetting settings)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/beginner");
            var response = await httpClient.PostAsJsonAsync(url, settings);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseDto>()
                ?? throw new InvalidOperationException("Failed to deserialize course");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to generate beginner course");
            return null;
        }
    }

    public async Task<CourseDto?> CreateCourse(CourseDto courseDto)
    {
        try
        {
            var url = apiConfig.BuildApiUrl(BaseUrl);
            var response = await httpClient.PostAsJsonAsync(url, courseDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseDto>()
                   ?? throw new InvalidOperationException("Failed to deserialize course");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to create course");
            return null;
        }
    }

    public async Task<DrillStats?> GenerateStartStats()
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/start-stats");
            return await httpClient.GetFromJsonAsync<DrillStats>(url);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to generate start stats");
            return null;
        }
    }

    public async Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, StatsBase stats)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{BaseUrl}/practice-lesson/{courseId}/{lessonId}");
            var response = await httpClient.PostAsJsonAsync(url, stats);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Lesson>()
                ?? throw new InvalidOperationException("Failed to deserialize lesson");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to get practice lesson");
            return null;
        }
    }
}