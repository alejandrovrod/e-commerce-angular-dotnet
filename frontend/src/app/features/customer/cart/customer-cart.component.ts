import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-cart',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">
        Carrito de Compras
      </h1>
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <p class="text-gray-600 dark:text-gray-400">
          Carrito de compras - En desarrollo
        </p>
      </div>
    </div>
  `
})
export class CustomerCartComponent {
  // TODO: Implementar carrito de compras
}
