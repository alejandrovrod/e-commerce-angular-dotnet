import { Injectable, signal, computed, effect } from '@angular/core';

export interface CartItem {
  id: string;
  productId: string;
  name: string;
  price: number;
  quantity: number;
  image?: string;
  description?: string;
}

export interface CartState {
  items: CartItem[];
  total: number;
  itemCount: number;
  isLoading: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CartStore {
  // Signals para el estado reactivo
  private readonly _items = signal<CartItem[]>([]);
  private readonly _isLoading = signal<boolean>(false);

  // Computed signals
  public readonly items = this._items.asReadonly();
  public readonly total = computed(() => 
    this._items().reduce((sum, item) => sum + (item.price * item.quantity), 0)
  );
  public readonly itemCount = computed(() => 
    this._items().reduce((sum, item) => sum + item.quantity, 0)
  );
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly isEmpty = computed(() => this._items().length === 0);

  constructor() {
    // Cargar carrito desde localStorage al inicializar
    this.loadCartFromStorage();

    // Guardar carrito en localStorage cuando cambie
    effect(() => {
      this.saveCartToStorage();
    });
  }

  // Actions
  addItem(product: Omit<CartItem, 'id'>): void {
    const existingItem = this._items().find(item => item.productId === product.productId);
    
    if (existingItem) {
      this.updateQuantity(existingItem.id, existingItem.quantity + 1);
    } else {
      const newItem: CartItem = {
        ...product,
        id: this.generateId()
      };
      this._items.update(items => [...items, newItem]);
    }
  }

  removeItem(itemId: string): void {
    this._items.update(items => items.filter(item => item.id !== itemId));
  }

  updateQuantity(itemId: string, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(itemId);
      return;
    }

    this._items.update(items =>
      items.map(item =>
        item.id === itemId ? { ...item, quantity } : item
      )
    );
  }

  clearCart(): void {
    this._items.set([]);
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  // Helpers
  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  private loadCartFromStorage(): void {
    try {
      const cartStr = localStorage.getItem('cart');
      if (cartStr) {
        const items = JSON.parse(cartStr);
        this._items.set(items);
      }
    } catch (error) {
      console.error('Error loading cart from storage:', error);
      localStorage.removeItem('cart');
    }
  }

  private saveCartToStorage(): void {
    try {
      localStorage.setItem('cart', JSON.stringify(this._items()));
    } catch (error) {
      console.error('Error saving cart to storage:', error);
    }
  }

  // Getters computados
  getItemById(itemId: string): CartItem | undefined {
    return this._items().find(item => item.id === itemId);
  }

  getItemByProductId(productId: string): CartItem | undefined {
    return this._items().find(item => item.productId === productId);
  }

  // Métodos de utilidad
  hasItem(productId: string): boolean {
    return this._items().some(item => item.productId === productId);
  }

  getItemQuantity(productId: string): number {
    const item = this.getItemByProductId(productId);
    return item ? item.quantity : 0;
  }
}
