using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Contract;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ICourse>> GetCourse(Guid id)
        {
            var course = await courseService.GetCourse(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpGet("all-keys/{id?}")]
        public async Task<ActionResult<ICourse>> GetAllKeysCourse(Guid? id)
        {
            var course = await courseService.GetAllKeysCourse(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPost("beginner")]
        public async Task<ActionResult<ICourse>> GenerateBeginnerCourse([FromBody] CourseSetting settings)
        {
            var course = await courseService.GenerateBeginnerCourse(settings);
            return Ok(course);
        }

        [HttpGet("start-stats")]
        public async Task<ActionResult<ICourse>> GenerateStartStats()
        {
            var course = await courseService.GenerateStartStats();
            return Ok(course);
        }

        [HttpPost("practice-lesson/{courseId}/{lessonId}")]
        public async Task<ActionResult<Lesson>> GetPracticeLesson(Guid courseId, int lessonId, [FromBody] CourseSetting settings)
        {
            var lesson = await courseService.GetPracticeLesson(courseId, lessonId, settings);
            if (lesson == null)
            {
                return NotFound();
            }
            return Ok(lesson);
        }
    }
}