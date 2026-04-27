namespace LMS.Application.DTOs.Enrollment;

public record EnrollRequest(Guid CourseId);

public record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    bool IsCompleted,
    DateTime EnrolledAt,
    int ProgressPercent);