import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environments';
  import { CheckoutResponse } from '../models/payment.model';
import { loadStripe } from '@stripe/stripe-js';

@Injectable({ providedIn: 'root' })
export class PaymentService {

  private apiUrl = `${environment.apiUrl}/payments`;

  constructor(private http: HttpClient) {}

  createCheckout(courseId: string): Observable<CheckoutResponse> {
    return this.http.post<CheckoutResponse>(
      `${this.apiUrl}/checkout`, { courseId });
  }

  async redirectToCheckout(checkoutUrl: string): Promise<void> {
    window.location.href = checkoutUrl;
  }
}