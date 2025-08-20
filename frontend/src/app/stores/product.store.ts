import { Injectable, computed, signal } from '@angular/core';
import { Product, ProductCategory, ProductFilter, ProductSort } from '../core/models/product.model';

export interface ProductState {
  products: Product[];
  categories: ProductCategory[];
  featuredProducts: Product[];
  selectedProduct: Product | null;
  filters: ProductFilter;
  sort: ProductSort;
  isLoading: boolean;
  error: string | null;
  pagination: {
    currentPage: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  };
  searchQuery: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductStore {
  // Private state signals
  private readonly _products = signal<Product[]>([]);
  private readonly _categories = signal<ProductCategory[]>([]);
  private readonly _featuredProducts = signal<Product[]>([]);
  private readonly _selectedProduct = signal<Product | null>(null);
  private readonly _filters = signal<ProductFilter>({});
  private readonly _sort = signal<ProductSort>({ field: 'name', direction: 'asc' });
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _pagination = signal({
    currentPage: 1,
    pageSize: 12,
    totalItems: 0,
    totalPages: 0
  });
  private readonly _searchQuery = signal<string>('');

  // Public readonly signals
  readonly products = this._products.asReadonly();
  readonly categories = this._categories.asReadonly();
  readonly featuredProducts = this._featuredProducts.asReadonly();
  readonly selectedProduct = this._selectedProduct.asReadonly();
  readonly filters = this._filters.asReadonly();
  readonly sort = this._sort.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly pagination = this._pagination.asReadonly();
  readonly searchQuery = this._searchQuery.asReadonly();

  // Computed signals
  readonly filteredProducts = computed(() => {
    let filtered = [...this._products()];
    const filters = this._filters();
    const searchQuery = this._searchQuery().toLowerCase();

    // Apply search filter
    if (searchQuery) {
      filtered = filtered.filter(product => 
        product.name.toLowerCase().includes(searchQuery) ||
        product.description.toLowerCase().includes(searchQuery) ||
        product.category.name.toLowerCase().includes(searchQuery)
      );
    }

    // Apply category filter
    if (filters.categoryId) {
      filtered = filtered.filter(product => product.category.id === filters.categoryId);
    }

    // Apply price range filter
    if (filters.minPrice !== undefined) {
      filtered = filtered.filter(product => product.price >= filters.minPrice!);
    }
    if (filters.maxPrice !== undefined) {
      filtered = filtered.filter(product => product.price <= filters.maxPrice!);
    }

    // Apply rating filter
    if (filters.minRating !== undefined) {
      filtered = filtered.filter(product => product.rating >= filters.minRating!);
    }

    // Apply availability filter
    if (filters.inStock !== undefined) {
      filtered = filtered.filter(product => 
        filters.inStock ? product.stock > 0 : product.stock === 0
      );
    }

    // Apply brand filter
    if (filters.brands && filters.brands.length > 0) {
      filtered = filtered.filter(product => 
        filters.brands!.includes(product.brand)
      );
    }

    return filtered;
  });

  readonly sortedProducts = computed(() => {
    const products = [...this.filteredProducts()];
    const sort = this._sort();

    return products.sort((a, b) => {
      let aValue: any = a[sort.field as keyof Product];
      let bValue: any = b[sort.field as keyof Product];

      // Handle nested properties
      if (sort.field === 'category.name') {
        aValue = a.category.name;
        bValue = b.category.name;
      }

      // Convert to comparable values
      if (typeof aValue === 'string') {
        aValue = aValue.toLowerCase();
        bValue = bValue.toLowerCase();
      }

      let comparison = 0;
      if (aValue < bValue) comparison = -1;
      if (aValue > bValue) comparison = 1;

      return sort.direction === 'desc' ? -comparison : comparison;
    });
  });

  readonly paginatedProducts = computed(() => {
    const products = this.sortedProducts();
    const pagination = this._pagination();
    const startIndex = (pagination.currentPage - 1) * pagination.pageSize;
    const endIndex = startIndex + pagination.pageSize;
    
    return products.slice(startIndex, endIndex);
  });

  readonly availableBrands = computed(() => {
    const brands = new Set(this._products().map(product => product.brand));
    return Array.from(brands).sort();
  });

  readonly priceRange = computed(() => {
    const products = this._products();
    if (products.length === 0) return { min: 0, max: 0 };
    
    const prices = products.map(product => product.price);
    return {
      min: Math.min(...prices),
      max: Math.max(...prices)
    };
  });

  readonly hasProducts = computed(() => this._products().length > 0);
  readonly hasFilters = computed(() => {
    const filters = this._filters();
    return !!(filters.categoryId || filters.minPrice !== undefined || 
             filters.maxPrice !== undefined || filters.minRating !== undefined ||
             filters.inStock !== undefined || (filters.brands && filters.brands.length > 0));
  });

  // State management methods
  setProducts(products: Product[]): void {
    this._products.set(products);
    this.updatePaginationTotal(this.filteredProducts().length);
    this._error.set(null);
  }

  setCategories(categories: ProductCategory[]): void {
    this._categories.set(categories);
  }

  setFeaturedProducts(products: Product[]): void {
    this._featuredProducts.set(products);
  }

  setSelectedProduct(product: Product | null): void {
    this._selectedProduct.set(product);
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
    this._isLoading.set(false);
  }

  setSearchQuery(query: string): void {
    this._searchQuery.set(query);
    this.resetPagination();
  }

  setFilters(filters: Partial<ProductFilter>): void {
    this._filters.update(current => ({ ...current, ...filters }));
    this.resetPagination();
  }

  clearFilters(): void {
    this._filters.set({});
    this._searchQuery.set('');
    this.resetPagination();
  }

  setSort(sort: ProductSort): void {
    this._sort.set(sort);
  }

  setPagination(pagination: Partial<typeof this._pagination.value>): void {
    this._pagination.update(current => ({ ...current, ...pagination }));
  }

  // Pagination methods
  nextPage(): void {
    const current = this._pagination();
    if (current.currentPage < current.totalPages) {
      this.setPagination({ currentPage: current.currentPage + 1 });
    }
  }

  previousPage(): void {
    const current = this._pagination();
    if (current.currentPage > 1) {
      this.setPagination({ currentPage: current.currentPage - 1 });
    }
  }

  goToPage(page: number): void {
    const current = this._pagination();
    if (page >= 1 && page <= current.totalPages) {
      this.setPagination({ currentPage: page });
    }
  }

  resetPagination(): void {
    this.setPagination({ currentPage: 1 });
  }

  // Utility methods
  getProductById(id: string): Product | undefined {
    return this._products().find(product => product.id === id);
  }

  getCategoryById(id: string): ProductCategory | undefined {
    return this._categories().find(category => category.id === id);
  }

  addProduct(product: Product): void {
    this._products.update(products => [...products, product]);
  }

  updateProduct(id: string, updates: Partial<Product>): void {
    this._products.update(products => 
      products.map(product => 
        product.id === id ? { ...product, ...updates } : product
      )
    );
  }

  removeProduct(id: string): void {
    this._products.update(products => 
      products.filter(product => product.id !== id)
    );
  }

  // Private methods
  private updatePaginationTotal(totalItems: number): void {
    const pageSize = this._pagination().pageSize;
    const totalPages = Math.ceil(totalItems / pageSize);
    this.setPagination({ totalItems, totalPages });
  }

  // Reset store
  reset(): void {
    this._products.set([]);
    this._categories.set([]);
    this._featuredProducts.set([]);
    this._selectedProduct.set(null);
    this._filters.set({});
    this._sort.set({ field: 'name', direction: 'asc' });
    this._isLoading.set(false);
    this._error.set(null);
    this._pagination.set({
      currentPage: 1,
      pageSize: 12,
      totalItems: 0,
      totalPages: 0
    });
    this._searchQuery.set('');
  }
}
