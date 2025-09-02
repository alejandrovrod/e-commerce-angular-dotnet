import { Injectable, signal, computed, effect, inject } from '@angular/core';
import { ProductService, ProductDto, CreateProductCommand, ProductFilters, ProductStatus, ApiResponse } from '../core/services/product.service';
import { CategoryService, CategoryDto } from '../core/services/category.service';
import { BrandService, BrandDto } from '../core/services/brand.service';
import { InventoryService, InventoryDto, InventoryMovementDto } from '../core/services/inventory.service';
import { catchError, tap, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

export interface AdminProductState {
  products: ProductDto[];
  filteredProducts: ProductDto[];
  categories: string[];
  brands: string[];
  isLoading: boolean;
  error: string | null;
  filters: ProductFilters;
  selectedProduct: ProductDto | null;
  pagination: {
    currentPage: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AdminProductStore {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);
  private readonly inventoryService = inject(InventoryService);

  // Signals para el estado reactivo
  private readonly _products = signal<ProductDto[]>([]);
  private readonly _categories = signal<CategoryDto[]>([]);
  private readonly _brands = signal<BrandDto[]>([]);
  private readonly _inventory = signal<InventoryDto[]>([]);
  private readonly _inventoryMovements = signal<InventoryMovementDto[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _filters = signal<ProductFilters>({});
  private readonly _selectedProduct = signal<ProductDto | null>(null);
  private readonly _pagination = signal({
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  });

  // Computed signals
  public readonly products = this._products.asReadonly();
  public readonly categories = this._categories.asReadonly();
  public readonly brands = this._brands.asReadonly();
  public readonly inventory = this._inventory.asReadonly();
  public readonly inventoryMovements = this._inventoryMovements.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly filters = this._filters.asReadonly();
  public readonly selectedProduct = this._selectedProduct.asReadonly();
  public readonly pagination = this._pagination.asReadonly();

  public readonly filteredProducts = computed(() => {
    const products = this._products();
    const filters = this._filters();

    return products.filter(product => {
      if (filters.category && product.categoryId !== filters.category) {
        return false;
      }
      if (filters.brand && product.brand !== filters.brand) {
        return false;
      }
      if (filters.minPrice && product.price < filters.minPrice) {
        return false;
      }
      if (filters.maxPrice && product.price > filters.maxPrice) {
        return false;
      }
      if (filters.isActive !== undefined && product.status !== (filters.isActive ? ProductStatus.Active : ProductStatus.Inactive)) {
        return false;
      }
      if (filters.isFeatured !== undefined && product.isFeatured !== filters.isFeatured) {
        return false;
      }
      if (filters.search) {
        const searchLower = filters.search.toLowerCase();
        return product.name.toLowerCase().includes(searchLower) ||
               product.description.toLowerCase().includes(searchLower) ||
               product.sku.toLowerCase().includes(searchLower);
      }
      return true;
    });
  });

  public readonly hasProducts = computed(() => this._products().length > 0);
  public readonly productsCount = computed(() => this._products().length);
  public readonly activeProductsCount = computed(() => 
    this._products().filter(p => p.status === ProductStatus.Active).length
  );

  // Computed signals para inventario
  public readonly lowStockProducts = computed(() => {
    return this._inventory().filter(inv => 
      inv.availableQuantity <= inv.reorderPoint
    );
  });

  public readonly outOfStockProducts = computed(() => {
    return this._inventory().filter(inv => inv.availableQuantity === 0);
  });

  public readonly getProductInventory = computed(() => {
    return (productId: string) => {
      return this._inventory().find(inv => inv.productId === productId);
    };
  });

  constructor() {
    // No cargar datos automáticamente al inicializar
    // Los datos se cargarán solo cuando se navegue a las pantallas correspondientes
  }

  // Actions para productos
  loadProducts(filters: ProductFilters = {}): void {
    this._isLoading.set(true);
    this._error.set(null);
    this._filters.set(filters);

    this.productService.getProducts(filters).pipe(
      tap((response: ApiResponse<ProductDto[]>) => {
        if (response.success && response.data) {
          this._products.set(response.data);
          this.updatePagination(response.data.length);
        } else {
          this._error.set(response.message || 'Error al cargar productos');
        }
      }),
      catchError((error) => {
        this._error.set('Error de conexión: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  loadProductById(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.getProductById(id).pipe(
      tap((response: ApiResponse<ProductDto>) => {
        if (response.success && response.data) {
          this._selectedProduct.set(response.data);
        } else {
          this._error.set(response.message || 'Producto no encontrado');
        }
      }),
      catchError((error) => {
        this._error.set('Error al cargar producto: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  createProduct(command: CreateProductCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.createProduct(command).pipe(
      tap((response: ApiResponse<ProductDto>) => {
        if (response.success && response.data) {
          this._products.update(products => [response.data!, ...products]);
          this._selectedProduct.set(response.data);
          this.updatePagination(this._products().length);
        } else {
          this._error.set(response.message || 'Error al crear producto');
        }
      }),
      catchError((error) => {
        this._error.set('Error al crear producto: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  updateProduct(id: string, command: CreateProductCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.updateProduct(id, command).pipe(
      tap((response: ApiResponse<ProductDto>) => {
        if (response.success && response.data) {
          this._products.update(products => 
            products.map(p => p.id === id ? response.data! : p)
          );
          this._selectedProduct.set(response.data);
        } else {
          this._error.set(response.message || 'Error al actualizar producto');
        }
      }),
      catchError((error) => {
        this._error.set('Error al actualizar producto: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  deleteProduct(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.deleteProduct(id).pipe(
      tap((response: ApiResponse<boolean>) => {
        if (response.success) {
          this._products.update(products => products.filter(p => p.id !== id));
          if (this._selectedProduct()?.id === id) {
            this._selectedProduct.set(null);
          }
          this.updatePagination(this._products().length);
        } else {
          this._error.set(response.message || 'Error al eliminar producto');
        }
      }),
      catchError((error) => {
        this._error.set('Error al eliminar producto: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  updateProductStatus(id: string, status: ProductStatus): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.updateProductStatus(id, status).pipe(
      tap((response: ApiResponse<boolean>) => {
        if (response.success) {
          this._products.update(products => 
            products.map(p => p.id === id ? { ...p, status } : p)
          );
          if (this._selectedProduct()?.id === id) {
            this._selectedProduct.update(p => p ? { ...p, status } : null);
          }
        } else {
          this._error.set(response.message || 'Error al actualizar estado');
        }
      }),
      catchError((error) => {
        this._error.set('Error al actualizar estado: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  toggleProductFeatured(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productService.toggleProductFeatured(id).pipe(
      tap((response: ApiResponse<boolean>) => {
        if (response.success) {
          this._products.update(products => 
            products.map(p => p.id === id ? { ...p, isFeatured: !p.isFeatured } : p)
          );
          if (this._selectedProduct()?.id === id) {
            this._selectedProduct.update(p => p ? { ...p, isFeatured: !p.isFeatured } : null);
          }
        } else {
          this._error.set(response.message || 'Error al cambiar destacado');
        }
      }),
      catchError((error) => {
        this._error.set('Error al cambiar destacado: ' + error.message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false))
    ).subscribe();
  }

  // Actions para filtros y paginación
  setFilters(filters: ProductFilters): void {
    this._filters.set(filters);
    this._pagination.update(p => ({ ...p, currentPage: 1 }));
    this.loadProducts(filters);
  }

  setPage(page: number): void {
    this._pagination.update(p => ({ ...p, currentPage: page }));
    const currentFilters = this._filters();
    this.loadProducts({ ...currentFilters, page });
  }

  setPageSize(pageSize: number): void {
    this._pagination.update(p => ({ ...p, pageSize, currentPage: 1 }));
    const currentFilters = this._filters();
    this.loadProducts({ ...currentFilters, pageSize, page: 1 });
  }

  // Actions para estado local
  setSelectedProduct(product: ProductDto | null): void {
    this._selectedProduct.set(product);
  }

  clearError(): void {
    this._error.set(null);
  }

  // Helpers privados
  private updatePagination(totalItems: number): void {
    const pageSize = this._pagination().pageSize;
    const totalPages = Math.ceil(totalItems / pageSize);
    
    this._pagination.update(p => ({
      ...p,
      totalItems,
      totalPages
    }));
  }

  // Actions para categorías y marcas
  loadCategories(): void {
    this.categoryService.getCategories().pipe(
      tap((response: ApiResponse<CategoryDto[]>) => {
        if (response.success && response.data) {
          this._categories.set(response.data);
        } else {
          console.error('Error al cargar categorías:', response.message);
        }
      }),
      catchError((error) => {
        console.error('Error al cargar categorías:', error);
        return of(null);
      })
    ).subscribe();
  }

  loadBrands(): void {
    this.brandService.getBrands().pipe(
      tap((response: ApiResponse<BrandDto[]>) => {
        if (response.success && response.data) {
          this._brands.set(response.data);
        } else {
          console.error('Error al cargar marcas:', response.message);
        }
      }),
      catchError((error) => {
        console.error('Error al cargar marcas:', error);
        return of(null);
      })
    ).subscribe();
  }

  loadInventory(): void {
    this.inventoryService.getInventory().pipe(
      tap((response: ApiResponse<InventoryDto[]>) => {
        if (response.success && response.data) {
          this._inventory.set(response.data);
        } else {
          console.error('Error al cargar inventario:', response.message);
        }
      }),
      catchError((error) => {
        console.error('Error al cargar inventario:', error);
        return of(null);
      })
    ).subscribe();
  }

  // Actions para historial de inventario
  setInventoryMovements(movements: InventoryMovementDto[]): void {
    this._inventoryMovements.set(movements);
  }

  clearFilters(): void {
    this._filters.set({
      search: undefined,
      category: undefined,
      brand: undefined,
      minPrice: undefined,
      maxPrice: undefined,
      inStock: undefined,
      isActive: undefined,
      isFeatured: undefined,
      page: 1,
      pageSize: 10,
      sortBy: undefined,
      sortDirection: 'asc'
    });
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  // Debug
  debugProducts(): void {
    this.productService.debugProducts().pipe(
      tap((response) => {
        console.log('Debug Products:', response);
      }),
      catchError((error) => {
        console.error('Debug Error:', error);
        return of(null);
      })
    ).subscribe();
  }
}
