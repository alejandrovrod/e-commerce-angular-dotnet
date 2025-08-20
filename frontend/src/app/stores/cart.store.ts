import { Injectable, computed, signal, effect } from '@angular/core';
import { CartItem, Cart } from '../core/models/cart.model';
import { Product } from '../core/models/product.model';

export interface CartState {
  items: CartItem[];
  isLoading: boolean;
  error: string | null;
  lastUpdated: Date | null;
}

@Injectable({
  providedIn: 'root'
})
export class CartStore {
  // Private state signals
  private readonly _items = signal<CartItem[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _lastUpdated = signal<Date | null>(null);

  // Public readonly signals
  readonly items = this._items.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly lastUpdated = this._lastUpdated.asReadonly();

  // Computed signals
  readonly itemCount = computed(() => 
    this._items().reduce((total, item) => total + item.quantity, 0)
  );

  readonly subtotal = computed(() => 
    this._items().reduce((total, item) => total + (item.product.price * item.quantity), 0)
  );

  readonly tax = computed(() => this.subtotal() * 0.08); // 8% tax

  readonly shipping = computed(() => {
    const subtotal = this.subtotal();
    return subtotal > 50 ? 0 : 5.99; // Free shipping over $50
  });

  readonly total = computed(() => 
    this.subtotal() + this.tax() + this.shipping()
  );

  readonly isEmpty = computed(() => this._items().length === 0);

  readonly uniqueItemCount = computed(() => this._items().length);

  // Effect to persist cart to localStorage
  constructor() {
    effect(() => {
      const items = this._items();
      if (items.length > 0) {
        localStorage.setItem('cart_items', JSON.stringify(items));
      } else {
        localStorage.removeItem('cart_items');
      }
    });

    // Load cart from localStorage on initialization
    this.loadCartFromStorage();
  }

  // State management methods
  addItem(product: Product, quantity: number = 1): void {
    this.setLoading(true);
    
    try {
      const items = [...this._items()];
      const existingItemIndex = items.findIndex(item => item.product.id === product.id);

      if (existingItemIndex > -1) {
        // Update existing item
        items[existingItemIndex] = {
          ...items[existingItemIndex],
          quantity: items[existingItemIndex].quantity + quantity
        };
      } else {
        // Add new item
        items.push({
          id: this.generateCartItemId(),
          product,
          quantity,
          addedAt: new Date()
        });
      }

      this._items.set(items);
      this._lastUpdated.set(new Date());
      this._error.set(null);
    } catch (error) {
      this._error.set('Failed to add item to cart');
    } finally {
      this.setLoading(false);
    }
  }

  removeItem(productId: string): void {
    const items = this._items().filter(item => item.product.id !== productId);
    this._items.set(items);
    this._lastUpdated.set(new Date());
  }

  updateQuantity(productId: string, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(productId);
      return;
    }

    const items = this._items().map(item => 
      item.product.id === productId 
        ? { ...item, quantity }
        : item
    );
    
    this._items.set(items);
    this._lastUpdated.set(new Date());
  }

  clearCart(): void {
    this._items.set([]);
    this._lastUpdated.set(new Date());
    this._error.set(null);
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
    this._isLoading.set(false);
  }

  // Utility methods
  getItemByProductId(productId: string): CartItem | undefined {
    return this._items().find(item => item.product.id === productId);
  }

  hasProduct(productId: string): boolean {
    return this._items().some(item => item.product.id === productId);
  }

  getProductQuantity(productId: string): number {
    const item = this.getItemByProductId(productId);
    return item?.quantity || 0;
  }

  // Private methods
  private loadCartFromStorage(): void {
    try {
      const savedItems = localStorage.getItem('cart_items');
      if (savedItems) {
        const items: CartItem[] = JSON.parse(savedItems);
        this._items.set(items);
      }
    } catch (error) {
      console.error('Error loading cart from storage:', error);
      this._error.set('Failed to load saved cart');
    }
  }

  private generateCartItemId(): string {
    return `cart_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  // Bulk operations
  addMultipleItems(items: { product: Product; quantity: number }[]): void {
    this.setLoading(true);
    
    try {
      items.forEach(({ product, quantity }) => {
        this.addItem(product, quantity);
      });
    } catch (error) {
      this._error.set('Failed to add items to cart');
    } finally {
      this.setLoading(false);
    }
  }

  // Cart synchronization (for logged-in users)
  syncWithServer(serverCart: CartItem[]): void {
    this._items.set(serverCart);
    this._lastUpdated.set(new Date());
  }

  // Export cart data
  exportCart(): Cart {
    return {
      items: this._items(),
      subtotal: this.subtotal(),
      tax: this.tax(),
      shipping: this.shipping(),
      total: this.total(),
      lastUpdated: this._lastUpdated()
    };
  }
}
