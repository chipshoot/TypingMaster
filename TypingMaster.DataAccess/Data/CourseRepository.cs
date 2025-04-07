using Microsoft.EntityFrameworkCore;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.DataAccess.Data;

public class CourseRepository(
    ApplicationDbContext context,
    Serilog.ILogger logger)
    : RepositoryBase(logger), ICourseRepository
{
    public async Task<CourseDao?> GetCourseByIdAsync(Guid id)
    {
        return await context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CourseDao>> GetAllCoursesAsync()
    {
        return await context.Courses
            .ToListAsync();
    }

    public async Task<CourseDao> CreateCourseAsync(CourseDao course)
    {
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        return course;
    }

    public async Task<CourseDao> UpdateCourseAsync(CourseDao course)
    {
        context.Entry(course).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return course;
    }

    public async Task<bool> DeleteCourseAsync(Guid id)
    {
        var course = await context.Courses.FindAsync(id);
        if (course == null)
            return false;

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CourseDao>> GetCoursesByTypeAsync(int accountId, TrainingType type)
    {
        var typeString = type.ToString();

        return await context.Courses
            .Where(c => c.AccountId == accountId && c.Type == typeString)
            .ToListAsync();
    }

    public async Task<CourseDao?> GetCourseByNameAsync(string name)
    {
        return await context.Courses
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}