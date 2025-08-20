import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900">
      <div class="text-center">
        <div class="mb-8">
          <h1 class="text-9xl font-bold text-red-600 dark:text-red-400">500</h1>
          <p class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">Server Error</p>
          <p class="text-gray-600 dark:text-gray-400 mb-8">
            Something went wrong on our end. We're working to fix it.
          </p>
        </div>
        <div class="space-y-4">
          <a routerLink="/" class="btn btn-primary">
            Go Back Home
          </a>
          <button onclick="window.location.reload()" class="btn btn-outline ml-4">
            Try Again
          </button>
        </div>
      </div>
    </div>
  `
})
export class ServerErrorComponent {}
