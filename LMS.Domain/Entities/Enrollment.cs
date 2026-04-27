using System;

namespace LMS.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}