import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environments';
import { Course, CreateCourseRequest } from '../models/course.model';

@Injectable({ providedIn: 'root' })
export class CourseService {

  private apiUrl = `${environment.apiUrl}/courses`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Course[]> {
    return this.http.get<Course[]>(this.apiUrl);
  }

  getById(id: string): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  create(req: CreateCourseRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, req);
  }
}