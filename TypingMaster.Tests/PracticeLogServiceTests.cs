using System.Security.Cryptography;
using AutoMapper;
using Moq;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Mapping;
using TypingMaster.Core.Models;
using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Data;
using Xunit;

namespace TypingMaster.Tests;

public class PracticeLogServiceTests
{
    private readonly Mock<IPracticeLogRepository> _practiceLogRepositoryMock;
    private readonly Mock<IDrillStatsRepository> _drillStatsRepositoryMock;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly PracticeLogService _service;

    public PracticeLogServiceTests()
    {
        _practiceLogRepositoryMock = new Mock<IPracticeLogRepository>();
        _drillStatsRepositoryMock = new Mock<IDrillStatsRepository>();

        // Setup AutoMapper with real configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DomainMapProfile>();
        });
        _mapper = config.CreateMapper();

        _logger = new LoggerConfiguration().CreateLogger();
        _service = new PracticeLogService(
            _practiceLogRepositoryMock.Object,
            _drillStatsRepositoryMock.Object,
            _mapper,
            _logger);
    }

    [Fact]
    public async Task GetPracticeLogById_ExistingId_ReturnsPracticeLog()
    {
        // Arrange
        var practiceLogDao = new PracticeLogDao
        {
            Id = 1,
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1,
            PracticeStats = new List<DrillStatsDao>()
        };

        _practiceLogRepositoryMock.Setup(x => x.GetPracticeLogByIdAsync(1))
            .ReturnsAsync(practiceLogDao);

        // Act
        var result = await _service.GetPracticeLogById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(practiceLogDao.Id, result.Id);
        Assert.Equal(practiceLogDao.CurrentCourseId, result.CurrentCourseId);
    }

    [Fact]
    public async Task AddDrillStat_ValidData_AddsDrillStat()
    {
        // Arrange
        var practiceLogId = 1;
        var practiceLogDao = new PracticeLogDao
        {
            Id = practiceLogId,
            CurrentCourseId = Guid.NewGuid(),
            CurrentLessonId = 1
        };

        var drillStat = new DrillStats
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            PracticeText = "Test text",
            TypedText = "Typed text",
            Accuracy = 95.5,
            Wpm = 60
        };

        var drillStatDao = _mapper.Map<DrillStatsDao>(drillStat);
        drillStatDao.Id = 1;
        drillStatDao.PracticeLogId = practiceLogId;

        _practiceLogRepositoryMock.Setup(x => x.GetPracticeLogByIdAsync(practiceLogId))
            .ReturnsAsync(practiceLogDao);
        _drillStatsRepositoryMock.Setup(x => x.CreateDrillStatAsync(It.IsAny<DrillStatsDao>()))
            .ReturnsAsync(drillStatDao);

        // Act
        var result = await _service.AddDrillStat(practiceLogId, drillStat);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(drillStat.CourseId, result.CourseId);
        Assert.Equal(drillStat.Accuracy, result.Accuracy);
        Assert.Equal(drillStat.Wpm, result.Wpm);
        Assert.Equal(practiceLogId, result.PracticeLogId);
    }

    [Fact]
    public async Task AddDrillStat_NonExistingPracticeLog_ReturnsNull()
    {
        // Arrange
        var practiceLogId = 999;
        var drillStat = new DrillStats
        {
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            Accuracy = 95.5,
            Wpm = 60
        };

        _practiceLogRepositoryMock.Setup(x => x.GetPracticeLogByIdAsync(practiceLogId))
            .ReturnsAsync((PracticeLogDao?)null);

        // Act
        var result = await _service.AddDrillStat(practiceLogId, drillStat);

        // Assert
        Assert.Null(result);
        _drillStatsRepositoryMock.Verify(x => x.CreateDrillStatAsync(It.IsAny<DrillStatsDao>()), Times.Never);
    }

    [Fact]
    public async Task GetDrillStatsByPracticeLogId_ExistingId_ReturnsDrillStats()
    {
        // Arrange
        var practiceLogId = 1;
        var drillStatsDao = new List<DrillStatsDao>
        {
            new()
            {
                Id = 1,
                PracticeLogId = practiceLogId,
                CourseId = Guid.NewGuid(),
                LessonId = 1,
                Accuracy = 95.5,
                Wpm = 60
            },
            new()
            {
                Id = 2,
                PracticeLogId = practiceLogId,
                CourseId = Guid.NewGuid(),
                LessonId = 2,
                Accuracy = 92.0,
                Wpm = 65
            }
        };

        _drillStatsRepositoryMock.Setup(x => x.GetPaginatedDrillStatsByPracticeLogIdAsync(practiceLogId, 1, 10, true, null))
            .ReturnsAsync(new ValueTuple<IEnumerable<DrillStatsDao>, int>(drillStatsDao, drillStatsDao.Count));

        // Act
        var result = await _service.GetPaginatedDrillStatsByPracticeLogId(practiceLogId);

        // Assert
        var resultList = result.Items.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal(drillStatsDao[0].Accuracy, resultList[0].Accuracy);
        Assert.Equal(drillStatsDao[1].Wpm, resultList[1].Wpm);
    }

    [Fact]
    public async Task UpdateDrillStat_ExistingDrillStat_UpdatesAndReturnsUpdatedDrillStat()
    {
        // Arrange
        var drillStat = new DrillStats
        {
            Id = 1,
            PracticeLogId = 1,
            CourseId = Guid.NewGuid(),
            LessonId = 1,
            Accuracy = 98.0,
            Wpm = 70
        };

        var drillStatDao = _mapper.Map<DrillStatsDao>(drillStat);

        _drillStatsRepositoryMock.Setup(x => x.UpdateDrillStatAsync(It.IsAny<DrillStatsDao>()))
            .ReturnsAsync(drillStatDao);

        // Act
        var result = await _service.UpdateDrillStat(drillStat);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(drillStat.Id, result.Id);
        Assert.Equal(drillStat.Accuracy, result.Accuracy);
        Assert.Equal(drillStat.Wpm, result.Wpm);
    }

    [Fact]
    public async Task DeleteDrillStat_ExistingId_ReturnsTrue()
    {
        // Arrange
        var drillStatId = 1;
        _drillStatsRepositoryMock.Setup(x => x.DeleteDrillStatAsync(drillStatId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteDrillStat(drillStatId);

        // Assert
        Assert.True(result);
    }
}