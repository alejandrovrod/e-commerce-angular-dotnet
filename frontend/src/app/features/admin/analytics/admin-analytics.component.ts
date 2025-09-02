import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">
        Analytics y Reportes
      </h1>
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <p class="text-gray-600 dark:text-gray-400">
          Panel de analytics y reportes - En desarrollo
        </p>
      </div>
    </div>
  `
})
export class AdminAnalyticsComponent {
  // TODO: Implementar analytics y reportes
}
