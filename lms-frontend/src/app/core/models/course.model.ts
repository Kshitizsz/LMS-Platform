export interface Course {
  id: string;
  title: string;
  description: string;
  price: number;
  thumbnailUrl: string;
  isPublished: boolean;
  instructorName: string;
  totalModules: number;
  totalLessons: number;
  createdAt: string;
}

export interface CreateCourseRequest {
  title: string;
  description: string;
  price: number;
  thumbnailUrl: string;
}