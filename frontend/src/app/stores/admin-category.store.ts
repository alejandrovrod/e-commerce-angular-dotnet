import { Injectable, signal, computed, inject } from '@angular/core';
import { CategoryService, CategoryDto, CreateCategoryCommand, UpdateCategoryCommand, ApiResponse } from '../core/services/category.service';
import { catchError, tap, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

export interface AdminCategory {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  parentCategoryId?: string;
  createdAt: Date;
  updatedAt: Date;
  // Campos adicionales para la UI
  slug?: string;
  image?: string;
  isFeatured?: boolean;
  sortOrder?: number;
  productCount?: number;
}

export interface CategoryFilters {
  isActive?: boolean;
  isFeatured?: boolean;
  parentCategoryId?: string;
  search?: string;
}

export interface AdminCategoryState {
  categories: AdminCategory[];
  filteredCategories: AdminCategory[];
  isLoading: boolean;
  error: string | null;
  filters: CategoryFilters;
  selectedCategory: AdminCategory | null;
}

@Injectable({
  providedIn: 'root'
})
export class AdminCategoryStore {
  private readonly categoryService = inject(CategoryService);

  // Signals para el estado reactivo
  private readonly _categories = signal<AdminCategory[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _filters = signal<CategoryFilters>({});
  private readonly _selectedCategory = signal<AdminCategory | null>(null);

  // Computed signals
  public readonly categories = this._categories.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly filters = this._filters.asReadonly();
  public readonly selectedCategory = this._selectedCategory.asReadonly();

  public readonly filteredCategories = computed(() => {
    const categories = this._categories();
    const filters = this._filters();

    return categories.filter(category => {
      if (filters.isActive !== undefined && category.isActive !== filters.isActive) {
        return false;
      }
      if (filters.isFeatured !== undefined && category.isFeatured !== filters.isFeatured) {
        return false;
      }
      if (filters.parentCategoryId && category.parentCategoryId !== filters.parentCategoryId) {
        return false;
      }
      if (filters.search) {
        const searchLower = filters.search.toLowerCase();
        return category.name.toLowerCase().includes(searchLower) ||
               category.description.toLowerCase().includes(searchLower) ||
               (category.slug && category.slug.toLowerCase().includes(searchLower));
      }
      return true;
    });
  });

  public readonly activeCategories = computed(() => {
    return this._categories().filter(c => c.isActive);
  });

  public readonly featuredCategories = computed(() => {
    return this._categories().filter(c => c.isFeatured && c.isActive);
  });

  public readonly parentCategories = computed(() => {
    return this._categories().filter(c => !c.parentCategoryId);
  });

  public readonly totalCategories = computed(() => this._categories().length);
  public readonly activeCategoriesCount = computed(() => this.activeCategories().length);

  constructor() {
    // No cargar datos automáticamente al inicializar
    // Los datos se cargarán solo cuando se navegue a las pantallas correspondientes
  }

  // Actions
  loadCategories(): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoryService.getCategories()
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            const categories = response.data.map(this.mapCategoryDtoToAdminCategory);
            this._categories.set(categories);
          } else {
            this._error.set(response.message || 'Error al cargar categorías');
          }
        }),
        catchError(error => {
          console.error('Error loading categories:', error);
          this._error.set('Error de conexión al cargar categorías');
          return of(null);
        }),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe();
  }

  loadCategoryById(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoryService.getCategoryById(id)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            const category = this.mapCategoryDtoToAdminCategory(response.data);
            this._selectedCategory.set(category);
          } else {
            this._error.set(response.message || 'Error al cargar categoría');
          }
        }),
        catchError(error => {
          console.error('Error loading category:', error);
          this._error.set('Error de conexión al cargar categoría');
          return of(null);
        }),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe();
  }

  setCategories(categories: AdminCategory[]): void {
    this._categories.set(categories);
    this._error.set(null);
  }

  createCategory(command: CreateCategoryCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoryService.createCategory(command)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            const category = this.mapCategoryDtoToAdminCategory(response.data);
            this._categories.update(categories => [category, ...categories]);
          } else {
            this._error.set(response.message || 'Error al crear categoría');
          }
        }),
        catchError(error => {
          console.error('Error creating category:', error);
          this._error.set('Error de conexión al crear categoría');
          return of(null);
        }),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe();
  }

  updateCategory(id: string, command: UpdateCategoryCommand): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoryService.updateCategory(id, command)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            const updatedCategory = this.mapCategoryDtoToAdminCategory(response.data);
            this._categories.update(categories =>
              categories.map(c => c.id === updatedCategory.id ? updatedCategory : c)
            );
          } else {
            this._error.set(response.message || 'Error al actualizar categoría');
          }
        }),
        catchError(error => {
          console.error('Error updating category:', error);
          this._error.set('Error de conexión al actualizar categoría');
          return of(null);
        }),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe();
  }

  deleteCategory(categoryId: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoryService.deleteCategory(categoryId)
      .pipe(
        tap(response => {
          if (response.success) {
            this._categories.update(categories => categories.filter(c => c.id !== categoryId));
          } else {
            this._error.set(response.message || 'Error al eliminar categoría');
          }
        }),
        catchError(error => {
          console.error('Error deleting category:', error);
          this._error.set('Error de conexión al eliminar categoría');
          return of(null);
        }),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe();
  }

  addCategory(category: AdminCategory): void {
    this._categories.update(categories => [category, ...categories]);
    this._error.set(null);
  }

  updateCategoryLocal(updatedCategory: AdminCategory): void {
    this._categories.update(categories =>
      categories.map(c => c.id === updatedCategory.id ? updatedCategory : c)
    );
    this._error.set(null);
  }

  deleteCategoryLocal(categoryId: string): void {
    this._categories.update(categories => categories.filter(c => c.id !== categoryId));
    this._error.set(null);
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  setFilters(filters: Partial<CategoryFilters>): void {
    this._filters.update(current => ({ ...current, ...filters }));
  }

  clearFilters(): void {
    this._filters.set({});
  }

  selectCategory(category: AdminCategory | null): void {
    this._selectedCategory.set(category);
  }

  // Métodos de utilidad
  getCategoryById(id: string): AdminCategory | undefined {
    return this._categories().find(c => c.id === id);
  }

  getCategoryBySlug(slug: string): AdminCategory | undefined {
    return this._categories().find(c => c.slug === slug);
  }

  toggleCategoryStatus(categoryId: string): void {
    this._categories.update(categories =>
      categories.map(c =>
        c.id === categoryId ? { ...c, isActive: !c.isActive } : c
      )
    );
  }

  toggleFeaturedStatus(categoryId: string): void {
    this._categories.update(categories =>
      categories.map(c =>
        c.id === categoryId ? { ...c, isFeatured: !c.isFeatured } : c
      )
    );
  }

  updateSortOrder(categoryId: string, newSortOrder: number): void {
    this._categories.update(categories =>
      categories.map(c =>
        c.id === categoryId ? { ...c, sortOrder: newSortOrder } : c
      )
    );
  }

  // Métodos para importar/exportar
  exportCategories(): AdminCategory[] {
    return this._categories();
  }

  importCategories(categories: AdminCategory[]): void {
    this._categories.set(categories);
  }

  // Métodos para estadísticas
  getCategoriesByParent(): Record<string, number> {
    const categories = this._categories();
    const result: Record<string, number> = {};
    categories.forEach(category => {
      const parentId = category.parentCategoryId || 'root';
      result[parentId] = (result[parentId] || 0) + 1;
    });
    return result;
  }

  getCategoryHierarchy(): AdminCategory[] {
    const categories = [...this._categories()];
    return categories.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0));
  }

  getSubcategories(parentId: string): AdminCategory[] {
    return this._categories().filter(c => c.parentCategoryId === parentId);
  }

  // Métodos para validación
  validateCategory(category: Partial<AdminCategory>): string[] {
    const errors: string[] = [];
    
    if (!category.name || category.name.trim().length === 0) {
      errors.push('El nombre de la categoría es requerido');
    }
    
    if (!category.description || category.description.trim().length === 0) {
      errors.push('La descripción de la categoría es requerida');
    }
    
    if (!category.slug || category.slug.trim().length === 0) {
      errors.push('El slug es requerido');
    }
    
    if (category.slug && !/^[a-z0-9-]+$/.test(category.slug)) {
      errors.push('El slug solo puede contener letras minúsculas, números y guiones');
    }
    
    if (category.sortOrder !== undefined && category.sortOrder < 0) {
      errors.push('El orden de clasificación no puede ser negativo');
    }
    
    return errors;
  }

  // Métodos para duplicar categorías
  duplicateCategory(categoryId: string): AdminCategory | null {
    const original = this.getCategoryById(categoryId);
    if (!original) return null;
    
    const duplicated: AdminCategory = {
      ...original,
      id: this.generateId(),
      name: `${original.name} (Copia)`,
      slug: `${original.slug || this.generateSlug(original.name)}-copy`,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    
    this.addCategory(duplicated);
    return duplicated;
  }

  // Métodos para bulk actions
  bulkUpdateStatus(categoryIds: string[], isActive: boolean): void {
    this._categories.update(categories =>
      categories.map(c =>
        categoryIds.includes(c.id) ? { ...c, isActive } : c
      )
    );
  }

  bulkDeleteCategories(categoryIds: string[]): void {
    this._categories.update(categories =>
      categories.filter(c => !categoryIds.includes(c.id))
    );
  }

  bulkUpdateParent(categoryIds: string[], parentCategoryId: string | undefined): void {
    this._categories.update(categories =>
      categories.map(c =>
        categoryIds.includes(c.id) ? { ...c, parentCategoryId } : c
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

  private mapCategoryDtoToAdminCategory = (dto: CategoryDto): AdminCategory => {
    return {
      id: dto.id,
      name: dto.name,
      description: dto.description,
      isActive: dto.isActive,
      parentCategoryId: dto.parentCategoryId,
      createdAt: dto.createdAt,
      updatedAt: dto.updatedAt,
      // Campos adicionales para la UI
      slug: this.generateSlug(dto.name),
      image: undefined,
      isFeatured: false,
      sortOrder: 0,
      productCount: 0
    };
  }
}
