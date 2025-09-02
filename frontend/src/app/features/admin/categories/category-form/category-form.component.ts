import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminCategoryStore, AdminCategory } from '../../../../stores/admin-category.store';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-4xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ isEditMode ? 'Editar Categoría' : 'Crear Nueva Categoría' }}
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ isEditMode ? 'Modifica la información de la categoría' : 'Completa la información para crear una nueva categoría' }}
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
            (click)="saveCategory()"
            [disabled]="categoryForm.invalid || isSaving"
            class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors rounded-md"
          >
            <svg *ngIf="isSaving" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ isSaving ? 'Guardando...' : (isEditMode ? 'Actualizar' : 'Crear') }}
          </button>
        </div>
      </div>

      <!-- Formulario -->
      <form [formGroup]="categoryForm" (ngSubmit)="saveCategory()" class="space-y-6">
        <!-- Información básica -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Información Básica</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre -->
            <div>
              <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Nombre de la Categoría *
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                (input)="onNameChange()"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: Electronics"
                [class.border-red-500]="isFieldInvalid('name')"
              />
              <div *ngIf="isFieldInvalid('name')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="categoryForm.get('name')?.hasError('required')">El nombre es requerido</span>
              </div>
            </div>

            <!-- Slug -->
            <div>
              <label for="slug" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Slug *
              </label>
              <input
                id="slug"
                type="text"
                formControlName="slug"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: electronics"
                [class.border-red-500]="isFieldInvalid('slug')"
              />
              <div *ngIf="isFieldInvalid('slug')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="categoryForm.get('slug')?.hasError('required')">El slug es requerido</span>
                <span *ngIf="categoryForm.get('slug')?.hasError('pattern')">Solo letras minúsculas, números y guiones</span>
              </div>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                El slug se genera automáticamente desde el nombre
              </p>
            </div>

            <!-- Categoría Padre -->
            <div>
              <label for="parentId" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Categoría Padre
              </label>
              <select
                id="parentId"
                formControlName="parentId"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Sin categoría padre</option>
                <option *ngFor="let category of availableParentCategories" [value]="category.id">
                  {{ category.name }}
                </option>
              </select>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                Deja vacío para crear una categoría principal
              </p>
            </div>

            <!-- Orden de Clasificación -->
            <div>
              <label for="sortOrder" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Orden de Clasificación
              </label>
              <input
                id="sortOrder"
                type="number"
                formControlName="sortOrder"
                min="0"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="0"
              />
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                Número menor = aparece primero
              </p>
            </div>
          </div>

          <!-- Descripción -->
          <div class="mt-6">
            <label for="description" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Descripción *
            </label>
            <textarea
              id="description"
              formControlName="description"
              rows="4"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
              placeholder="Describe la categoría y qué productos incluye..."
              [class.border-red-500]="isFieldInvalid('description')"
            ></textarea>
            <div *ngIf="isFieldInvalid('description')" class="mt-1 text-red-500 text-sm">
              <span *ngIf="categoryForm.get('description')?.hasError('required')">La descripción es requerida</span>
            </div>
          </div>
        </div>

        <!-- Imagen -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Imagen</h3>
          
          <div>
            <label for="image" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              URL de la Imagen
            </label>
            <input
              id="image"
              type="url"
              formControlName="image"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
              placeholder="https://ejemplo.com/imagen.jpg"
            />
            <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
              URL de la imagen representativa de la categoría
            </p>
          </div>

          <!-- Vista previa de la imagen -->
          <div *ngIf="categoryForm.get('image')?.value" class="mt-4">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Vista Previa
            </label>
            <div class="w-32 h-32 border-2 border-gray-300 dark:border-gray-600 rounded-lg overflow-hidden">
              <img
                [src]="categoryForm.get('image')?.value"
                [alt]="categoryForm.get('name')?.value"
                class="w-full h-full object-cover"
                (error)="onImageError($event)"
              />
            </div>
          </div>
        </div>

        <!-- Configuración -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Configuración</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="space-y-4">
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Categoría Activa</span>
              </label>
              
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isFeatured"
                  class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Categoría Destacada</span>
              </label>
            </div>
            
            <div class="space-y-4">
              <div class="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Información de la Categoría</h4>
                <div class="space-y-2 text-xs text-gray-600 dark:text-gray-400">
                  <div>Nombre: {{ categoryForm.get('name')?.value || 'No especificado' }}</div>
                  <div>Slug: {{ categoryForm.get('slug')?.value || 'No especificado' }}</div>
                  <div>Padre: {{ getParentCategoryName() || 'Sin categoría padre' }}</div>
                  <div>Orden: {{ categoryForm.get('sortOrder')?.value || '0' }}</div>
                </div>
              </div>
            </div>
          </div>
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
export class CategoryFormComponent implements OnInit, OnDestroy {
  categoryForm: FormGroup;
  isEditMode = false;
  isSaving = false;
  categoryId: string | null = null;
  availableParentCategories: AdminCategory[] = [];

  constructor(
    private fb: FormBuilder,
    private categoryStore: AdminCategoryStore,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      slug: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      image: [''],
      isActive: [true],
      isFeatured: [false],
      parentId: [''],
      sortOrder: [0, [Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.categoryId = params['id'];
        if (this.categoryId) {
          this.loadCategory(this.categoryId);
        }
      }
    });

    // Cargar categorías disponibles como padres
    this.availableParentCategories = this.categoryStore.parentCategories();
  }

  ngOnDestroy(): void {
    // Cleanup si es necesario
  }

  loadCategory(id: string): void {
    const category = this.categoryStore.getCategoryById(id);
    if (category) {
      this.categoryForm.patchValue({
        name: category.name,
        description: category.description,
        slug: category.slug,
        image: category.image || '',
        isActive: category.isActive,
        isFeatured: category.isFeatured,
        parentId: category.parentCategoryId || '',
        sortOrder: category.sortOrder
      });

      // Filtrar categorías padre disponibles (excluir la actual)
      this.availableParentCategories = this.categoryStore.parentCategories()
        .filter((c: any) => c.id !== id);
    }
  }

  onNameChange(): void {
    const name = this.categoryForm.get('name')?.value;
    if (name && !this.isEditMode) {
      const slug = this.generateSlug(name);
      this.categoryForm.patchValue({ slug });
    }
  }

  onImageError(event: any): void {
    event.target.style.display = 'none';
    if (event.target.nextElementSibling) {
      event.target.nextElementSibling.style.display = 'block';
    }
  }

  saveCategory(): void {
    if (this.categoryForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isSaving = true;
    const formValue = this.categoryForm.value;

    const category: AdminCategory = {
      id: this.isEditMode ? this.categoryId! : this.generateId(),
      name: formValue.name,
      description: formValue.description,
      slug: formValue.slug,
      image: formValue.image || undefined,
      isActive: formValue.isActive,
      isFeatured: formValue.isFeatured,
      parentCategoryId: formValue.parentId || undefined,
      sortOrder: formValue.sortOrder || 0,
      productCount: this.isEditMode ? (this.categoryStore.getCategoryById(this.categoryId!)?.productCount || 0) : 0,
      createdAt: this.isEditMode ? new Date() : new Date(),
      updatedAt: new Date()
    };

    try {
      if (this.isEditMode) {
        const updateCommand = {
          id: category.id,
          name: category.name,
          description: category.description,
          parentCategoryId: category.parentCategoryId
        };
        this.categoryStore.updateCategory(category.id, updateCommand);
        this.uiStore.showSuccess('Categoría actualizada', `${category.name} ha sido actualizada correctamente`);
      } else {
        const createCommand = {
          name: category.name,
          description: category.description,
          parentCategoryId: category.parentCategoryId
        };
        this.categoryStore.createCategory(createCommand);
        this.uiStore.showSuccess('Categoría creada', `${category.name} ha sido creada correctamente`);
      }
      
      this.router.navigate(['/admin/categories']);
    } catch (error) {
      this.uiStore.showError('Error', 'Ha ocurrido un error al guardar la categoría');
    } finally {
      this.isSaving = false;
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/categories']);
  }

  getParentCategoryName(): string {
    const parentId = this.categoryForm.get('parentId')?.value;
    if (!parentId) return '';
    
    const parent = this.categoryStore.getCategoryById(parentId);
    return parent ? parent.name : '';
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.categoryForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.categoryForm.controls).forEach(key => {
      const control = this.categoryForm.get(key);
      control?.markAsTouched();
    });
  }

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
}
