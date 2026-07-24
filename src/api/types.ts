export type ApiCategory = {
  id: number;
  name: string;
  description: string;
  products?: ApiProduct[];
};

export type ApiProduct = {
  id: number;
  name: string;
  price: number;
  stock: number;
  freshnessScore: number;
  imageUrl: string;
  categoryId: number;
  category?: ApiCategory | null;
  defaultVaseLifeDays: number;
};

export type ApiDistrict = {
  id: number;
  name: string;
  baseDeliveryFee: number;
};

export type CreateOrderItem = {
  productId: number;
  quantity: number;
};

export type CreateOrderRequest = {
  customerName: string;
  customerPhone: string;
  address: string;
  districtId: number;
  items: CreateOrderItem[];
};

export type CreateOrderResponse = {
  message: string;
  orderId: number;
  customerName: string;
  districtName: string;
  subTotal: number;
  deliveryFee: number;
  grandTotal: number;
  orderStatus: string;
  orderDate: string;
};
