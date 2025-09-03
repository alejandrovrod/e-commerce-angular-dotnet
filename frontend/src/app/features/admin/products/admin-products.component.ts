import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminProductStore } from 'src/app/stores/admin-product.store';
import { ProductDto, ProductStatus, ProductFilters } from 'src/app/core/services/product.service';
import { FiltersDrawerComponent } from 'src/app/shared/components/ui/filters-drawer/filters-drawer.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, FiltersDrawerComponent],
  templateUrl: './admin-products.component.html',
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class AdminProductsComponent implements OnInit {
  private readonly productStore = inject(AdminProductStore);

  // Signals del store
  products = this.productStore.products;
  filteredProducts = this.productStore.filteredProducts;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;
  categories = this.productStore.categories;
  brands = this.productStore.brands;
  pagination = this.productStore.pagination;
  getProductInventory = this.productStore.getProductInventory;

  // Variables de filtros
  searchTerm = '';
  selectedCategory = '';
  selectedBrand = '';
  selectedStatus = '';

  // Drawer de filtros
  filtersDrawerOpen = signal(false);
  currentFilters: ProductFilters = {};

  constructor() {}

  ngOnInit(): void {
    console.log('🔍 AdminProductsComponent: ngOnInit - Componente cargado con store');
    // Cargar datos solo cuando se navega a este componente
    this.productStore.loadProducts();
    this.productStore.loadCategories();
    this.productStore.loadBrands();
    this.productStore.loadInventory();
  }

  onDeleteProduct(id: string): void {
    if (confirm('¿Estás seguro de que quieres eliminar este producto?')) {
      this.productStore.deleteProduct(id);
    }
  }

  onToggleFeatured(id: string): void {
    this.productStore.toggleProductFeatured(id);
  }

  onUpdateStatus(id: string, status: ProductStatus): void {
    this.productStore.updateProductStatus(id, status);
  }

  getStatusText(status: ProductStatus): string {
    switch (status) {
      case ProductStatus.Draft: return 'Borrador';
      case ProductStatus.Active: return 'Activo';
      case ProductStatus.Inactive: return 'Inactivo';
      case ProductStatus.OutOfStock: return 'Sin Stock';
      case ProductStatus.Discontinued: return 'Descontinuado';
      default: return 'Desconocido';
    }
  }

  getStatusClass(status: ProductStatus): string {
    switch (status) {
      case ProductStatus.Active: return 'bg-green-100 text-green-800';
      case ProductStatus.Inactive: return 'bg-gray-100 text-gray-800';
      case ProductStatus.OutOfStock: return 'bg-red-100 text-red-800';
      case ProductStatus.Draft: return 'bg-yellow-100 text-yellow-800';
      case ProductStatus.Discontinued: return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  // Métodos de filtrado
  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    const searchTerm = target.value;
    
    const currentFilters = this.productStore.filters();
    this.productStore.setFilters({
      ...currentFilters,
      search: searchTerm || undefined
    });
  }

  onCategoryFilterChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const categoryId = target.value;
    
    const currentFilters = this.productStore.filters();
    this.productStore.setFilters({
      ...currentFilters,
      category: categoryId || undefined
    });
  }

  onBrandFilterChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const brand = target.value;
    
    const currentFilters = this.productStore.filters();
    this.productStore.setFilters({
      ...currentFilters,
      brand: brand || undefined
    });
  }

  onStatusFilterChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const status = target.value;
    
    const currentFilters = this.productStore.filters();
    this.productStore.setFilters({
      ...currentFilters,
      isActive: status ? status === '2' : undefined
    });
  }

  clearFilters(): void {
    this.productStore.setFilters({});
    
    // Limpiar los campos del formulario
    const searchInput = document.getElementById('search') as HTMLInputElement;
    const categorySelect = document.getElementById('categoryFilter') as HTMLSelectElement;
    const brandSelect = document.getElementById('brandFilter') as HTMLSelectElement;
    const statusSelect = document.getElementById('statusFilter') as HTMLSelectElement;
    
    if (searchInput) searchInput.value = '';
    if (categorySelect) categorySelect.value = '';
    if (brandSelect) brandSelect.value = '';
    if (statusSelect) statusSelect.value = '';
  }

  // Métodos de paginación
  onPageChange(page: number): void {
    this.productStore.setPage(page);
  }

  getPageNumbers(): number[] {
    const currentPage = this.pagination().currentPage;
    const totalPages = this.pagination().totalPages;
    const pages: number[] = [];
    
    // Mostrar máximo 5 páginas
    const maxVisiblePages = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
    
    // Ajustar si estamos cerca del final
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  // Métodos para inventario
  getProductInventoryData(productId: string) {
    return this.getProductInventory()(productId);
  }

  getStockStatusClass(availableQuantity: number, reorderPoint: number): string {
    if (availableQuantity === 0) {
      return 'text-red-600 font-semibold';
    } else if (availableQuantity <= reorderPoint) {
      return 'text-yellow-600 font-semibold';
    } else {
      return 'text-green-600';
    }
  }

  getStatusBadgeClass(status: number): string {
    switch (status) {
      case 1: // Borrador
        return 'bg-gray-100 text-gray-800';
      case 2: // Activo
        return 'bg-green-100 text-green-800';
      case 3: // Inactivo
        return 'bg-red-100 text-red-800';
      case 4: // Sin Stock
        return 'bg-yellow-100 text-yellow-800';
      case 5: // Descontinuado
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusBadgeStyle(status: number): any {
    if (!this.isDarkMode) return {};
    
    switch (status) {
      case 1: // Borrador
        return { 'background-color': '#4b5563', 'color': '#ffffff' };
      case 2: // Activo
        return { 'background-color': '#16a34a', 'color': '#ffffff' };
      case 3: // Inactivo
        return { 'background-color': '#dc2626', 'color': '#ffffff' };
      case 4: // Sin Stock
        return { 'background-color': '#d97706', 'color': '#ffffff' };
      case 5: // Descontinuado
        return { 'background-color': '#4b5563', 'color': '#ffffff' };
      default:
        return { 'background-color': '#4b5563', 'color': '#ffffff' };
    }
  }

  // Exponer Math para el template
  Math = Math;

  // Verificar si está en modo oscuro
  get isDarkMode(): boolean {
    return document.documentElement.classList.contains('dark');
  }

  // Métodos del drawer de filtros
  openFiltersDrawer(): void {
    this.filtersDrawerOpen.set(true);
  }

  closeFiltersDrawer(): void {
    this.filtersDrawerOpen.set(false);
  }

  onFiltersChange(filters: ProductFilters): void {
    this.currentFilters = filters;
    
    // Aplicar filtros al store
    this.productStore.setFilters(filters);
    
    // Actualizar variables locales para mostrar en la UI
    this.searchTerm = filters.search || '';
    this.selectedCategory = filters.category || '';
    this.selectedBrand = filters.brand || '';
    this.selectedStatus = filters.isActive !== undefined ? filters.isActive.toString() : '';
  }

  // Métodos para filtros activos
  hasActiveFilters(): boolean {
    return !!(this.searchTerm || this.selectedCategory || this.selectedBrand || this.selectedStatus);
  }

  getActiveFiltersCount(): number {
    let count = 0;
    if (this.searchTerm) count++;
    if (this.selectedCategory) count++;
    if (this.selectedBrand) count++;
    if (this.selectedStatus) count++;
    return count;
  }

  // Métodos para limpiar filtros individuales
  clearSearch(): void {
    this.searchTerm = '';
    this.onSearchChange({ target: { value: '' } } as any);
  }

  clearCategory(): void {
    this.selectedCategory = '';
    this.onCategoryFilterChange({ target: { value: '' } } as any);
  }

  clearBrand(): void {
    this.selectedBrand = '';
    this.onBrandFilterChange({ target: { value: '' } } as any);
  }

  clearStatus(): void {
    this.selectedStatus = '';
    this.onStatusFilterChange({ target: { value: '' } } as any);
  }

  clearAllFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = '';
    this.selectedBrand = '';
    this.selectedStatus = '';
    this.currentFilters = {};
    this.productStore.clearFilters();
  }

  // Métodos helper para mostrar nombres
  getCategoryName(categoryId: string): string {
    const category = this.categories().find(c => c.id === categoryId);
    return category ? category.name : categoryId;
  }

  getBrandName(brandName: string): string {
    return brandName;
  }

  getStatusName(status: string): string {
    switch (status) {
      case '1': return 'Borrador';
      case '2': return 'Activo';
      case '3': return 'Inactivo';
      case '4': return 'Sin Stock';
      case '5': return 'Descontinuado';
      default: return status;
    }
  }

  // Método para exportar productos
  exportProducts(): void {
    const products = this.filteredProducts();
    if (products.length === 0) {
      alert('No hay productos para exportar');
      return;
    }

    const csvContent = this.convertToCSV(products);
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `productos_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  private convertToCSV(products: ProductDto[]): string {
    const headers = ['ID', 'Nombre', 'SKU', 'Descripción', 'Precio', 'Marca', 'Categoría', 'Estado', 'Destacado', 'Stock'];
    const rows = products.map(product => [
      product.id,
      product.name,
      product.sku,
      product.description,
      product.price,
      product.brand,
      this.getCategoryName(product.categoryId),
      this.getStatusText(product.status),
      product.isFeatured ? 'Sí' : 'No',
      this.getProductInventory()(product.id)?.availableQuantity || 0
    ]);

    return [headers, ...rows]
      .map(row => row.map(cell => `"${cell}"`).join(','))
      .join('\n');
  }
}