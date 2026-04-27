import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;
  fullName = '';
  role = '';

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.auth.getLoggedIn$().subscribe(val => {
      this.isLoggedIn = val;
      this.fullName = this.auth.getFullName();
      this.role = this.auth.getRole();
    });
  }

  logout(): void { this.auth.logout(); }
}