import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './shared/component/navbar/navbar.component';
import { AuthService } from './core/services/auth.service';
import { SignalRService } from './core/services/signalr.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent implements OnInit {
  notification: string | null = null;

  constructor(
    private auth: AuthService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.signalR.startConnection();
    }
    this.signalR.notification.subscribe(n => {
      if (n) {
        this.notification = n.message;
        setTimeout(() => this.notification = null, 4000);
      }
    });
  }
}