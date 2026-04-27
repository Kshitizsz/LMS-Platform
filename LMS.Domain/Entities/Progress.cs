namespace LMS.Domain.Entities;

public class Progress
{
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public Guid EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}