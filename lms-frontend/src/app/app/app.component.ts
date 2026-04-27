import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import { SignalRService } from '../core/services/signalr.service';
import { NavbarComponent } from "../shared/component/navbar/navbar.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [NavbarComponent, RouterModule, NgIf]
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