import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { CartStore } from '../../../stores/cart.store';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="bg-white dark:bg-gray-900 shadow-sm border-b dark:border-gray-800">
      <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo -->
          <div class="flex items-center">
            <a routerLink="/" class="flex items-center space-x-2">
              <div class="h-8 w-8 bg-primary-600 rounded-lg flex items-center justify-center">
                <span class="text-white font-bold text-lg">E</span>
              </div>
              <span class="font-bold text-xl text-gray-900 dark:text-white">Commerce</span>
            </a>
          </div>

          <!-- Navigation Links -->
          <div class="hidden md:flex items-center space-x-8">
            <a routerLink="/shop" 
               routerLinkActive="text-primary-600" 
               class="text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400 px-3 py-2 text-sm font-medium transition-colors">
              Shop
            </a>
            @if (userStore.isAdmin()) {
              <a routerLink="/admin" 
                 routerLinkActive="text-primary-600"
                 class="text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400 px-3 py-2 text-sm font-medium transition-colors">
                Admin
              </a>
            }
          </div>

          <!-- Right side items -->
          <div class="flex items-center space-x-4">
            <!-- Search -->
            <div class="hidden lg:block">
              <div class="relative">
                <input
                  type="text"
                  placeholder="Search products..."
                  class="w-64 px-4 py-2 pl-10 pr-4 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-50 dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                >
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
                  </svg>
                </div>
              </div>
            </div>

            <!-- Theme Toggle -->
            <button
              (click)="themeService.toggleTheme()"
              class="p-2 text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
              title="Toggle theme"
            >
              @if (themeService.isDark()) {
                <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"/>
                </svg>
              } @else {
                <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"/>
                </svg>
              }
            </button>

            <!-- Cart -->
            <a routerLink="/cart" class="relative p-2 text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors">
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m0 0h8"/>
              </svg>
              @if (cartStore.itemCount() > 0) {
                <span class="absolute -top-1 -right-1 bg-primary-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {{ cartStore.itemCount() }}
                </span>
              }
            </a>

            <!-- User Menu -->
            @if (userStore.isAuthenticated()) {
              <div class="relative">
                <button
                  #userMenuButton
                  (click)="toggleUserMenu()"
                  class="flex items-center space-x-2 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
                >
                  <div class="h-8 w-8 bg-primary-600 rounded-full flex items-center justify-center">
                    <span class="text-white text-sm font-medium">
                      {{ userStore.userInitials() }}
                    </span>
                  </div>
                  <span class="hidden md:block text-sm font-medium text-gray-700 dark:text-gray-300">
                    {{ userStore.userFullName() }}
                  </span>
                  <svg class="h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                  </svg>
                </button>

                <!-- User Dropdown Menu -->
                @if (showUserMenu) {
                  <div class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border dark:border-gray-700 z-50">
                    <div class="py-2">
                      <a routerLink="/profile" 
                         (click)="closeUserMenu()"
                         class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">
                        My Profile
                      </a>
                      <a routerLink="/orders" 
                         (click)="closeUserMenu()"
                         class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">
                        My Orders
                      </a>
                      <a routerLink="/wishlist" 
                         (click)="closeUserMenu()"
                         class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">
                        Wishlist
                      </a>
                      <hr class="my-2 border-gray-200 dark:border-gray-600">
                      <button
                        (click)="logout()"
                        class="w-full text-left px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-gray-100 dark:hover:bg-gray-700"
                      >
                        Sign Out
                      </button>
                    </div>
                  </div>
                }
              </div>
            } @else {
              <div class="flex items-center space-x-2">
                <a routerLink="/auth/login" 
                   class="text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400 px-3 py-2 text-sm font-medium transition-colors">
                  Sign In
                </a>
                <a routerLink="/auth/register" 
                   class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                  Sign Up
                </a>
              </div>
            }

            <!-- Mobile menu button -->
            <button
              (click)="toggleMobileMenu()"
              class="md:hidden p-2 text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
              </svg>
            </button>
          </div>
        </div>

        <!-- Mobile Menu -->
        @if (showMobileMenu) {
          <div class="md:hidden border-t dark:border-gray-800 pt-4 pb-3">
            <div class="space-y-1">
              <a routerLink="/shop" 
                 (click)="closeMobileMenu()"
                 class="block px-3 py-2 text-base font-medium text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400">
                Shop
              </a>
            </div>
          </div>
        }
      </nav>
    </header>
  `
})
export class HeaderComponent {
  protected readonly userStore = inject(UserStore);
  protected readonly cartStore = inject(CartStore);
  protected readonly authService = inject(AuthService);
  protected readonly themeService = inject(ThemeService);

  showUserMenu = false;
  showMobileMenu = false;

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  closeUserMenu(): void {
    this.showUserMenu = false;
  }

  toggleMobileMenu(): void {
    this.showMobileMenu = !this.showMobileMenu;
  }

  closeMobileMenu(): void {
    this.showMobileMenu = false;
  }

  logout(): void {
    this.authService.logout();
    this.closeUserMenu();
  }
}
