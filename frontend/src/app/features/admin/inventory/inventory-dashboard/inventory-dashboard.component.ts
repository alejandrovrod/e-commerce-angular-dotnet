import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { InventoryService } from '../../../../core/services/inventory.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-inventory-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './inventory-dashboard.component.html',
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class InventoryDashboardComponent implements OnInit {
  // Signals del store
  products = this.productStore.products;
  categories = this.productStore.categories;
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
    // Cargar datos si no están cargados
    if (this.products().length === 0) {
      this.productStore.loadProducts();
    }
    if (this.categories().length === 0) {
      this.productStore.loadCategories();
    }
    if (this.inventory().length === 0) {
      this.productStore.loadInventory();
    }
  }

  getTotalProducts(): number {
    return this.products().length;
  }

  getTotalInventoryValue(): number {
    return this.products().reduce((total, product) => {
      const inventory = this.getProductInventoryData(product.id);
      const quantity = inventory?.availableQuantity || 0;
      return total + (product.price * quantity);
    }, 0);
  }

  getCategoriesWithCounts() {
    return this.categories().map(category => ({
      ...category,
      count: this.products().filter(product => product.categoryId === category.id).length
    }));
  }

  getTopProductsByValue() {
    return this.products()
      .map(product => ({
        ...product,
        totalValue: product.price * (this.getProductInventoryData(product.id)?.availableQuantity || 0)
      }))
      .sort((a, b) => b.totalValue - a.totalValue)
      .slice(0, 10);
  }

  getCriticalAlerts() {
    const criticalProducts = this.products().filter(product => {
      const inventory = this.getProductInventoryData(product.id);
      return inventory && inventory.availableQuantity <= inventory.reorderPoint;
    });

    return criticalProducts.map(product => ({
      id: product.id,
      name: product.name,
      stock: this.getProductInventoryData(product.id)?.availableQuantity || 0
    }));
  }

  getProductInventoryData(productId: string) {
    return this.getProductInventory()(productId);
  }

  getCategoryColor(categoryId: string): string {
    const colors = [
      '#3B82F6', '#EF4444', '#10B981', '#F59E0B', '#8B5CF6',
      '#EC4899', '#06B6D4', '#84CC16', '#F97316', '#6366F1'
    ];
    const index = this.categories().findIndex(cat => cat.id === categoryId);
    return colors[index % colors.length];
  }

  refreshDashboard(): void {
    this.productStore.loadProducts();
    this.productStore.loadCategories();
    this.productStore.loadInventory();
    this.uiStore.showSuccess('Actualizado', 'El dashboard ha sido actualizado');
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }
}
