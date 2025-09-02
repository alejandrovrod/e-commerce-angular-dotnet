import { Component, OnInit, OnDestroy, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminProductStore } from '../../../../stores/admin-product.store';
import { ProductDto, CreateProductCommand, ProductStatus } from 'src/app/core/services/product.service';
import { UIStore } from '../../../../stores/ui.store';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-4xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ isEditMode ? 'Editar Producto' : 'Crear Nuevo Producto' }}
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ isEditMode ? 'Modifica la información del producto' : 'Completa la información para crear un nuevo producto' }}
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
            (click)="saveProduct()"
            [disabled]="productForm.invalid || isSaving"
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

      <!-- Error State -->
      @if (error()) {
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          <p><strong>Error:</strong> {{ error() }}</p>
        </div>
      }

      <!-- Formulario -->
      <form [formGroup]="productForm" (ngSubmit)="saveProduct()" class="space-y-6">
        <!-- Información básica -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Información Básica</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre -->
            <div>
              <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Nombre del Producto *
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: Laptop Gaming Pro"
                [class.border-red-500]="isFieldInvalid('name')"
              />
              <div *ngIf="isFieldInvalid('name')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('name') }}
              </div>
            </div>

            <!-- SKU -->
            <div>
              <label for="sku" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                SKU *
              </label>
              <input
                id="sku"
                type="text"
                formControlName="sku"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: LAP-GAM-001"
                [class.border-red-500]="isFieldInvalid('sku')"
              />
              <div *ngIf="isFieldInvalid('sku')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('sku') }}
              </div>
            </div>

            <!-- Categoría -->
            <div>
              <label for="category" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Categoría *
              </label>
              <select
                id="category"
                formControlName="category"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                [class.border-red-500]="isFieldInvalid('category')"
              >
                <option value="">Seleccionar categoría</option>
                @for (category of categories(); track category.id) {
                  <option [value]="category.id">{{ category.name }}</option>
                }
              </select>
              <div *ngIf="isFieldInvalid('category')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="productForm.get('category')?.hasError('required')">La categoría es requerida</span>
              </div>
            </div>

            <!-- Marca -->
            <div>
              <label for="brand" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Marca *
              </label>
              <select
                id="brand"
                formControlName="brand"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                [class.border-red-500]="isFieldInvalid('brand')"
              >
                <option value="">Seleccionar marca</option>
                @for (brand of brands(); track brand.id) {
                  <option [value]="brand.name">{{ brand.name }}</option>
                }
              </select>
              <div *ngIf="isFieldInvalid('brand')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="productForm.get('brand')?.hasError('required')">La marca es requerida</span>
              </div>
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
              placeholder="Describe las características y beneficios del producto..."
              [class.border-red-500]="isFieldInvalid('description')"
            ></textarea>
                          <div *ngIf="isFieldInvalid('description')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('description') }}
              </div>
          </div>
        </div>

        <!-- Precios y Stock -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Precios y Stock</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <!-- Precio -->
            <div>
              <label for="price" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Precio *
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-gray-500 dark:text-gray-400 sm:text-sm">$</span>
                </div>
                <input
                  id="price"
                  type="number"
                  formControlName="price"
                  step="0.01"
                  min="0"
                  class="block w-full pl-7 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                  placeholder="0.00"
                  [class.border-red-500]="isFieldInvalid('price')"
                />
              </div>
              <div *ngIf="isFieldInvalid('price')" class="mt-1 text-red-500 text-sm">
                {{ getFieldError('price') }}
              </div>
            </div>

            <!-- Precio Original -->
            <div>
              <label for="originalPrice" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Precio Original
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-gray-500 dark:text-gray-400 sm:text-sm">$</span>
                </div>
                <input
                  id="originalPrice"
                  type="number"
                  formControlName="originalPrice"
                  step="0.01"
                  min="0"
                  class="block w-full pl-7 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                  placeholder="0.00"
                />
              </div>
            </div>

            <!-- Stock -->
            <div>
              <label for="stock" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Stock *
              </label>
              <input
                id="stock"
                type="number"
                formControlName="stock"
                min="0"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="0"
                [class.border-red-500]="isFieldInvalid('stock')"
              />
              <div *ngIf="isFieldInvalid('stock')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="productForm.get('stock')?.hasError('required')">El stock es requerido</span>
                <span *ngIf="productForm.get('stock')?.hasError('min')">El stock no puede ser negativo</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Imágenes -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Imágenes</h3>
          
          <div formArrayName="images" class="space-y-4">
            <div *ngFor="let image of imagesArray.controls; let i = index" class="flex items-center space-x-3">
              <input
                [formControlName]="i"
                type="text"
                class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="URL de la imagen"
              />
              <button
                type="button"
                (click)="removeImage(i)"
                class="p-2 text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                </svg>
              </button>
            </div>
          </div>
          
          <button
            type="button"
            (click)="addImage()"
            class="mt-4 inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 transition-colors"
          >
            <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
            </svg>
            Agregar Imagen
          </button>
        </div>

        <!-- Tags -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Etiquetas</h3>
          
          <div class="space-y-4">
            <div class="flex flex-wrap gap-2">
              <span
                *ngFor="let tag of tagsArray; let i = index"
                class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-300"
              >
                {{ tag }}
                <button
                  type="button"
                  (click)="removeTag(i)"
                  class="ml-2 text-purple-600 hover:text-purple-800 dark:text-purple-400 dark:hover:text-purple-300"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
              </span>
            </div>
            
            <div class="flex space-x-2">
              <input
                #tagInput
                type="text"
                (keyup.enter)="addTag(tagInput.value)"
                class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Agregar etiqueta y presionar Enter"
              />
              <button
                type="button"
                (click)="addTag(tagInput.value)"
                class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-md transition-colors"
              >
                Agregar
              </button>
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
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Producto Activo</span>
              </label>
              
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isFeatured"
                  class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Producto Destacado</span>
              </label>
            </div>
            
            <div class="space-y-4">
              <div>
                <label for="weight" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Peso (kg)
                </label>
                <input
                  id="weight"
                  type="number"
                  formControlName="weight"
                  step="0.01"
                  min="0"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                  placeholder="0.00"
                />
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
export class ProductFormComponent implements OnInit, OnDestroy {
  productForm: FormGroup;
  isEditMode = false;
  isSaving = false;
  productId: string | null = null;

  // Signals del store
  categories = this.productStore.categories;
  brands = this.productStore.brands;
  isLoading = this.productStore.isLoading;
  error = this.productStore.error;

  constructor(
    private fb: FormBuilder,
    private productStore: AdminProductStore,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.productForm = this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]],
      description: ['', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(1000)
      ]],
      price: [0, [
        Validators.required,
        Validators.min(0.01),
        Validators.max(999999.99)
      ]],
      originalPrice: [0, [
        Validators.min(0),
        Validators.max(999999.99)
      ]],
      images: this.fb.array(['']),
      category: ['', [Validators.required]],
      brand: ['', [Validators.required]],
      tags: this.fb.array([]),
      stock: [0, [
        Validators.required,
        Validators.min(0),
        Validators.max(99999)
      ]],
      rating: [0, [
        Validators.min(0),
        Validators.max(5)
      ]],
      reviewCount: [0, [
        Validators.min(0)
      ]],
      isActive: [true],
      isFeatured: [false],
      isDigital: [false],
      requiresShipping: [true],
      isTaxable: [true],
      sku: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        Validators.pattern(/^[A-Z0-9-_]+$/)
      ]],
      weight: [0, [
        Validators.min(0),
        Validators.max(999.99)
      ]],
      dimensions: this.fb.group({
        length: [0, [Validators.min(0), Validators.max(999.99)]],
        width: [0, [Validators.min(0), Validators.max(999.99)]],
        height: [0, [Validators.min(0), Validators.max(999.99)]]
      })
    });
  }

  ngOnInit(): void {
    // Cargar categorías y marcas para el formulario
    this.productStore.loadCategories();
    this.productStore.loadBrands();
    
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.productId = params['id'];
        if (this.productId) {
          this.loadProduct(this.productId);
        }
      }
    });
  }

  ngOnDestroy(): void {
    // Cleanup si es necesario
  }

  get imagesArray() {
    return this.productForm.get('images') as FormArray;
  }

  get tagsArray() {
    return this.productForm.get('tags')?.value || [];
  }

  loadProduct(id: string): void {
    this.productStore.loadProductById(id);
    
    // Usar effect para reaccionar a los cambios del producto seleccionado
    effect(() => {
      const product = this.productStore.selectedProduct();
      if (product) {
        this.productForm.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          category: product.categoryId,
          brand: product.brand,
          isFeatured: product.isFeatured,
          isDigital: product.isDigital,
          requiresShipping: product.requiresShipping,
          isTaxable: product.isTaxable,
          sku: product.sku
        });
      }
    });
  }

  addImage(): void {
    this.imagesArray.push(this.fb.control(''));
  }

  removeImage(index: number): void {
    if (this.imagesArray.length > 1) {
      this.imagesArray.removeAt(index);
    }
  }

  addTag(value: string): void {
    if (value && value.trim()) {
      const tagsArray = this.productForm.get('tags') as FormArray;
      if (!tagsArray.value.includes(value.trim())) {
        tagsArray.push(this.fb.control(value.trim()));
      }
      // Limpiar el input
      const input = document.querySelector('input[placeholder*="etiqueta"]') as HTMLInputElement;
      if (input) input.value = '';
    }
  }

  removeTag(index: number): void {
    const tagsArray = this.productForm.get('tags') as FormArray;
    tagsArray.removeAt(index);
  }

  saveProduct(): void {
    if (this.productForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isSaving = true;
    const formValue = this.productForm.value;

    // Filtrar imágenes vacías
    formValue.images = formValue.images.filter((img: string) => img.trim() !== '');

    const productCommand: CreateProductCommand = {
      name: formValue.name,
      description: formValue.description,
      sku: formValue.sku,
      price: formValue.price,
      brand: formValue.brand,
      categoryId: formValue.category,
      isFeatured: formValue.isFeatured,
      isDigital: formValue.isDigital,
      requiresShipping: formValue.requiresShipping,
      isTaxable: formValue.isTaxable
    };

    try {
      if (this.isEditMode) {
        this.productStore.updateProduct(this.productId!, productCommand);
        this.uiStore.showSuccess('Producto actualizado', `${productCommand.name} ha sido actualizado correctamente`);
      } else {
        this.productStore.createProduct(productCommand);
        this.uiStore.showSuccess('Producto creado', `${productCommand.name} ha sido creado correctamente`);
      }
      
      this.router.navigate(['/admin/products']);
    } catch (error) {
      this.uiStore.showError('Error', 'Ha ocurrido un error al guardar el producto');
    } finally {
      this.isSaving = false;
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/products']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.productForm.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    const errors = field.errors;
    
    if (errors['required']) return `${this.getFieldLabel(fieldName)} es requerido`;
    if (errors['minlength']) return `${this.getFieldLabel(fieldName)} debe tener al menos ${errors['minlength'].requiredLength} caracteres`;
    if (errors['maxlength']) return `${this.getFieldLabel(fieldName)} no puede tener más de ${errors['maxlength'].requiredLength} caracteres`;
    if (errors['min']) return `${this.getFieldLabel(fieldName)} debe ser mayor a ${errors['min'].min}`;
    if (errors['max']) return `${this.getFieldLabel(fieldName)} no puede ser mayor a ${errors['max'].max}`;
    if (errors['pattern']) return `${this.getFieldLabel(fieldName)} tiene un formato inválido`;
    
    return 'Campo inválido';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      'name': 'El nombre',
      'description': 'La descripción',
      'price': 'El precio',
      'originalPrice': 'El precio original',
      'category': 'La categoría',
      'brand': 'La marca',
      'stock': 'El stock',
      'sku': 'El SKU',
      'weight': 'El peso',
      'rating': 'La calificación',
      'reviewCount': 'El número de reseñas'
    };
    return labels[fieldName] || 'Este campo';
  }

  private markFormGroupTouched(): void {
    Object.keys(this.productForm.controls).forEach(key => {
      const control = this.productForm.get(key);
      control?.markAsTouched();
    });
  }

  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }
}
