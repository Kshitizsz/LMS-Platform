using System;

namespace LMS.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = "Video"; // Video | Article | Quiz
    public int DurationMinutes { get; set; }
    public int Order { get; set; }

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}