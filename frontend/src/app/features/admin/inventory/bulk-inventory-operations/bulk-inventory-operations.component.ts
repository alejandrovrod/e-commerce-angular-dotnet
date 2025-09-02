import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { InventoryService, AdjustStockCommand } from '../../../../core/services/inventory.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-bulk-inventory-operations',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './bulk-inventory-operations.component.html',
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class BulkInventoryOperationsComponent implements OnInit {
  bulkForm: FormGroup;
  isProcessing = false;
  selectedProducts: string[] = [];

  // Signals del store
  products = this.productStore.products;
  getProductInventory = this.productStore.getProductInventory;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;

  constructor(
    private fb: FormBuilder,
    private productStore: AdminProductStore,
    private inventoryService: InventoryService,
    private uiStore: UIStore,
    private router: Router
  ) {
    this.bulkForm = this.fb.group({
      operationType: ['adjust', Validators.required],
      quantity: [0, [Validators.required, Validators.min(1)]],
      reason: ['', Validators.required],
      notes: ['']
    });
  }

  ngOnInit(): void {
    // Cargar datos si no están cargados
    if (this.products().length === 0) {
      this.productStore.loadProducts();
    }
  }

  getProductInventoryData(productId: string) {
    return this.getProductInventory()(productId);
  }

  getStockClass(quantity: number): string {
    if (quantity === 0) {
      return 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-200';
    } else if (quantity <= 10) {
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-200';
    } else {
      return 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-200';
    }
  }

  isProductSelected(productId: string): boolean {
    return this.selectedProducts.includes(productId);
  }

  toggleProductSelection(productId: string): void {
    const index = this.selectedProducts.indexOf(productId);
    if (index > -1) {
      this.selectedProducts.splice(index, 1);
    } else {
      this.selectedProducts.push(productId);
    }
  }

  selectAll(): void {
    this.selectedProducts = this.products().map(product => product.id);
  }

  deselectAll(): void {
    this.selectedProducts = [];
  }

  onSubmit(): void {
    if (this.bulkForm.valid && this.selectedProducts.length > 0) {
      this.executeBulkOperation();
    }
  }

  executeBulkOperation(): void {
    if (!this.bulkForm.valid || this.selectedProducts.length === 0) {
      return;
    }

    this.isProcessing = true;
    const formValue = this.bulkForm.value;
    
    // Ejecutar operación para cada producto seleccionado
    const operations = this.selectedProducts.map(productId => {
      const command: AdjustStockCommand = {
        productId,
        quantity: formValue.quantity,
        reason: formValue.reason,
        notes: formValue.notes
      };

      return this.inventoryService.adjustStock(command).subscribe({
        next: (response) => {
          console.log(`Stock ajustado para producto ${productId}:`, response);
        },
        error: (error) => {
          console.error(`Error ajustando stock para producto ${productId}:`, error);
        }
      });
    });

    // Esperar a que todas las operaciones terminen
    Promise.all(operations).then(() => {
      this.isProcessing = false;
      this.uiStore.showSuccess('Operación Completada', 'El stock ha sido ajustado para todos los productos seleccionados');
      this.productStore.loadProducts(); // Recargar productos
      this.selectedProducts = []; // Limpiar selección
      this.bulkForm.reset();
    }).catch(() => {
      this.isProcessing = false;
      this.uiStore.showError('Error', 'Hubo un error al procesar algunas operaciones');
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }
}