import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { DataTableComponent, TableColumn, TableAction } from '../../../shared/components/ui/data-table/data-table.component';
import { AdminCategoryStore, AdminCategory } from '../../../stores/admin-category.store';
import { UIStore } from '../../../stores/ui.store';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-categories.component.html',
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class AdminCategoriesComponent implements OnInit {
  
  // Configuración de la tabla
  tableColumns: TableColumn[] = [
    {
      prop: 'image',
      name: 'Imagen',
      width: 16
    },
    {
      prop: 'name',
      name: 'Nombre',
      sortable: true
    },
    {
      prop: 'slug',
      name: 'Slug',
      sortable: true
    },
    {
      prop: 'description',
      name: 'Descripción',
      sortable: true
    },
    {
      prop: 'productCount',
      name: 'Productos',
      sortable: true
    },
    {
      prop: 'sortOrder',
      name: 'Orden',
      sortable: true
    },
    {
      prop: 'isActive',
      name: 'Estado'
    },
    {
      prop: 'isFeatured',
      name: 'Destacada'
    },
    {
      prop: 'createdAt',
      name: 'Creada',
      sortable: true
    }
  ];

  // Acciones de la tabla
  tableActions: TableAction[] = [
    {
      label: 'Ver',
      action: 'view',
      icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z',
      class: 'text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300'
    },
    {
      label: 'Editar',
      action: 'edit',
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      class: 'text-green-600 hover:text-blue-900 dark:text-green-400 dark:hover:text-blue-300'
    },
    {
      label: 'Duplicar',
      action: 'duplicate',
      icon: 'M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z',
      class: 'text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300'
    },
    {
      label: 'Eliminar',
      action: 'delete',
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      class: 'text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300'
    }
  ];

  constructor(
    public categoryStore: AdminCategoryStore,
    private uiStore: UIStore,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Cargar datos del backend
    this.categoryStore.loadCategories();
  }

  // Manejadores de eventos
  handleTableAction(event: { action: string; item: any }): void {
    const { action, item } = event;

    switch (action) {
      case 'create':
        this.router.navigate(['/admin/categories/create']);
        break;
      case 'view':
        this.viewCategory(item);
        break;
      case 'edit':
        this.editCategory(item);
        break;
      case 'duplicate':
        this.duplicateCategory(item);
        break;
      case 'delete':
        this.deleteCategory(item);
        break;
      default:
        console.log('Acción no implementada:', action);
    }
  }

  // Métodos de acción
  viewCategory(category: AdminCategory): void {
    this.uiStore.showInfo('Ver Categoría', `Viendo detalles de ${category.name}`);
    // Aquí implementarías la navegación al detalle de la categoría
    // Por ahora solo mostramos la información en un modal o alert
  }

  editCategory(category: AdminCategory): void {
    this.router.navigate(['/admin/categories/edit', category.id]);
  }

  duplicateCategory(category: AdminCategory): void {
    const duplicated = this.categoryStore.duplicateCategory(category.id);
    if (duplicated) {
      this.uiStore.showSuccess('Categoría duplicada', `${duplicated.name} ha sido duplicada correctamente`);
    } else {
      this.uiStore.showError('Error', 'No se pudo duplicar la categoría');
    }
  }

  deleteCategory(category: AdminCategory): void {
    if (category.productCount && category.productCount > 0) {
      this.uiStore.showError('Error', `No se puede eliminar la categoría "${category.name}" porque tiene productos asociados`);
      return;
    }

    if (confirm(`¿Estás seguro de que quieres eliminar "${category.name}"?`)) {
      this.categoryStore.deleteCategory(category.id);
      this.uiStore.showSuccess('Categoría eliminada', `${category.name} ha sido eliminada correctamente`);
    }
  }

  // Métodos de filtros
  onFilterChange(filterKey: string, event: any): void {
    let value: any = event.target?.value;
    
    if (filterKey === 'isActive' || filterKey === 'isFeatured') {
      if (value === '') {
        value = undefined;
      } else {
        value = value === 'true';
      }
    }
    
    if (filterKey === 'parentCategoryId' && value === '') {
      value = undefined;
    }

    this.categoryStore.setFilters({ [filterKey]: value });
  }

  clearFilters(): void {
    this.categoryStore.clearFilters();
  }

  // Métodos de exportación y acciones en lote
  exportCategories(): void {
    const categories = this.categoryStore.exportCategories();
    if (categories.length === 0) {
      this.uiStore.showError('Error', 'No hay categorías para exportar');
      return;
    }
    const csvContent = this.convertToCSV(categories);
    this.downloadCSV(csvContent, 'categorias.csv');
    this.uiStore.showSuccess('Exportación completada', 'Las categorías han sido exportadas a CSV');
  }

  bulkActions(): void {
    this.uiStore.showInfo('Acciones en lote', 'Funcionalidad de acciones en lote próximamente');
    // Aquí implementarías la funcionalidad de acciones en lote
    // Por ejemplo: seleccionar múltiples categorías y aplicar acciones como activar/desactivar, eliminar, etc.
  }

  // Métodos de estadísticas
  getCategoryHierarchy(): AdminCategory[] {
    return this.categoryStore.getCategoryHierarchy();
  }

  getTotalProducts(): number {
    const categories = this.categoryStore.categories();
    return categories.reduce((total, category) => total + (category.productCount || 0), 0);
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive 
      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
      : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200';
  }

  getParentCategoryName(parentId: string): string {
    const parent = this.categoryStore.categories().find(c => c.id === parentId);
    return parent ? parent.name : 'Categoría padre';
  }

  // Métodos de utilidad
  private convertToCSV(categories: AdminCategory[]): string {
    const headers = ['ID', 'Nombre', 'Slug', 'Descripción', 'Estado', 'Destacada', 'Productos', 'Orden'];
    const rows = categories.map(c => [
      c.id,
      c.name,
      c.slug || '',
      c.description,
      c.isActive ? 'Activa' : 'Inactiva',
      c.isFeatured ? 'Sí' : 'No',
      c.productCount || 0,
      c.sortOrder || 0
    ]);
    
    return [headers, ...rows]
      .map(row => row.map(cell => `"${cell}"`).join(','))
      .join('\n');
  }

  private downloadCSV(content: string, filename: string): void {
    const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', filename);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
