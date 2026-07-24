import { apiFetch } from './client';
import type { CreateOrderRequest, CreateOrderResponse } from './types';

export async function createOrder(data: CreateOrderRequest): Promise<CreateOrderResponse> {
  return apiFetch<CreateOrderResponse>('/orders', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
