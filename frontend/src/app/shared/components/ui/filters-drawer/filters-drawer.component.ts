import { Component, Input, Output, EventEmitter, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface FilterOption {
  value: string | number | boolean;
  label: string;
}

export interface FilterConfig {
  key: string;
  label: string;
  type: 'text' | 'select' | 'number' | 'range' | 'checkbox';
  options?: FilterOption[];
  placeholder?: string;
  min?: number;
  max?: number;
}

@Component({
  selector: 'app-filters-drawer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <!-- Overlay -->
    <div 
      *ngIf="isOpen()" 
      class="fixed inset-0 bg-black bg-opacity-50 z-40 transition-opacity duration-300"
      (click)="close()"
    ></div>

    <!-- Drawer -->
    <div 
      class="fixed top-0 right-0 h-full w-96 bg-white dark:bg-gray-800 shadow-xl z-50 transform transition-transform duration-300 ease-in-out"
      [class.translate-x-0]="isOpen()"
      [class.translate-x-full]="!isOpen()"
    >
      <!-- Header -->
      <div class="flex items-center justify-between p-6 border-b border-gray-200 dark:border-gray-700">
        <div>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Filtros</h2>
          <p class="text-sm text-gray-500 dark:text-gray-400">Filtra productos por criterios específicos</p>
        </div>
        <button
          (click)="close()"
          class="p-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Content -->
      <div class="flex-1 overflow-y-auto p-6">
        <form class="space-y-6">
          <!-- Búsqueda -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Búsqueda
            </label>
            <div class="relative">
              <input
                type="text"
                [(ngModel)]="filters.search"
                (ngModelChange)="onFilterChange()"
                name="search"
                placeholder="Nombre, SKU, descripción..."
                class="block w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
              />
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                </svg>
              </div>
            </div>
          </div>

          <!-- Categoría -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Categoría
            </label>
            <select
              [(ngModel)]="filters.category"
              (ngModelChange)="onFilterChange()"
              name="category"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todas las categorías</option>
              <option *ngFor="let category of categories" [value]="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>

          <!-- Marca -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Marca
            </label>
            <select
              [(ngModel)]="filters.brand"
              (ngModelChange)="onFilterChange()"
              name="brand"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todas las marcas</option>
              <option *ngFor="let brand of brands" [value]="brand.id">
                {{ brand.name }}
              </option>
            </select>
          </div>

          <!-- Rango de Precios -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Rango de Precios
            </label>
            <div class="grid grid-cols-2 gap-3">
              <div>
                <input
                  type="number"
                  [(ngModel)]="filters.minPrice"
                  (ngModelChange)="onFilterChange()"
                  name="minPrice"
                  placeholder="Mínimo"
                  min="0"
                  step="0.01"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
                />
              </div>
              <div>
                <input
                  type="number"
                  [(ngModel)]="filters.maxPrice"
                  (ngModelChange)="onFilterChange()"
                  name="maxPrice"
                  placeholder="Máximo"
                  min="0"
                  step="0.01"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
                />
              </div>
            </div>
          </div>

          <!-- Estado -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Estado
            </label>
            <select
              [(ngModel)]="filters.isActive"
              (ngModelChange)="onFilterChange()"
              name="isActive"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todos los estados</option>
              <option [value]="true">Activos</option>
              <option [value]="false">Inactivos</option>
            </select>
          </div>

          <!-- Ordenamiento -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Ordenar por
            </label>
            <div class="grid grid-cols-2 gap-3">
              <select
                [(ngModel)]="filters.sortBy"
                (ngModelChange)="onFilterChange()"
                name="sortBy"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Seleccionar campo</option>
                <option value="name">Nombre</option>
                <option value="price">Precio</option>
                <option value="createdAt">Fecha de creación</option>
                <option value="updatedAt">Fecha de actualización</option>
              </select>
              <select
                [(ngModel)]="filters.sortDirection"
                (ngModelChange)="onFilterChange()"
                name="sortDirection"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="asc">Ascendente</option>
                <option value="desc">Descendente</option>
              </select>
            </div>
          </div>

          <!-- Filtros adicionales -->
          <div class="space-y-3">
            <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">Filtros adicionales</h3>
            
            <label class="flex items-center">
              <input
                type="checkbox"
                [(ngModel)]="filters.isFeatured"
                (ngModelChange)="onFilterChange()"
                name="isFeatured"
                class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Solo destacados</span>
            </label>

            <label class="flex items-center">
              <input
                type="checkbox"
                [(ngModel)]="filters.inStock"
                (ngModelChange)="onFilterChange()"
                name="inStock"
                class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Solo con stock</span>
            </label>
          </div>
        </form>
      </div>

      <!-- Footer -->
      <div class="p-6 border-t border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-700">
        <div class="flex space-x-3">
          <button
            (click)="clearFilters()"
            class="flex-1 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-600 border border-gray-300 dark:border-gray-500 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-500 transition-colors"
          >
            Limpiar filtros
          </button>
          <button
            (click)="applyFilters()"
            class="flex-1 px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-lg transition-colors"
          >
            Aplicar filtros
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class FiltersDrawerComponent {
  @Input() isOpen = signal(false);
  @Input() categories: any[] = [];
  @Input() brands: any[] = [];
  @Input() initialFilters: any = {};

  @Output() filtersChange = new EventEmitter<any>();
  @Output() drawerClose = new EventEmitter<void>();

  filters = {
    search: '',
    category: '',
    brand: '',
    minPrice: null as number | null,
    maxPrice: null as number | null,
    isActive: '',
    isFeatured: false,
    inStock: false,
    sortBy: '',
    sortDirection: 'asc' as 'asc' | 'desc'
  };

  ngOnInit() {
    // Inicializar filtros con valores por defecto
    this.filters = { ...this.filters, ...this.initialFilters };
  }

  onFilterChange() {
    // Emitir cambios en tiempo real (opcional)
    // this.filtersChange.emit(this.filters);
  }

  applyFilters() {
    // Limpiar valores vacíos
    const cleanFilters = Object.fromEntries(
      Object.entries(this.filters).filter(([_, value]) => 
        value !== '' && value !== null && value !== undefined
      )
    );

    this.filtersChange.emit(cleanFilters);
    this.close();
  }

  clearFilters() {
    this.filters = {
      search: '',
      category: '',
      brand: '',
      minPrice: null,
      maxPrice: null,
      isActive: '',
      isFeatured: false,
      inStock: false,
      sortBy: '',
      sortDirection: 'asc'
    };
    this.filtersChange.emit({});
  }

  close() {
    this.isOpen.set(false);
    this.drawerClose.emit();
  }
}
