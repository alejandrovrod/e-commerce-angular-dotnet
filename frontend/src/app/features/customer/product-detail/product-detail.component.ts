import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">
        Detalle del Producto
      </h1>
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <p class="text-gray-600 dark:text-gray-400">
          Vista detallada del producto - En desarrollo
        </p>
      </div>
    </div>
  `
})
export class ProductDetailComponent {
  // TODO: Implementar vista detallada del producto
}
