import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { UIStore } from '../../stores/ui.store';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
      <!-- Overlay para cerrar sidebar en móviles -->
      <div 
        *ngIf="uiStore.sidebarOpen() && isMobile()"
        (click)="uiStore.closeSidebar()"
        class="fixed inset-0 z-40 bg-black bg-opacity-50 transition-opacity duration-300 ease-in-out"
      ></div>

      <!-- Sidebar -->
      <div 
        class="fixed inset-y-0 left-0 z-50 bg-white dark:bg-gray-800 shadow-lg transition-all duration-300 ease-in-out"
        [class.w-64]="uiStore.sidebarOpen()"
        [class.w-16]="!uiStore.sidebarOpen()"
        [class.translate-x-0]="uiStore.sidebarOpen() || !isMobile()"
        [class.-translate-x-full]="!uiStore.sidebarOpen() && isMobile()"
        [style.width]="uiStore.sidebarOpen() ? '256px' : '64px'"
      >

        
        <!-- Logo y Botón Hamburguesa -->
        <div class="flex items-center h-16 border-b border-gray-200 dark:border-gray-700 relative"
             [class.justify-between]="uiStore.sidebarOpen()"
             [class.justify-center]="!uiStore.sidebarOpen()"
             [class.px-4]="uiStore.sidebarOpen()"
             [class.px-2]="!uiStore.sidebarOpen()">
          
          <!-- Contenido cuando está expandido -->
          <div class="flex items-center space-x-3" *ngIf="uiStore.sidebarOpen()">
            <div class="w-8 h-8 bg-gradient-to-r from-purple-600 to-pink-600 rounded-lg flex items-center justify-center cursor-default">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <span class="text-xl font-bold text-gray-900 dark:text-white">Admin Panel</span>
          </div>
          
          <!-- Botón Hamburguesa cuando está expandido -->
          <button
            *ngIf="uiStore.sidebarOpen()"
            (click)="toggleSidebar()"
            class="w-8 h-8 flex items-center justify-center rounded-md text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 transition-all duration-200 cursor-pointer"
          >
            <svg 
              class="w-5 h-5 transition-transform duration-200" 
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 4v16M12 4v16M18 4v16"></path>
            </svg>
          </button>
          

          
          <!-- Botón Hamburguesa - Solo visible cuando está colapsado -->
          <button
            *ngIf="!uiStore.sidebarOpen()"
            (click)="toggleSidebar()"
            class="absolute w-8 h-8 flex items-center justify-center rounded-md text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 transition-all duration-200 cursor-pointer z-20"
            [style.right]="'8px'"
            [style.top]="'50%'"
            [style.transform]="'translateY(-50%)'"
          >
            <svg 
              class="w-5 h-5"
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
          </button>
        </div>

        <!-- Navigation Menu -->
        <nav class="mt-6 px-2">
          <div class="space-y-1">
            <!-- Dashboard -->
            <a
              routerLink="/admin/dashboard"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Dashboard"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Dashboard</span>
            </a>

            <!-- Productos -->
            <a
              routerLink="/admin/products"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Productos"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Productos</span>
            </a>

            <!-- Categorías -->
            <a
              routerLink="/admin/categories"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Categorías"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Categorías</span>
            </a>

            <!-- Marcas -->
            <a
              routerLink="/admin/brands"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Marcas"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Marcas</span>
            </a>

            <!-- Pedidos -->
            <a
              routerLink="/admin/orders"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Pedidos"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Pedidos</span>
            </a>

            <!-- Usuarios -->
            <a
              routerLink="/admin/users"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Usuarios"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Usuarios</span>
            </a>

            <!-- Analytics -->
            <a
              routerLink="/admin/analytics"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Analytics"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Analytics</span>
            </a>

            <!-- Configuración -->
            <a
              routerLink="/admin/settings"
              routerLinkActive="bg-purple-50 dark:bg-purple-900/20 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-700"
              class="group flex items-center rounded-lg border border-transparent hover:bg-gray-50 dark:hover:bg-gray-700/50 hover:text-gray-900 dark:hover:text-white transition-all duration-200"
              [class.px-3]="uiStore.sidebarOpen()"
              [class.px-2]="!uiStore.sidebarOpen()"
              [class.py-2]="uiStore.sidebarOpen()"
              [class.py-3]="!uiStore.sidebarOpen()"
              [class.justify-center]="!uiStore.sidebarOpen()"
              [class.justify-start]="uiStore.sidebarOpen()"
              title="Configuración"
            >
              <svg class="h-5 w-5 text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
              </svg>
              <span *ngIf="uiStore.sidebarOpen()" class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">Configuración</span>
            </a>
          </div>
        </nav>

        <!-- User Info -->
        <div class="absolute bottom-0 left-0 right-0 border-t border-gray-200 dark:border-gray-700"
             [class.p-4]="uiStore.sidebarOpen()"
             [class.p-2]="!uiStore.sidebarOpen()">
          <div class="flex items-center"
               [class.space-x-3]="uiStore.sidebarOpen()"
               [class.justify-center]="!uiStore.sidebarOpen()">
            <img
              [src]="userStore.currentUser()?.avatar || '/assets/images/admin-avatar.jpg'"
              [alt]="userStore.currentUser()?.name"
              class="rounded-full object-cover"
              [class.w-8]="uiStore.sidebarOpen()"
              [class.h-8]="uiStore.sidebarOpen()"
              [class.w-10]="!uiStore.sidebarOpen()"
              [class.h-10]="!uiStore.sidebarOpen()"
            />
            <div *ngIf="uiStore.sidebarOpen()" class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 dark:text-white truncate">
                {{ userStore.currentUser()?.name }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400 truncate">
                {{ userStore.currentUser()?.email }}
              </p>
            </div>
            <button
              (click)="logout()"
              class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
              [class.p-1]="uiStore.sidebarOpen()"
              [class.p-2]="!uiStore.sidebarOpen()"
              [class.ml-auto]="uiStore.sidebarOpen()"
              title="Cerrar sesión"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path>
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Main Content -->
      <div class="transition-all duration-300 ease-in-out" 
           [class.pl-64]="uiStore.sidebarOpen() && !isMobile()"
           [class.pl-16]="!uiStore.sidebarOpen() && !isMobile()"
           [class.pl-0]="isMobile()">
        <!-- Header -->
        <header class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between h-16 px-4 sm:px-6 lg:px-8">
            <!-- Mobile menu button -->
            <button
              *ngIf="isMobile()"
              (click)="toggleSidebar()"
              class="p-2 rounded-md text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 transition-all duration-200"
            >
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
              </svg>
            </button>

            <!-- Page title -->
            <div class="flex-1 min-w-0">
              <h1 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ getPageTitle() }}
              </h1>
            </div>

            <!-- Right side actions -->
            <div class="flex items-center space-x-4">
              <!-- Theme toggle -->
              <button
                (click)="uiStore.toggleTheme()"
                class="p-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
                title="Cambiar tema"
              >
                <svg *ngIf="uiStore.isLightMode()" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
                </svg>
                <svg *ngIf="uiStore.isDarkMode()" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
                </svg>
              </button>

              <!-- Notifications -->
              <button
                (click)="uiStore.openModal('notifications')"
                class="p-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors relative"
                title="Notificaciones"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM4.19 4.19A4 4 0 004 7v6a4 4 0 004 4h6a4 4 0 004-4V7a4 4 0 00-4-4H8a4 4 0 00-2.81 1.19z"></path>
                </svg>
                <span
                  *ngIf="uiStore.unreadNotifications().length > 0"
                  class="absolute -top-1 -right-1 h-4 w-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center"
                >
                  {{ uiStore.unreadNotifications().length }}
                </span>
              </button>
            </div>
          </div>
        </header>

        <!-- Page content -->
        <main class="p-4 sm:p-6 lg:p-8">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
    }
  `]
})
export class AdminLayoutComponent implements OnInit {
  private router = inject(Router);
  
  constructor(
    public userStore: UserStore,
    public uiStore: UIStore
  ) {}

  ngOnInit(): void {
    console.log('🔧 AdminLayoutComponent: ngOnInit');
    console.log('🔧 Window width:', window.innerWidth);
    console.log('🔧 Is mobile:', this.isMobile());
    console.log('🔧 Initial sidebar state:', this.uiStore.sidebarOpen());
    
    // Verificar autenticación
    if (!this.userStore.isAuthenticated() || !this.userStore.isAdmin()) {
      console.log('❌ Usuario no autenticado o no es admin, redirigiendo a login');
      this.router.navigate(['/auth/login']);
      return;
    }

    console.log('✅ Usuario autenticado como admin');
    console.log('🔧 Sidebar state inicial:', this.uiStore.sidebarOpen());

    // Configurar sidebar según el tamaño de pantalla
    if (this.isMobile()) {
      console.log('📱 Pantalla móvil detectada, sidebar cerrado por defecto');
      this.uiStore.closeSidebar();
    } else {
      console.log('🖥️ Pantalla grande detectada, sidebar cerrado para mostrar menú colapsado');
      this.uiStore.closeSidebar();
    }
    
    console.log('🔧 Sidebar state final:', this.uiStore.sidebarOpen());
  }

  logout(): void {
    if (!this.userStore.isAuthenticated() || !this.userStore.isAdmin()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    
    this.userStore.logout();
    this.router.navigate(['/auth/login']);
  }

  isMobile(): boolean {
    return window.innerWidth < 1024;
  }

  toggleSidebar(): void {
    console.log('🔧 Toggle sidebar clicked');
    console.log('🔧 Current state:', this.uiStore.sidebarOpen());
    this.uiStore.toggleSidebar();
    console.log('🔧 New state:', this.uiStore.sidebarOpen());
  }

  getPageTitle(): string {
    const url = this.router.url;
    if (url.includes('/dashboard')) return 'Dashboard';
    if (url.includes('/products')) return 'Gestión de Productos';
    if (url.includes('/categories')) return 'Gestión de Categorías';
    if (url.includes('/brands')) return 'Gestión de Marcas';
    if (url.includes('/orders')) return 'Gestión de Pedidos';
    if (url.includes('/users')) return 'Gestión de Usuarios';
    if (url.includes('/analytics')) return 'Analytics';
    if (url.includes('/settings')) return 'Configuración';
    return 'Admin Panel';
  }
}
