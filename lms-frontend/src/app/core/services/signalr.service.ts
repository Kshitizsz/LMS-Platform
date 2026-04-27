import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environments';

export interface Notification {
  type: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class SignalRService {

  private hub!: signalR.HubConnection;
  private notification$ = new BehaviorSubject<Notification | null>(null);

  notification = this.notification$.asObservable();

  startConnection(): void {
    const token = localStorage.getItem('token');

    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/notifications`, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.hub.on('ReceiveNotification', (data: Notification) => {
      this.notification$.next(data);
    });

    this.hub.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR error:', err));
  }

  joinCourseGroup(courseId: string): void {
    this.hub?.invoke('JoinCourseGroup', courseId);
  }

  stopConnection(): void {
    this.hub?.stop();
  }
}