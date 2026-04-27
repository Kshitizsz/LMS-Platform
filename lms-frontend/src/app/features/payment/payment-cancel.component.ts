import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-payment-cancel',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center">
      <div class="text-center">
        <div class="display-1 text-danger mb-3">❌</div>
        <h2>Payment Cancelled</h2>
        <a routerLink="/courses" class="btn btn-outline-primary mt-3">Back to Courses</a>
      </div>
    </div>`
})
export class PaymentCancelComponent {}