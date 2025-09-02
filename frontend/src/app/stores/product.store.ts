import { Injectable, signal, computed, effect } from '@angular/core';

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  originalPrice?: number;
  images: string[];
  category: string;
  tags: string[];
  stock: number;
  rating: number;
  reviewCount: number;
  isActive: boolean;
  isFeatured: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface ProductFilters {
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  rating?: number;
  inStock?: boolean;
  search?: string;
}

export interface ProductState {
  products: Product[];
  filteredProducts: Product[];
  categories: string[];
  isLoading: boolean;
  error: string | null;
  filters: ProductFilters;
}

@Injectable({
  providedIn: 'root'
})
export class ProductStore {
  // Signals para el estado reactivo
  private readonly _products = signal<Product[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _filters = signal<ProductFilters>({});

  // Computed signals
  public readonly products = this._products.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly filters = this._filters.asReadonly();

  public readonly filteredProducts = computed(() => {
    const products = this._products();
    const filters = this._filters();
    
    return products.filter(product => {
      // Filtro por categoría
      if (filters.category && product.category !== filters.category) {
        return false;
      }

      // Filtro por precio mínimo
      if (filters.minPrice && product.price < filters.minPrice) {
        return false;
      }

      // Filtro por precio máximo
      if (filters.maxPrice && product.price > filters.maxPrice) {
        return false;
      }

      // Filtro por rating
      if (filters.rating && product.rating < filters.rating) {
        return false;
      }

      // Filtro por stock
      if (filters.inStock && product.stock <= 0) {
        return false;
      }

      // Filtro por búsqueda
      if (filters.search) {
        const searchLower = filters.search.toLowerCase();
        return product.name.toLowerCase().includes(searchLower) ||
               product.description.toLowerCase().includes(searchLower) ||
               product.tags.some(tag => tag.toLowerCase().includes(searchLower));
      }

      return true;
    });
  });

  public readonly categories = computed(() => {
    const products = this._products();
    const uniqueCategories = new Set(products.map(p => p.category));
    return Array.from(uniqueCategories).sort();
  });

  public readonly featuredProducts = computed(() => {
    return this._products().filter(p => p.isFeatured && p.isActive);
  });

  public readonly activeProducts = computed(() => {
    return this._products().filter(p => p.isActive);
  });

  public readonly productsByCategory = computed(() => {
    const products = this._products();
    const result: Record<string, Product[]> = {};
    
    products.forEach(product => {
      if (!result[product.category]) {
        result[product.category] = [];
      }
      result[product.category].push(product);
    });
    
    return result;
  });

  constructor() {
    // Cargar productos de ejemplo al inicializar
    this.loadSampleProducts();
  }

  // Actions
  setProducts(products: Product[]): void {
    this._products.set(products);
    this._error.set(null);
  }

  addProduct(product: Product): void {
    this._products.update(products => [...products, product]);
  }

  updateProduct(updatedProduct: Product): void {
    this._products.update(products =>
      products.map(p => p.id === updatedProduct.id ? updatedProduct : p)
    );
  }

  deleteProduct(productId: string): void {
    this._products.update(products => products.filter(p => p.id !== productId));
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  setFilters(filters: Partial<ProductFilters>): void {
    this._filters.update(current => ({ ...current, ...filters }));
  }

  clearFilters(): void {
    this._filters.set({});
  }

  // Helpers
  private loadSampleProducts(): void {
    const sampleProducts: Product[] = [
      {
        id: '1',
        name: 'Laptop Gaming Pro',
        description: 'Potente laptop para gaming con gráficos dedicados',
        price: 1299.99,
        originalPrice: 1499.99,
        images: ['/assets/images/laptop-1.jpg'],
        category: 'Electronics',
        tags: ['gaming', 'laptop', 'performance'],
        stock: 15,
        rating: 4.8,
        reviewCount: 127,
        isActive: true,
        isFeatured: true,
        createdAt: new Date('2024-01-15'),
        updatedAt: new Date('2024-01-15')
      },
      {
        id: '2',
        name: 'Smartphone Ultra',
        description: 'Smartphone de última generación con cámara profesional',
        price: 899.99,
        images: ['/assets/images/phone-1.jpg'],
        category: 'Electronics',
        tags: ['smartphone', 'camera', 'mobile'],
        stock: 25,
        rating: 4.6,
        reviewCount: 89,
        isActive: true,
        isFeatured: false,
        createdAt: new Date('2024-01-10'),
        updatedAt: new Date('2024-01-10')
      },
      {
        id: '3',
        name: 'Auriculares Wireless',
        description: 'Auriculares bluetooth con cancelación de ruido',
        price: 199.99,
        images: ['/assets/images/headphones-1.jpg'],
        category: 'Electronics',
        tags: ['headphones', 'wireless', 'noise-cancelling'],
        stock: 50,
        rating: 4.4,
        reviewCount: 203,
        isActive: true,
        isFeatured: true,
        createdAt: new Date('2024-01-05'),
        updatedAt: new Date('2024-01-05')
      }
    ];

    this._products.set(sampleProducts);
  }

  // Getters computados
  getProductById(id: string): Product | undefined {
    return this._products().find(p => p.id === id);
  }

  getProductsByCategory(category: string): Product[] {
    return this._products().filter(p => p.category === category);
  }

  getProductsByTag(tag: string): Product[] {
    return this._products().filter(p => p.tags.includes(tag));
  }

  getProductsInPriceRange(min: number, max: number): Product[] {
    return this._products().filter(p => p.price >= min && p.price <= max);
  }

  // Métodos de utilidad
  hasStock(productId: string): boolean {
    const product = this.getProductById(productId);
    return product ? product.stock > 0 : false;
  }

  getStockCount(productId: string): number {
    const product = this.getProductById(productId);
    return product ? product.stock : 0;
  }

  isOnSale(productId: string): boolean {
    const product = this.getProductById(productId);
    return product ? !!product.originalPrice && product.originalPrice > product.price : false;
  }

  getDiscountPercentage(productId: string): number {
    const product = this.getProductById(productId);
    if (!product || !product.originalPrice) return 0;
    
    return Math.round(((product.originalPrice - product.price) / product.originalPrice) * 100);
  }
}
