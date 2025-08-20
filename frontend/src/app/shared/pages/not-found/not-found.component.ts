import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900">
      <div class="text-center">
        <div class="mb-8">
          <h1 class="text-9xl font-bold text-primary-600 dark:text-primary-400">404</h1>
          <p class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">Page Not Found</p>
          <p class="text-gray-600 dark:text-gray-400 mb-8">
            Sorry, we couldn't find the page you're looking for.
          </p>
        </div>
        <div class="space-y-4">
          <a routerLink="/" class="btn btn-primary">
            Go Back Home
          </a>
          <a routerLink="/products" class="btn btn-outline ml-4">
            Browse Products
          </a>
        </div>
      </div>
    </div>
  `
})
export class NotFoundComponent {}
