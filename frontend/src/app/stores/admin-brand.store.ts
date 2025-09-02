import { Injectable, signal, computed, inject } from '@angular/core';
import { BrandService, BrandDto, CreateBrandCommand, UpdateBrandCommand, ApiResponse } from '../core/services/brand.service';
import { tap, catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

export interface AdminBrand {
  id: string;
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  // Campos adicionales para la UI
  slug?: string;
  isFeatured?: boolean;
  sortOrder?: number;
  productCount?: number;
  country?: string;
  foundedYear?: number;
}

export interface BrandFilters {
  isActive?: boolean;
  isFeatured?: boolean;
  country?: string;
  search?: string;
}

export interface AdminBrandState {
  brands: AdminBrand[];
  filteredBrands: AdminBrand[];
  isLoading: boolean;
  error: string | null;
  filters: BrandFilters;
  selectedBrand: AdminBrand | null;
}

@Injectable({
  providedIn: 'root'
})
export class AdminBrandStore {
  private readonly brandService = inject(BrandService);

  // Signals para el estado reactivo
  private readonly _brands = signal<AdminBrand[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _filters = signal<BrandFilters>({});
  private readonly _selectedBrand = signal<AdminBrand | null>(null);

  // Computed signals
  public readonly brands = this._brands.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly filters = this._filters.asReadonly();
  public readonly selectedBrand = this._selectedBrand.asReadonly();

  public readonly filteredBrands = computed(() => {
    const brands = this._brands();
    const filters = this._filters();

    return brands.filter(brand => {
      if (filters.isActive !== undefined && brand.isActive !== filters.isActive) {
        return false;
      }
      if (filters.isFeatured !== undefined && brand.isFeatured !== filters.isFeatured) {
        return false;
      }
      if (filters.country && brand.country !== filters.country) {
        return false;
      }
      if (filters.search) {
        const searchLower = filters.search.toLowerCase();
        return brand.name.toLowerCase().includes(searchLower) ||
               brand.description.toLowerCase().includes(searchLower) ||
               (brand.slug && brand.slug.toLowerCase().includes(searchLower)) ||
               (brand.country && brand.country.toLowerCase().includes(searchLower));
      }
      return true;
    });
  });

  public readonly activeBrands = computed(() => {
    return this._brands().filter(b => b.isActive);
  });

  public readonly featuredBrands = computed(() => {
    return this._brands().filter(b => b.isFeatured && b.isActive);
  });

  public readonly totalBrands = computed(() => this._brands().length);
  public readonly activeBrandsCount = computed(() => this.activeBrands().length);

  public readonly countries = computed(() => {
    const brands = this._brands();
    const uniqueCountries = new Set(brands.map(b => b.country).filter(Boolean));
    return Array.from(uniqueCountries).sort();
  });

  constructor() {
    // No cargar datos automáticamente, se cargarán cuando se necesiten
  }

  // Actions
  setBrands(brands: AdminBrand[]): void {
    this._brands.set(brands);
    this._error.set(null);
  }

  // Método para cargar marcas desde el backend
  loadBrands(): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.brandService.getBrands()
      .pipe(
        tap((response: ApiResponse<BrandDto[]>) => {
          if (response.success) {
            const adminBrands = response.data.map(dto => this.mapBrandDtoToAdminBrand(dto));
            this._brands.set(adminBrands);
          } else {
            this._error.set(response.message || 'Error al cargar marcas');
          }
        }),
        catchError((error) => {
          this._error.set('Error de conexión al cargar marcas');
          console.error('Error loading brands:', error);
          return of(null);
        }),
        finalize(() => {
          this._isLoading.set(false);
        })
      )
      .subscribe();
  }

  // Método para cargar una marca por ID
  loadBrandById(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.brandService.getBrandById(id)
      .pipe(
        tap((response: ApiResponse<BrandDto>) => {
          if (response.success) {
            const adminBrand = this.mapBrandDtoToAdminBrand(response.data);
            this._selectedBrand.set(adminBrand);
          } else {
            this._error.set(response.message || 'Error al cargar la marca');
          }
        }),
        catchError((error) => {
          this._error.set('Error de conexión al cargar la marca');
          console.error('Error loading brand:', error);
          return of(null);
        }),
        finalize(() => {
          this._isLoading.set(false);
        })
      )
      .subscribe();
  }

  // Crear nueva marca
  createBrand(command: CreateBrandCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.brandService.createBrand(command)
      .pipe(
        tap((response: ApiResponse<BrandDto>) => {
          if (response.success) {
            const adminBrand = this.mapBrandDtoToAdminBrand(response.data);
            this._brands.update(brands => [adminBrand, ...brands]);
          } else {
            this._error.set(response.message || 'Error al crear la marca');
          }
        }),
        catchError((error) => {
          this._error.set('Error de conexión al crear la marca');
          console.error('Error creating brand:', error);
          return of(null);
        }),
        finalize(() => {
          this._isLoading.set(false);
        })
      )
      .subscribe();
  }

  // Actualizar marca
  updateBrand(command: UpdateBrandCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.brandService.updateBrand(command)
      .pipe(
        tap((response: ApiResponse<BrandDto>) => {
          if (response.success) {
            const adminBrand = this.mapBrandDtoToAdminBrand(response.data);
            this._brands.update(brands =>
              brands.map(b => b.id === adminBrand.id ? adminBrand : b)
            );
          } else {
            this._error.set(response.message || 'Error al actualizar la marca');
          }
        }),
        catchError((error) => {
          this._error.set('Error de conexión al actualizar la marca');
          console.error('Error updating brand:', error);
          return of(null);
        }),
        finalize(() => {
          this._isLoading.set(false);
        })
      )
      .subscribe();
  }

  // Eliminar marca
  deleteBrand(brandId: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.brandService.deleteBrand(brandId)
      .pipe(
        tap((response: ApiResponse<boolean>) => {
          if (response.success) {
            this._brands.update(brands => brands.filter(b => b.id !== brandId));
          } else {
            this._error.set(response.message || 'Error al eliminar la marca');
          }
        }),
        catchError((error) => {
          this._error.set('Error de conexión al eliminar la marca');
          console.error('Error deleting brand:', error);
          return of(null);
        }),
        finalize(() => {
          this._isLoading.set(false);
        })
      )
      .subscribe();
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  setFilters(filters: Partial<BrandFilters>): void {
    this._filters.update(current => ({ ...current, ...filters }));
  }

  clearFilters(): void {
    this._filters.set({});
  }

  selectBrand(brand: AdminBrand | null): void {
    this._selectedBrand.set(brand);
  }

  // Métodos de utilidad
  getBrandById(id: string): AdminBrand | undefined {
    return this._brands().find(b => b.id === id);
  }

  getBrandBySlug(slug: string): AdminBrand | undefined {
    return this._brands().find(b => b.slug === slug);
  }

  toggleBrandStatus(brandId: string): void {
    this._brands.update(brands =>
      brands.map(b =>
        b.id === brandId ? { ...b, isActive: !b.isActive } : b
      )
    );
  }

  toggleFeaturedStatus(brandId: string): void {
    this._brands.update(brands =>
      brands.map(b =>
        b.id === brandId ? { ...b, isFeatured: !b.isFeatured } : b
      )
    );
  }

  // Método para mapear BrandDto a AdminBrand
  private mapBrandDtoToAdminBrand = (dto: BrandDto): AdminBrand => {
    return {
      id: dto.id,
      name: dto.name,
      description: dto.description,
      logoUrl: dto.logoUrl,
      website: dto.website,
      isActive: dto.isActive,
      createdAt: dto.createdAt,
      updatedAt: dto.updatedAt,
      // Campos adicionales para la UI
      slug: this.generateSlug(dto.name),
      isFeatured: false, // Por defecto no destacada
      sortOrder: 0,
      productCount: dto.products?.length || 0,
      country: undefined,
      foundedYear: undefined
    };
  }

  updateSortOrder(brandId: string, newSortOrder: number): void {
    this._brands.update(brands =>
      brands.map(b =>
        b.id === brandId ? { ...b, sortOrder: newSortOrder } : b
      )
    );
  }

  // Métodos para importar/exportar
  exportBrands(): AdminBrand[] {
    return this._brands();
  }

  importBrands(brands: AdminBrand[]): void {
    this._brands.set(brands);
  }

  // Métodos para estadísticas
  getBrandsByCountry(): Record<string, number> {
    const brands = this._brands();
    const result: Record<string, number> = {};
    brands.forEach(brand => {
      if (brand.country) {
        result[brand.country] = (result[brand.country] || 0) + 1;
      }
    });
    return result;
  }

  getBrandsByYear(): Record<number, number> {
    const brands = this._brands();
    const result: Record<number, number> = {};
    brands.forEach(brand => {
      if (brand.foundedYear) {
        result[brand.foundedYear] = (result[brand.foundedYear] || 0) + 1;
      }
    });
    return result;
  }

  getTopBrandsByProducts(limit: number = 10): AdminBrand[] {
    const brands = [...this._brands()];
    return brands
      .sort((a, b) => (b.productCount || 0) - (a.productCount || 0))
      .slice(0, limit);
  }

  // Métodos para validación
  validateBrand(brand: Partial<AdminBrand>): string[] {
    const errors: string[] = [];
    
    if (!brand.name || brand.name.trim().length === 0) {
      errors.push('El nombre de la marca es requerido');
    }
    
    if (!brand.description || brand.description.trim().length === 0) {
      errors.push('La descripción de la marca es requerida');
    }
    
    if (!brand.slug || brand.slug.trim().length === 0) {
      errors.push('El slug es requerido');
    }
    
    if (brand.slug && !/^[a-z0-9-]+$/.test(brand.slug)) {
      errors.push('El slug solo puede contener letras minúsculas, números y guiones');
    }
    
    if (brand.sortOrder !== undefined && brand.sortOrder < 0) {
      errors.push('El orden de clasificación no puede ser negativo');
    }
    
    if (brand.foundedYear && (brand.foundedYear < 1800 || brand.foundedYear > new Date().getFullYear())) {
      errors.push('El año de fundación debe ser válido');
    }
    
    if (brand.website && !this.isValidUrl(brand.website)) {
      errors.push('La URL del sitio web debe ser válida');
    }
    
    return errors;
  }

  // Métodos para duplicar marcas
  duplicateBrand(brandId: string): AdminBrand | null {
    const original = this.getBrandById(brandId);
    if (!original) return null;
    
    const duplicated: AdminBrand = {
      ...original,
      id: this.generateId(),
      name: `${original.name} (Copia)`,
      slug: `${original.slug}-copy`,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    
    // Agregar a la lista local (no al backend)
    this._brands.update(brands => [duplicated, ...brands]);
    return duplicated;
  }

  // Métodos para bulk actions
  bulkUpdateStatus(brandIds: string[], isActive: boolean): void {
    this._brands.update(brands =>
      brands.map(b =>
        brandIds.includes(b.id) ? { ...b, isActive } : b
      )
    );
  }

  bulkDeleteBrands(brandIds: string[]): void {
    this._brands.update(brands =>
      brands.filter(b => !brandIds.includes(b.id))
    );
  }

  bulkUpdateCountry(brandIds: string[], country: string): void {
    this._brands.update(brands =>
      brands.map(b =>
        brandIds.includes(b.id) ? { ...b, country } : b
      )
    );
  }

  // Métodos privados
  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  private generateSlug(name: string): string {
    return name
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/[^a-z0-9\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim();
  }

  private isValidUrl(url: string): boolean {
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }


}
