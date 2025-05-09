using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Server.Controllers
{
    [ApiController]
    [Route("api/courses")]
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

        [HttpGet("by-type")]
        public async Task<ActionResult<IEnumerable<ICourse>>> GetCoursesByType([FromQuery] int accountId, [FromQuery] TrainingType type)
        {
            var courses = await courseService.GetCoursesByType(accountId, type);
            if (!courses.Any())
            {
                return NotFound();
            }
            return Ok(courses);
        }

        [HttpGet("by-type-guest")]
        public async Task<ActionResult<ICourse>> GetCoursesByType([FromQuery] TrainingType type)
        {
            var course = await courseService.GetCoursesByTypeForGuest(type);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("Course data is required");
            }

            // Assuming we need to add a method to ICourseService for creating courses
            var createdCourse = await courseService.CreateCourse(courseDto);
            if (createdCourse == null)
            {
                return BadRequest("Failed to create the course");
            }

            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
        }


        [HttpPost("beginner")]
        public async Task<ActionResult<CourseDto>> GenerateBeginnerCourse([FromBody] CourseSetting settings)
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
        public async Task<ActionResult<PracticeLessonResult>> GetPracticeLesson([FromBody] LessonRequest request)
        {
            var lessonDto = await courseService.GetPracticeLesson(
                request.CourseId, 
                request.LessonId, 
                request.Stats,
                request.Phase,
                request.MaxCharacters);

            if (courseService.ProcessResult.HasErrors)
            {
                return BadRequest(courseService.ProcessResult.ErrorMessage);
            }

            if (lessonDto == null)
            {
                return NotFound("Lesson not found");
            }
            return Ok(lessonDto);
        }
    }
}