import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminBrandStore, AdminBrand } from '../../../stores/admin-brand.store';
import { UIStore } from '../../../stores/ui.store';

@Component({
  selector: 'app-admin-brands',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-brands.component.html'
})
export class AdminBrandsComponent implements OnInit, OnDestroy {
  // Variables de búsqueda
  searchTerm = '';

  constructor(
    public brandStore: AdminBrandStore,
    private uiStore: UIStore
  ) {}

  ngOnInit(): void {
    // Cargar marcas desde el backend
    this.brandStore.loadBrands();
  }

  ngOnDestroy(): void {
    // Cleanup si es necesario
  }

  // Métodos de búsqueda
  onSearchChange(event: any): void {
    const searchTerm = event.target.value;
    this.brandStore.setFilters({ search: searchTerm || undefined });
  }

  // Computed para marcas filtradas
  filteredBrands() {
    return this.brandStore.filteredBrands();
  }

  toggleFeatured(brandId: string): void {
    this.brandStore.toggleFeaturedStatus(brandId);
    const brand = this.brandStore.getBrandById(brandId);
    if (brand) {
      this.uiStore.showSuccess(
        'Marca actualizada', 
        `${brand.name} ha sido ${brand.isFeatured ? 'destacada' : 'removida de destacados'}`
      );
    }
  }

  deleteBrand(brand: AdminBrand): void {
    if (confirm(`¿Estás seguro de que quieres eliminar la marca "${brand.name}"?`)) {
      this.brandStore.deleteBrand(brand.id);
      this.uiStore.showSuccess('Marca eliminada', `${brand.name} ha sido eliminada correctamente`);
    }
  }

  exportToCSV(): void {
    const brands = this.brandStore.brands();
    if (brands.length === 0) {
      this.uiStore.showWarning('Sin datos', 'No hay marcas para exportar');
      return;
    }

    const headers = ['ID', 'Nombre', 'Slug', 'Descripción', 'País', 'Año', 'Estado', 'Destacada', 'Productos', 'Orden'];
    const csvContent = [
      headers.join(','),
      ...brands.map((b: AdminBrand) => [
        b.id,
        `"${b.name}"`,
        b.slug,
        `"${b.description}"`,
        b.country || '',
        b.foundedYear || '',
        b.isActive ? 'Activo' : 'Inactivo',
        b.isFeatured ? 'Sí' : 'No',
        b.productCount,
        b.sortOrder
      ].join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `marcas_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    this.uiStore.showSuccess('Exportación completada', 'El archivo CSV ha sido descargado');
  }

  getActiveBrandsCount(): number {
    return this.brandStore.brands().filter((brand: AdminBrand) => brand.isActive).length;
  }

  getFeaturedBrandsCount(): number {
    return this.brandStore.brands().filter((brand: AdminBrand) => brand.isFeatured).length;
  }

  getTotalProducts(): number {
    const brands = this.brandStore.brands();
    return brands.reduce((total: number, brand: AdminBrand) => total + (brand.productCount || 0), 0);
  }

  getBrandsByCountry(): Array<{name: string, count: number}> {
    const brands = this.brandStore.brands();
    const countryMap = new Map<string, number>();
    
    brands.forEach((brand: AdminBrand) => {
      if (brand.country) {
        countryMap.set(brand.country, (countryMap.get(brand.country) || 0) + 1);
      }
    });
    
    return Array.from(countryMap.entries())
      .map(([name, count]) => ({ name, count }))
      .sort((a, b) => b.count - a.count);
  }

  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }
}
