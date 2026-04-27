namespace LMS.Application.DTOs.Course;

public record CreateCourseRequest(
    string Title,
    string Description,
    decimal Price,
    string ThumbnailUrl);

public record UpdateCourseRequest(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string ThumbnailUrl,
    bool IsPublished);

public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string ThumbnailUrl,
    bool IsPublished,
    string InstructorName,
    int TotalModules,
    int TotalLessons,
    DateTime CreatedAt);