import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminStore } from '../../../stores/admin.store';
import { UIStore } from '../../../stores/ui.store';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="space-y-6">
      <!-- Header del Dashboard -->
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Resumen general de tu ecommerce
          </p>
        </div>
        
        <!-- Selector de período -->
        <div class="mt-4 sm:mt-0">
          <select
            (change)="onPeriodChange($event)"
            [value]="adminStore.selectedPeriod()"
            class="block w-full sm:w-auto px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="7d">Últimos 7 días</option>
            <option value="30d">Últimos 30 días</option>
            <option value="90d">Últimos 90 días</option>
            <option value="1y">Último año</option>
          </select>
        </div>
      </div>

      <!-- Métricas principales -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- Total de Ventas -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Total de Ventas
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ adminStore.totalRevenue() | number:'1.0-0' }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-5 py-3">
            <div class="text-sm">
              <span class="text-green-600 dark:text-green-400 font-medium">
                +{{ adminStore.getRevenueGrowth() | number:'1.0-1' }}%
              </span>
              <span class="text-gray-500 dark:text-gray-400"> vs período anterior</span>
            </div>
          </div>
        </div>

        <!-- Total de Pedidos -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Total de Pedidos
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ adminStore.orderCount() | number }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-5 py-3">
            <div class="text-sm">
              <span class="text-blue-600 dark:text-blue-400 font-medium">
                +{{ adminStore.getOrderGrowth() | number:'1.0-1' }}%
              </span>
              <span class="text-gray-500 dark:text-gray-400"> vs período anterior</span>
            </div>
          </div>
        </div>

        <!-- Total de Clientes -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Total de Clientes
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ adminStore.customerCount() | number }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-5 py-3">
            <div class="text-sm">
              <span class="text-purple-600 dark:text-purple-400 font-medium">
                +12.5%
              </span>
              <span class="text-gray-500 dark:text-gray-400"> vs período anterior</span>
            </div>
          </div>
        </div>

        <!-- Valor Promedio del Pedido -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Valor Promedio
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ adminStore.avgOrderValue() | number:'1.0-2' }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-5 py-3">
            <div class="text-sm">
              <span class="text-yellow-600 dark:text-yellow-400 font-medium">
                +8.2%
              </span>
              <span class="text-gray-500 dark:text-gray-400"> vs período anterior</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos y Tablas -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Gráfico de Ventas -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Ventas y Pedidos</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Tendencia de los últimos días</p>
          </div>
          <div class="p-6">
            <div class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
              <div class="text-center">
                <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
                </svg>
                <p class="mt-2">Gráfico de ventas</p>
                <p class="text-sm">Aquí se mostrará el gráfico con Chart.js</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Productos Top -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Productos Más Vendidos</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Por volumen de ventas</p>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div *ngFor="let product of adminStore.topProductsBySales(); trackBy: trackByProduct" class="flex items-center space-x-3">
                <img
                  [src]="product.image || '/assets/images/placeholder.jpg'"
                  [alt]="product.name"
                  class="w-10 h-10 rounded-lg object-cover"
                />
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-medium text-gray-900 dark:text-white truncate">
                    {{ product.name }}
                  </p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">
                    {{ product.sales }} ventas
                  </p>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ product.revenue | number:'1.0-0' }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pedidos Recientes -->
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Pedidos Recientes</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Últimos pedidos del sistema</p>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  ID
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Cliente
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Total
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Estado
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Fecha
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Acciones
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr *ngFor="let order of adminStore.recentOrders(); trackBy: trackByOrder">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                  {{ order.id }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ order.customerName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ order.total | number:'1.0-2' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                    [ngClass]="getStatusClasses(order.status)"
                  >
                    {{ getStatusText(order.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ order.date | date:'short' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    (click)="viewOrder(order.id)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    Ver
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  
  constructor(
    public adminStore: AdminStore,
    private uiStore: UIStore
  ) {}

  ngOnInit(): void {
    // El store ya carga datos de ejemplo en el constructor
  }

  onPeriodChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const period = target.value as '7d' | '30d' | '90d' | '1y';
    this.adminStore.setPeriod(period);
  }

  viewOrder(orderId: string): void {
    this.uiStore.showInfo('Ver Pedido', `Redirigiendo al pedido ${orderId}`);
    // Aquí implementarías la navegación al detalle del pedido
  }

  getStatusClasses(status: string): string {
    switch (status) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300';
      case 'processing':
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300';
      case 'shipped':
        return 'bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-300';
      case 'delivered':
        return 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300';
      case 'cancelled':
        return 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-300';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'pending': return 'Pendiente';
      case 'processing': return 'Procesando';
      case 'shipped': return 'Enviado';
      case 'delivered': return 'Entregado';
      case 'cancelled': return 'Cancelado';
      default: return status;
    }
  }

  trackByProduct(index: number, product: any): string {
    return product.id;
  }

  trackByOrder(index: number, order: any): string {
    return order.id;
  }
}
