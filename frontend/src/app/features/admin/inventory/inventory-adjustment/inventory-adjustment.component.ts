import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { InventoryService, AdjustStockCommand } from '../../../../core/services/inventory.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-inventory-adjustment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-4xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Ajustar Stock de Inventario
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Ajusta la cantidad de stock disponible para un producto
          </p>
        </div>
        
        <div class="flex items-center space-x-3">
          <button
            (click)="goBack()"
            class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors"
          >
            Cancelar
          </button>
          
          <button
            (click)="adjustStock()"
            [disabled]="adjustmentForm.invalid || isAdjusting"
            class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors rounded-md"
          >
            <svg *ngIf="isAdjusting" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ isAdjusting ? 'Ajustando...' : 'Ajustar Stock' }}
          </button>
        </div>
      </div>

      <!-- Error State -->
      @if (error()) {
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          <p><strong>Error:</strong> {{ error() }}</p>
        </div>
      }

      <!-- Formulario -->
      <form [formGroup]="adjustmentForm" (ngSubmit)="adjustStock()" class="space-y-6">
        <!-- Información del Producto -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Información del Producto</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Producto -->
            <div>
              <label for="productId" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Producto *
              </label>
              <select
                id="productId"
                formControlName="productId"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                [class.border-red-500]="isFieldInvalid('productId')"
                (change)="onProductChange($event)"
              >
                <option value="">Seleccionar producto</option>
                @for (product of products(); track product.id) {
                  <option [value]="product.id">{{ product.name }} ({{ product.sku }})</option>
                }
              </select>
              <div *ngIf="isFieldInvalid('productId')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('productId') }}
              </div>
            </div>

            <!-- Stock Actual -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Stock Actual
              </label>
              <div class="px-3 py-2 bg-gray-50 dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md">
                <span class="text-lg font-semibold" [class]="getCurrentStockClass()">
                  {{ getCurrentStock() }}
                </span>
                @if (getCurrentStock() <= getReorderPoint()) {
                  <span class="ml-2 text-xs bg-red-100 text-red-800 px-2 py-1 rounded-full">
                    Stock Bajo
                  </span>
                }
              </div>
            </div>
          </div>

          <!-- Información de Inventario -->
          @if (selectedProductInventory()) {
            <div class="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
              <div class="bg-blue-50 dark:bg-blue-900/20 p-3 rounded-lg">
                <div class="text-sm text-blue-600 dark:text-blue-400">Stock Mínimo</div>
                <div class="text-lg font-semibold text-blue-900 dark:text-blue-100">
                  {{ selectedProductInventory()!.minimumStock }}
                </div>
              </div>
              <div class="bg-yellow-50 dark:bg-yellow-900/20 p-3 rounded-lg">
                <div class="text-sm text-yellow-600 dark:text-yellow-400">Punto de Reorden</div>
                <div class="text-lg font-semibold text-yellow-900 dark:text-yellow-100">
                  {{ selectedProductInventory()!.reorderPoint }}
                </div>
              </div>
              <div class="bg-green-50 dark:bg-green-900/20 p-3 rounded-lg">
                <div class="text-sm text-green-600 dark:text-green-400">Stock Máximo</div>
                <div class="text-lg font-semibold text-green-900 dark:text-green-100">
                  {{ selectedProductInventory()!.maximumStock }}
                </div>
              </div>
            </div>
          }
        </div>

        <!-- Ajuste de Stock -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Ajuste de Stock</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Tipo de Ajuste -->
            <div>
              <label for="adjustmentType" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Tipo de Ajuste *
              </label>
              <select
                id="adjustmentType"
                formControlName="adjustmentType"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                [class.border-red-500]="isFieldInvalid('adjustmentType')"
              >
                <option value="">Seleccionar tipo</option>
                <option value="add">Agregar Stock</option>
                <option value="remove">Quitar Stock</option>
                <option value="set">Establecer Stock</option>
              </select>
              <div *ngIf="isFieldInvalid('adjustmentType')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('adjustmentType') }}
              </div>
            </div>

            <!-- Cantidad -->
            <div>
              <label for="quantity" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Cantidad *
              </label>
              <input
                id="quantity"
                type="number"
                formControlName="quantity"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="0"
                [class.border-red-500]="isFieldInvalid('quantity')"
              />
              <div *ngIf="isFieldInvalid('quantity')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('quantity') }}
              </div>
            </div>
          </div>

          <!-- Motivo -->
          <div class="mt-6">
            <label for="reason" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Motivo del Ajuste *
            </label>
            <select
              id="reason"
              formControlName="reason"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
              [class.border-red-500]="isFieldInvalid('reason')"
            >
              <option value="">Seleccionar motivo</option>
              <option value="receipt">Recepción de Mercancía</option>
              <option value="return">Devolución de Cliente</option>
              <option value="damage">Mercancía Dañada</option>
              <option value="theft">Pérdida por Robo</option>
              <option value="expired">Producto Vencido</option>
              <option value="audit">Auditoría de Inventario</option>
              <option value="transfer">Transferencia</option>
              <option value="other">Otro</option>
            </select>
            <div *ngIf="isFieldInvalid('reason')" class="mt-1 text-red-500 text-sm">
              {{ getFieldError('reason') }}
            </div>
          </div>

          <!-- Notas -->
          <div class="mt-6">
            <label for="notes" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Notas Adicionales
            </label>
            <textarea
              id="notes"
              formControlName="notes"
              rows="3"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
              placeholder="Detalles adicionales sobre el ajuste..."
            ></textarea>
          </div>

          <!-- Resumen del Ajuste -->
          @if (adjustmentForm.get('productId')?.value && adjustmentForm.get('adjustmentType')?.value && adjustmentForm.get('quantity')?.value) {
            <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Resumen del Ajuste</h4>
              <div class="text-sm text-gray-600 dark:text-gray-300">
                <p><strong>Stock Actual:</strong> {{ getCurrentStock() }}</p>
                <p><strong>Tipo:</strong> {{ getAdjustmentTypeText() }}</p>
                <p><strong>Cantidad:</strong> {{ adjustmentForm.get('quantity')?.value }}</p>
                <p><strong>Stock Final:</strong> 
                  <span [class]="getFinalStockClass()">{{ getFinalStock() }}</span>
                </p>
              </div>
            </div>
          }
        </div>
      </form>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class InventoryAdjustmentComponent implements OnInit {
  adjustmentForm: FormGroup;
  isAdjusting = false;
  selectedProductId: string | null = null;

  // Signals del store
  products = this.productStore.products;
  inventory = this.productStore.inventory;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;

  constructor(
    private fb: FormBuilder,
    private productStore: AdminProductStore,
    private inventoryService: InventoryService,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.adjustmentForm = this.fb.group({
      productId: ['', [Validators.required]],
      adjustmentType: ['', [Validators.required]],
      quantity: [0, [
        Validators.required,
        Validators.min(1),
        Validators.max(99999)
      ]],
      reason: ['', [Validators.required]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    // Cargar productos e inventario si no están cargados
    if (this.products().length === 0) {
      this.productStore.loadProducts();
    }
    if (this.inventory().length === 0) {
      this.productStore.loadInventory();
    }

    // Verificar si hay un productId en los query params
    this.route.queryParams.subscribe(params => {
      if (params['productId']) {
        this.selectedProductId = params['productId'];
        this.adjustmentForm.patchValue({
          productId: params['productId']
        });
      }
    });
  }

  onProductChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedProductId = target.value || null;
  }

  selectedProductInventory() {
    if (!this.selectedProductId) return null;
    return this.productStore.getProductInventory()(this.selectedProductId);
  }

  getCurrentStock(): number {
    const inventory = this.selectedProductInventory();
    return inventory ? inventory.availableQuantity : 0;
  }

  getReorderPoint(): number {
    const inventory = this.selectedProductInventory();
    return inventory ? inventory.reorderPoint : 0;
  }

  getCurrentStockClass(): string {
    const stock = this.getCurrentStock();
    const reorderPoint = this.getReorderPoint();
    
    if (stock === 0) return 'text-red-600';
    if (stock <= reorderPoint) return 'text-yellow-600';
    return 'text-green-600';
  }

  getAdjustmentTypeText(): string {
    const type = this.adjustmentForm.get('adjustmentType')?.value;
    switch (type) {
      case 'add': return 'Agregar Stock';
      case 'remove': return 'Quitar Stock';
      case 'set': return 'Establecer Stock';
      default: return '';
    }
  }

  getFinalStock(): number {
    const currentStock = this.getCurrentStock();
    const adjustmentType = this.adjustmentForm.get('adjustmentType')?.value;
    const quantity = this.adjustmentForm.get('quantity')?.value || 0;

    switch (adjustmentType) {
      case 'add': return currentStock + quantity;
      case 'remove': return Math.max(0, currentStock - quantity);
      case 'set': return quantity;
      default: return currentStock;
    }
  }

  getFinalStockClass(): string {
    const finalStock = this.getFinalStock();
    const reorderPoint = this.getReorderPoint();
    
    if (finalStock === 0) return 'text-red-600 font-semibold';
    if (finalStock <= reorderPoint) return 'text-yellow-600 font-semibold';
    return 'text-green-600 font-semibold';
  }

  adjustStock(): void {
    if (this.adjustmentForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isAdjusting = true;
    const formValue = this.adjustmentForm.value;

    const command: AdjustStockCommand = {
      productId: formValue.productId,
      quantity: formValue.quantity,
      reason: formValue.reason,
      notes: formValue.notes || undefined
    };

    this.inventoryService.adjustStock(command).subscribe({
      next: (response) => {
        if (response.success) {
          this.uiStore.showSuccess('Stock Ajustado', 'El stock del producto ha sido ajustado correctamente');
          this.productStore.loadInventory(); // Recargar inventario
          this.router.navigate(['/admin/products']);
        } else {
          this.uiStore.showError('Error', response.message || 'Error al ajustar el stock');
        }
      },
      error: (error) => {
        console.error('Error al ajustar stock:', error);
        this.uiStore.showError('Error', 'Ha ocurrido un error al ajustar el stock');
      },
      complete: () => {
        this.isAdjusting = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.adjustmentForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.adjustmentForm.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    const errors = field.errors;
    
    if (errors['required']) return `${this.getFieldLabel(fieldName)} es requerido`;
    if (errors['min']) return `${this.getFieldLabel(fieldName)} debe ser mayor a ${errors['min'].min}`;
    if (errors['max']) return `${this.getFieldLabel(fieldName)} no puede ser mayor a ${errors['max'].max}`;
    
    return 'Campo inválido';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      'productId': 'El producto',
      'adjustmentType': 'El tipo de ajuste',
      'quantity': 'La cantidad',
      'reason': 'El motivo'
    };
    return labels[fieldName] || 'Este campo';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.adjustmentForm.controls).forEach(key => {
      const control = this.adjustmentForm.get(key);
      control?.markAsTouched();
    });
  }
}
