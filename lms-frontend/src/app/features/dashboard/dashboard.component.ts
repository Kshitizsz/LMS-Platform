import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { AuthService } from '../../core/services/auth.service';
import { Enrollment } from '../../core/models/enrollment.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  enrollments: Enrollment[] = [];
  fullName = '';
  role = '';
  loading = true;

  constructor(
    private enrollmentService: EnrollmentService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.fullName = this.auth.getFullName();
    this.role = this.auth.getRole();
    this.enrollmentService.getMyEnrollments().subscribe({
      next: data => { this.enrollments = data; this.loading = false; },
      error: err => { console.error(err); this.loading = false; }
    });
  }
}