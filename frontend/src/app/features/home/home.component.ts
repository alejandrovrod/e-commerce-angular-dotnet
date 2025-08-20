import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-primary-50 to-secondary-50 dark:from-gray-900 dark:to-gray-800">
      <!-- Hero Section -->
      <section class="relative py-20 px-4">
        <div class="max-w-7xl mx-auto text-center">
          <h1 class="text-4xl md:text-6xl font-bold text-gray-900 dark:text-white mb-6">
            Welcome to 
            <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-secondary-600">
              E-Commerce
            </span>
          </h1>
          <p class="text-xl text-gray-600 dark:text-gray-300 mb-8 max-w-3xl mx-auto">
            Discover amazing products with our modern shopping platform built with Angular 20 and .NET 9 Aspire
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <a routerLink="/products" 
               class="btn btn-primary btn-lg">
              Shop Now
            </a>
            <a routerLink="/categories" 
               class="btn btn-outline btn-lg">
              Browse Categories
            </a>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section class="py-16 px-4 bg-white dark:bg-gray-800">
        <div class="max-w-7xl mx-auto">
          <h2 class="text-3xl font-bold text-center text-gray-900 dark:text-white mb-12">
            Why Choose Our Platform?
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div class="text-center p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <div class="w-16 h-16 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg class="w-8 h-8 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Fast Performance</h3>
              <p class="text-gray-600 dark:text-gray-300">
                Built with Angular 20 and .NET 9 for lightning-fast performance and smooth user experience.
              </p>
            </div>

            <div class="text-center p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <div class="w-16 h-16 bg-secondary-100 dark:bg-secondary-900 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg class="w-8 h-8 text-secondary-600 dark:text-secondary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Secure Shopping</h3>
              <p class="text-gray-600 dark:text-gray-300">
                Advanced security features and encrypted transactions to keep your data safe.
              </p>
            </div>

            <div class="text-center p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <div class="w-16 h-16 bg-accent-100 dark:bg-accent-900 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg class="w-8 h-8 text-accent-600 dark:text-accent-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"/>
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Customer First</h3>
              <p class="text-gray-600 dark:text-gray-300">
                24/7 customer support and hassle-free returns for the best shopping experience.
              </p>
            </div>
          </div>
        </div>
      </section>

      <!-- Stats Section -->
      <section class="py-16 px-4 bg-gray-900 text-white">
        <div class="max-w-7xl mx-auto">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-8 text-center">
            <div>
              <div class="text-4xl font-bold text-primary-400 mb-2">10K+</div>
              <div class="text-gray-300">Happy Customers</div>
            </div>
            <div>
              <div class="text-4xl font-bold text-secondary-400 mb-2">50K+</div>
              <div class="text-gray-300">Products</div>
            </div>
            <div>
              <div class="text-4xl font-bold text-accent-400 mb-2">99.9%</div>
              <div class="text-gray-300">Uptime</div>
            </div>
            <div>
              <div class="text-4xl font-bold text-green-400 mb-2">24/7</div>
              <div class="text-gray-300">Support</div>
            </div>
          </div>
        </div>
      </section>
    </div>
  `
})
export class HomeComponent {}
