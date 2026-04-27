import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { CourseService } from '../../../core/services/course.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { AuthService } from '../../../core/services/auth.service';
import { Course } from '../../../core/models/course.model';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent implements OnInit {
  courses: Course[] = [];
  loading = true;
  isLoggedIn = false;

  constructor(
    private courseService: CourseService,
    private enrollmentService: EnrollmentService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.auth.isLoggedIn();
    this.courseService.getAll().subscribe({
      next: data => { this.courses = data; this.loading = false; },
      error: () => this.loading = false
    });
  }

  enroll(courseId: string): void {
    this.enrollmentService.enroll(courseId).subscribe({
      next: () => {
        alert('Successfully enrolled!');
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        alert(err.error?.message || 'Enrollment failed.');
      }
    });
  }
}