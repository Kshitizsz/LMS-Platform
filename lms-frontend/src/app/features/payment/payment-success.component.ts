import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-payment-success',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center">
      <div class="text-center">
        <div class="display-1 text-success mb-3">✅</div>
        <h2>Payment Successful!</h2>
        <a routerLink="/dashboard" class="btn btn-primary mt-3">Go to Dashboard</a>
      </div>
    </div>`
})
export class PaymentSuccessComponent {}