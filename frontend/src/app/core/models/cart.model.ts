import { Product, ProductVariant } from './product.model';

export interface CartItem {
  id: string;
  product: Product;
  quantity: number;
  selectedVariants?: ProductVariant[];
  addedAt: Date;
  notes?: string;
}

export interface Cart {
  items: CartItem[];
  subtotal: number;
  tax: number;
  shipping: number;
  discount?: number;
  total: number;
  lastUpdated: Date | null;
  couponCode?: string;
}

export interface CartSummary {
  itemCount: number;
  subtotal: number;
  tax: number;
  shipping: number;
  discount: number;
  total: number;
}

export interface AddToCartRequest {
  productId: string;
  quantity: number;
  variantIds?: string[];
  notes?: string;
}

export interface UpdateCartItemRequest {
  cartItemId: string;
  quantity: number;
  variantIds?: string[];
  notes?: string;
}

export interface ShippingOption {
  id: string;
  name: string;
  description: string;
  price: number;
  estimatedDays: number;
  carrier: string;
  isDefault: boolean;
}

export interface CouponCode {
  id: string;
  code: string;
  name: string;
  description: string;
  type: 'percentage' | 'fixed_amount' | 'free_shipping';
  value: number;
  minimumAmount?: number;
  maximumDiscount?: number;
  usageLimit?: number;
  usageCount: number;
  validFrom: Date;
  validTo: Date;
  isActive: boolean;
  applicableProducts?: string[];
  applicableCategories?: string[];
}

export interface ApplyCouponRequest {
  code: string;
  cartTotal: number;
}

export interface ApplyCouponResponse {
  coupon: CouponCode;
  discountAmount: number;
  isValid: boolean;
  message: string;
}
