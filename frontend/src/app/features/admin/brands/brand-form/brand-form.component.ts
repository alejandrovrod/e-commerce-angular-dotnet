import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminBrandStore, AdminBrand } from '../../../../stores/admin-brand.store';
import { UIStore } from '../../../../stores/ui.store';
import { CreateBrandCommand, UpdateBrandCommand } from '../../../../core/services/brand.service';

@Component({
  selector: 'app-brand-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-4xl mx-auto space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ isEditMode ? 'Editar Marca' : 'Crear Nueva Marca' }}
          </h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ isEditMode ? 'Modifica la información de la marca' : 'Completa la información para crear una nueva marca' }}
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
            (click)="saveBrand()"
            [disabled]="brandForm.invalid || isSaving"
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
      <form [formGroup]="brandForm" (ngSubmit)="saveBrand()" class="space-y-6">
        <!-- Información básica -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Información Básica</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre -->
            <div>
              <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Nombre de la Marca *
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                (input)="onNameChange()"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: GamingTech"
                [class.border-red-500]="isFieldInvalid('name')"
              />
              <div *ngIf="isFieldInvalid('name')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="brandForm.get('name')?.hasError('required')">El nombre es requerido</span>
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
                placeholder="Ej: gamingtech"
                [class.border-red-500]="isFieldInvalid('slug')"
              />
              <div *ngIf="isFieldInvalid('slug')" class="mt-1 text-red-500 text-sm">
                <span *ngIf="brandForm.get('slug')?.hasError('required')">El slug es requerido</span>
                <span *ngIf="brandForm.get('slug')?.hasError('pattern')">Solo letras minúsculas, números y guiones</span>
              </div>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                El slug se genera automáticamente desde el nombre
              </p>
            </div>

            <!-- País -->
            <div>
              <label for="country" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                País
              </label>
              <input
                id="country"
                type="text"
                formControlName="country"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: Estados Unidos"
              />
            </div>

            <!-- Año de Fundación -->
            <div>
              <label for="foundedYear" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Año de Fundación
              </label>
              <input
                id="foundedYear"
                type="number"
                formControlName="foundedYear"
                min="1800"
                max="2024"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ej: 1995"
              />
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
              placeholder="Describe la marca y su historia..."
              [class.border-red-500]="isFieldInvalid('description')"
            ></textarea>
            <div *ngIf="isFieldInvalid('description')" class="mt-1 text-red-500 text-sm">
              <span *ngIf="brandForm.get('description')?.hasError('required')">La descripción es requerida</span>
            </div>
          </div>
        </div>

        <!-- Logo y Sitio Web -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Logo y Sitio Web</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Logo -->
            <div>
              <label for="logo" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                URL del Logo
              </label>
              <input
                id="logo"
                type="url"
                formControlName="logo"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="https://ejemplo.com/logo.png"
              />
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                URL de la imagen del logo de la marca
              </p>
            </div>

            <!-- Sitio Web -->
            <div>
              <label for="website" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Sitio Web
              </label>
              <input
                id="website"
                type="url"
                formControlName="website"
                class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
                placeholder="https://ejemplo.com"
              />
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                Sitio web oficial de la marca
              </p>
            </div>
          </div>

          <!-- Vista previa del logo -->
          <div *ngIf="brandForm.get('logo')?.value" class="mt-6">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Vista Previa del Logo
            </label>
            <div class="w-32 h-32 border-2 border-gray-300 dark:border-gray-600 rounded-lg overflow-hidden">
              <img
                [src]="brandForm.get('logo')?.value"
                [alt]="brandForm.get('name')?.value + ' logo'"
                class="w-full h-full object-contain"
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
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Marca Activa</span>
              </label>
              
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isFeatured"
                  class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Marca Destacada</span>
              </label>
            </div>
            
            <div class="space-y-4">
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
          </div>

          <!-- Información de la marca -->
          <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Información de la Marca</h4>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-xs text-gray-600 dark:text-gray-400">
              <div>Nombre: {{ brandForm.get('name')?.value || 'No especificado' }}</div>
              <div>Slug: {{ brandForm.get('slug')?.value || 'No especificado' }}</div>
              <div>País: {{ brandForm.get('country')?.value || 'No especificado' }}</div>
              <div>Año: {{ brandForm.get('foundedYear')?.value || 'No especificado' }}</div>
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
export class BrandFormComponent implements OnInit, OnDestroy {
  brandForm: FormGroup;
  isEditMode = false;
  isSaving = false;
  brandId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private brandStore: AdminBrandStore,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.brandForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      slug: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      logo: [''],
      website: [''],
      isActive: [true],
      isFeatured: [false],
      sortOrder: [0, [Validators.min(0)]],
      country: [''],
      foundedYear: [null, [Validators.min(1800), Validators.max(2024)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.brandId = params['id'];
        if (this.brandId) {
          this.loadBrand(this.brandId);
        }
      }
    });
  }

  ngOnDestroy(): void {
    // Cleanup si es necesario
  }

  loadBrand(id: string): void {
    const brand = this.brandStore.getBrandById(id);
    if (brand) {
      this.brandForm.patchValue({
        name: brand.name,
        description: brand.description,
        slug: brand.slug,
        logo: brand.logoUrl || '',
        website: brand.website || '',
        isActive: brand.isActive,
        isFeatured: brand.isFeatured,
        sortOrder: brand.sortOrder,
        country: brand.country || '',
        foundedYear: brand.foundedYear || null
      });
    }
  }

  onNameChange(): void {
    const name = this.brandForm.get('name')?.value;
    if (name && !this.isEditMode) {
      const slug = this.generateSlug(name);
      this.brandForm.patchValue({ slug });
    }
  }

  onImageError(event: any): void {
    event.target.style.display = 'none';
    const nextElement = event.target.nextElementSibling;
    if (nextElement) {
      nextElement.style.display = 'block';
    }
  }

  saveBrand(): void {
    if (this.brandForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isSaving = true;
    const formValue = this.brandForm.value;

    const brand: AdminBrand = {
      id: this.isEditMode ? this.brandId! : this.generateId(),
      name: formValue.name,
      description: formValue.description,
      slug: formValue.slug,
      logoUrl: formValue.logo || '',
      website: formValue.website || '',
      isActive: formValue.isActive,
      isFeatured: formValue.isFeatured,
      sortOrder: formValue.sortOrder || 0,
      productCount: this.isEditMode && this.brandId ? (this.brandStore.getBrandById(this.brandId)?.productCount || 0) : 0,
      country: formValue.country || undefined,
      foundedYear: formValue.foundedYear || undefined,
      createdAt: this.isEditMode ? (this.brandStore.getBrandById(this.brandId!)?.createdAt || new Date().toISOString()) : new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    try {
      if (this.isEditMode) {
        const updateCommand: UpdateBrandCommand = {
          id: brand.id,
          name: brand.name,
          description: brand.description,
          logoUrl: brand.logoUrl,
          website: brand.website,
          isActive: brand.isActive
        };
        this.brandStore.updateBrand(updateCommand);
        this.uiStore.showSuccess('Marca actualizada', `${brand.name} ha sido actualizada correctamente`);
      } else {
        const createCommand: CreateBrandCommand = {
          name: brand.name,
          description: brand.description,
          logoUrl: brand.logoUrl,
          website: brand.website,
          isActive: brand.isActive
        };
        this.brandStore.createBrand(createCommand);
        this.uiStore.showSuccess('Marca creada', `${brand.name} ha sido creada correctamente`);
      }
      
      this.router.navigate(['/admin/brands']);
    } catch (error) {
      this.uiStore.showError('Error', 'Ha ocurrido un error al guardar la marca');
    } finally {
      this.isSaving = false;
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/brands']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.brandForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.brandForm.controls).forEach(key => {
      const control = this.brandForm.get(key);
      if (control) {
        control.markAsTouched();
      }
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
