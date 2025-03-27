using System.Net.Http.Json;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services;

public class CourseWebService(HttpClient httpClient) : ICourseWebService
{
    private const string BaseUrl = "api/account/course";

    public async Task<CourseBase?> GetCourse(Guid id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<CourseBase>($"{BaseUrl}/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<CourseBase?> GetAllKeysCourse(Guid? id)
    {
        try
        {
            var url = id.HasValue ? $"{BaseUrl}/all-keys/{id}" : $"{BaseUrl}/all-keys";
            return await httpClient.GetFromJsonAsync<CourseBase>(url);
        }
        catch
        {
            return null;
        }
    }

    public async Task<CourseBase> GenerateBeginnerCourse(CourseSetting settings)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/beginner", settings);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseBase>()
                ?? throw new InvalidOperationException("Failed to deserialize course");
        }
        catch
        {
            throw;
        }
    }

    public async Task<DrillStats?> GenerateStartStats()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<DrillStats>($"{BaseUrl}/start-stats");
        }
        catch
        {
            throw;
        }
    }

    public async Task<Lesson?> GetPracticeLesson(Guid courseId, int lessonId, CourseSetting settings)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/course/{courseId}/{lessonId}", settings);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Lesson>()
                ?? throw new InvalidOperationException("Failed to deserialize lesson");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}