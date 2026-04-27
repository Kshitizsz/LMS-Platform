export interface Enrollment {
  id: string;
  courseId: string;
  courseTitle: string;
  isCompleted: boolean;
  enrolledAt: string;
  progressPercent: number;
}