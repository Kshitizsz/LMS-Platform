export interface CreateCheckoutRequest {
  courseId: string;
}

export interface CheckoutResponse {
  checkoutUrl: string;
  sessionId: string;
}