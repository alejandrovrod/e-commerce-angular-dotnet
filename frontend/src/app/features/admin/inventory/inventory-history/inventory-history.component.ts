import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { InventoryService, InventoryMovementDto, InventoryHistoryFilters } from '../../../../core/services/inventory.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-inventory-history',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Historial de Movimientos de Inventario
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Registro de todos los movimientos de stock en el sistema
          </p>
        </div>
        
        <div class="flex items-center space-x-3">
          <button
            (click)="goBack()"
            class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors"
          >
            Volver
          </button>
          
          <button
            (click)="exportHistory()"
            [disabled]="isLoading()"
            class="px-4 py-2 text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors rounded-md"
          >
            Exportar
          </button>
        </div>
      </div>

      <!-- Filtros -->
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Filtros</h3>
        
        <form [formGroup]="filtersForm" (ngSubmit)="applyFilters()" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
          <!-- Producto -->
          <div>
            <label for="productId" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Producto
            </label>
            <select
              id="productId"
              formControlName="productId"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todos los productos</option>
              @for (product of products(); track product.id) {
                <option [value]="product.id">{{ product.name }} ({{ product.sku }})</option>
              }
            </select>
          </div>

          <!-- Tipo de Movimiento -->
          <div>
            <label for="movementType" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Tipo de Movimiento
            </label>
            <select
              id="movementType"
              formControlName="movementType"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todos los tipos</option>
              <option value="adjustment">Ajuste</option>
              <option value="reservation">Reserva</option>
              <option value="release">Liberación</option>
              <option value="sale">Venta</option>
              <option value="return">Devolución</option>
            </select>
          </div>

          <!-- Fecha Desde -->
          <div>
            <label for="dateFrom" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Fecha Desde
            </label>
            <input
              id="dateFrom"
              type="date"
              formControlName="dateFrom"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <!-- Fecha Hasta -->
          <div>
            <label for="dateTo" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Fecha Hasta
            </label>
            <input
              id="dateTo"
              type="date"
              formControlName="dateTo"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <!-- Botones -->
          <div class="flex items-end space-x-2">
            <button
              type="submit"
              class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-md transition-colors"
            >
              Filtrar
            </button>
            <button
              type="button"
              (click)="clearFilters()"
              class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors"
            >
              Limpiar
            </button>
          </div>
        </form>
      </div>

      <!-- Estado de Carga -->
      @if (isLoading()) {
        <div class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
        </div>
      }

      <!-- Error State -->
      @if (error()) {
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          <p><strong>Error:</strong> {{ error() }}</p>
        </div>
      }

      <!-- Historial -->
      @if (!isLoading() && !error()) {
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h2 class="text-xl font-semibold text-gray-900 dark:text-white">
              Movimientos ({{ movements().length }})
            </h2>
          </div>
          
          @if (movements().length === 0) {
            <div class="text-center py-12">
              <p class="text-gray-500 dark:text-gray-400">No hay movimientos que coincidan con los filtros</p>
            </div>
          } @else {
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Fecha
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Producto
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Tipo
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Cantidad
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Stock Anterior
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Stock Nuevo
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Motivo
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Usuario
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  @for (movement of movements(); track movement.id) {
                    <tr class="hover:bg-gray-50 dark:hover:bg-gray-700">
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ formatDate(movement.createdAt) }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ movement.productName || 'Producto no encontrado' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <span [class]="getMovementTypeClass(movement.movementType)">
                          {{ getMovementTypeText(movement.movementType) }}
                        </span>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        <span [class]="getQuantityClass(movement.quantity, movement.movementType)">
                          {{ getQuantityText(movement.quantity, movement.movementType) }}
                        </span>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ movement.previousQuantity }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ movement.newQuantity }}
                      </td>
                      <td class="px-6 py-4 text-sm text-gray-900 dark:text-white">
                        {{ movement.reason }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ movement.userName || 'Sistema' }}
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class InventoryHistoryComponent implements OnInit {
  filtersForm: FormGroup;
  movements = this.productStore.inventoryMovements;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;
  products = this.productStore.products;

  constructor(
    private fb: FormBuilder,
    private productStore: AdminProductStore,
    private inventoryService: InventoryService,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.filtersForm = this.fb.group({
      productId: [''],
      movementType: [''],
      dateFrom: [''],
      dateTo: ['']
    });
  }

  ngOnInit(): void {
    // Cargar productos si no están cargados
    if (this.products().length === 0) {
      this.productStore.loadProducts();
    }

    // Cargar historial inicial
    this.loadHistory();

    // Verificar si hay un productId en los query params
    this.route.queryParams.subscribe(params => {
      if (params['productId']) {
        this.filtersForm.patchValue({
          productId: params['productId']
        });
        this.applyFilters();
      }
    });
  }

  loadHistory(filters?: InventoryHistoryFilters): void {
    this.inventoryService.getInventoryHistory(filters).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.productStore.setInventoryMovements(response.data);
        } else {
          this.productStore.setError(response.message || 'Error al cargar el historial');
        }
      },
      error: (error) => {
        console.error('Error al cargar historial:', error);
        this.productStore.setError('Error al cargar el historial de inventario');
      }
    });
  }

  applyFilters(): void {
    const formValue = this.filtersForm.value;
    const filters: InventoryHistoryFilters = {};

    if (formValue.productId) filters.productId = formValue.productId;
    if (formValue.movementType) filters.movementType = formValue.movementType;
    if (formValue.dateFrom) filters.dateFrom = new Date(formValue.dateFrom);
    if (formValue.dateTo) filters.dateTo = new Date(formValue.dateTo);

    this.loadHistory(filters);
  }

  clearFilters(): void {
    this.filtersForm.reset();
    this.loadHistory();
  }

  exportHistory(): void {
    // TODO: Implementar exportación a CSV/Excel
    this.uiStore.showSuccess('Exportación', 'Funcionalidad de exportación en desarrollo');
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleString('es-ES', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getMovementTypeText(type: string): string {
    const types: { [key: string]: string } = {
      'adjustment': 'Ajuste',
      'reservation': 'Reserva',
      'release': 'Liberación',
      'sale': 'Venta',
      'return': 'Devolución'
    };
    return types[type] || type;
  }

  getMovementTypeClass(type: string): string {
    const classes: { [key: string]: string } = {
      'adjustment': 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 text-blue-800',
      'reservation': 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800',
      'release': 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800',
      'sale': 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-purple-100 text-purple-800',
      'return': 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-gray-100 text-gray-800'
    };
    return classes[type] || 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-gray-100 text-gray-800';
  }

  getQuantityText(quantity: number, type: string): string {
    if (type === 'adjustment' || type === 'sale' || type === 'return') {
      return quantity > 0 ? `+${quantity}` : `${quantity}`;
    }
    return `${quantity}`;
  }

  getQuantityClass(quantity: number, type: string): string {
    if (type === 'adjustment' || type === 'sale' || type === 'return') {
      return quantity > 0 ? 'text-green-600 font-semibold' : 'text-red-600 font-semibold';
    }
    return 'text-gray-900';
  }
}
