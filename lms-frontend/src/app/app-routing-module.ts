import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { CourseListComponent } from './features/courses/course-list/course-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { PaymentSuccessComponent } from './features/payment/payment-success.component';
import { PaymentCancelComponent } from './features/payment/payment-cancel.component';

export const routes: Routes = [
  { path: '',                redirectTo: 'courses', pathMatch: 'full' },
  { path: 'auth/login',      component: LoginComponent },
  { path: 'auth/register',   component: RegisterComponent },
  { path: 'courses',         component: CourseListComponent },
  { path: 'dashboard',       component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'payment/success', component: PaymentSuccessComponent },
  { path: 'payment/cancel',  component: PaymentCancelComponent },
  { path: '**',              redirectTo: 'courses' }
];

export const appRoutes = routes;