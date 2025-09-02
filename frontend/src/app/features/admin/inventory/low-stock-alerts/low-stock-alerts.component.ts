import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { InventoryService } from '../../../../core/services/inventory.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-low-stock-alerts',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="max-w-7xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Alertas de Stock Bajo
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Productos que requieren atención inmediata por stock bajo
          </p>
        </div>
        
        <div class="flex items-center space-x-3">
          <button
            (click)="refreshAlerts()"
            [disabled]="isLoading()"
            class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors disabled:opacity-50"
          >
            <svg *ngIf="isLoading()" class="animate-spin -ml-1 mr-2 h-4 w-4 text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Actualizar
          </button>
          
          <button
            (click)="goBack()"
            class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors"
          >
            Volver
          </button>
        </div>
      </div>

      <!-- Resumen de Alertas -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Stock Bajo -->
        <div class="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-8 w-8 text-yellow-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-medium text-yellow-800 dark:text-yellow-200">Stock Bajo</h3>
              <p class="text-2xl font-bold text-yellow-900 dark:text-yellow-100">{{ lowStockProducts().length }}</p>
              <p class="text-sm text-yellow-700 dark:text-yellow-300">Productos</p>
            </div>
          </div>
        </div>

        <!-- Sin Stock -->
        <div class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-8 w-8 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-medium text-red-800 dark:text-red-200">Sin Stock</h3>
              <p class="text-2xl font-bold text-red-900 dark:text-red-100">{{ outOfStockProducts().length }}</p>
              <p class="text-sm text-red-700 dark:text-red-300">Productos</p>
            </div>
          </div>
        </div>

        <!-- Total Alertas -->
        <div class="bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-800 rounded-lg p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-8 w-8 text-orange-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM4.828 7l2.586 2.586a2 2 0 002.828 0L12.828 7H4.828z" />
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-medium text-orange-800 dark:text-orange-200">Total Alertas</h3>
              <p class="text-2xl font-bold text-orange-900 dark:text-orange-100">{{ getTotalAlerts() }}</p>
              <p class="text-sm text-orange-700 dark:text-orange-300">Requieren Atención</p>
            </div>
          </div>
        </div>
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

      <!-- Lista de Alertas -->
      @if (!isLoading() && !error()) {
        @if (getTotalAlerts() === 0) {
          <div class="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-8 text-center">
            <svg class="mx-auto h-12 w-12 text-green-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <h3 class="text-lg font-medium text-green-800 dark:text-green-200 mb-2">¡Excelente!</h3>
            <p class="text-green-700 dark:text-green-300">No hay productos con stock bajo en este momento.</p>
          </div>
        } @else {
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h2 class="text-xl font-semibold text-gray-900 dark:text-white">
                Productos con Alertas ({{ getTotalAlerts() }})
              </h2>
            </div>
            
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Producto
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      SKU
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Stock Actual
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Punto de Reorden
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Estado
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                      Acciones
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  @for (product of getProductsWithAlerts(); track product.id) {
                    <tr class="hover:bg-gray-50 dark:hover:bg-gray-700">
                      <td class="px-6 py-4 whitespace-nowrap">
                        <div class="text-sm font-medium text-gray-900 dark:text-white">
                          {{ product.name }}
                        </div>
                        <div class="text-sm text-gray-500 dark:text-gray-400">
                          {{ product.brand }}
                        </div>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ product.sku }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                                                 <span [class]="getStockClass(getProductInventoryData(product.id)?.availableQuantity || 0)">
                           {{ getProductInventoryData(product.id)?.availableQuantity || 0 }}
                         </span>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                        {{ getProductInventoryData(product.id)?.reorderPoint || 0 }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <span [class]="getAlertClass(getProductInventoryData(product.id)?.availableQuantity || 0, getProductInventoryData(product.id)?.reorderPoint || 0)">
                          {{ getAlertText(getProductInventoryData(product.id)?.availableQuantity || 0, getProductInventoryData(product.id)?.reorderPoint || 0) }}
                        </span>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                        <div class="flex space-x-2">
                          <a [routerLink]="['/admin/inventory/adjust']" 
                             [queryParams]="{productId: product.id}"
                             class="text-blue-600 hover:text-blue-900">
                            Ajustar Stock
                          </a>
                          <a [routerLink]="['/admin/products/edit', product.id]" 
                             class="text-indigo-600 hover:text-indigo-900">
                            Editar
                          </a>
                        </div>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class LowStockAlertsComponent implements OnInit {
  // Signals del store
  products = this.productStore.products;
  inventory = this.productStore.inventory;
  lowStockProducts = this.productStore.lowStockProducts;
  outOfStockProducts = this.productStore.outOfStockProducts;
  getProductInventory = this.productStore.getProductInventory;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;

  constructor(
    private productStore: AdminProductStore,
    private inventoryService: InventoryService,
    private uiStore: UIStore,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Cargar productos e inventario si no están cargados
    if (this.products().length === 0) {
      this.productStore.loadProducts();
    }
    if (this.inventory().length === 0) {
      this.productStore.loadInventory();
    }
  }

  getTotalAlerts(): number {
    return this.lowStockProducts().length + this.outOfStockProducts().length;
  }

  getProductsWithAlerts() {
    const lowStockProductIds = this.lowStockProducts().map(inv => inv.productId);
    const outOfStockProductIds = this.outOfStockProducts().map(inv => inv.productId);
    const allAlertProductIds = [...new Set([...lowStockProductIds, ...outOfStockProductIds])];
    
    return this.products().filter(product => allAlertProductIds.includes(product.id));
  }

  getStockClass(quantity: number): string {
    if (quantity === 0) return 'text-red-600 font-semibold';
    if (quantity <= 10) return 'text-yellow-600 font-semibold';
    return 'text-green-600';
  }

  getAlertClass(quantity: number, reorderPoint: number): string {
    if (quantity === 0) {
      return 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800';
    } else if (quantity <= reorderPoint) {
      return 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800';
    }
    return 'inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800';
  }

  getAlertText(quantity: number, reorderPoint: number): string {
    if (quantity === 0) return 'Sin Stock';
    if (quantity <= reorderPoint) return 'Stock Bajo';
    return 'Normal';
  }

  getProductInventoryData(productId: string) {
    return this.getProductInventory()(productId);
  }

  refreshAlerts(): void {
    this.productStore.loadInventory();
    this.uiStore.showSuccess('Actualizado', 'Las alertas de stock han sido actualizadas');
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }
}
