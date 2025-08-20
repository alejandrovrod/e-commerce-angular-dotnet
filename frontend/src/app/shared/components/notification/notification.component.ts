import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 z-50 space-y-2 max-w-sm">
      @for (notification of notificationService.notifications(); track notification.id) {
        <div
          class="transform transition-all duration-300 ease-in-out animate-slide-down"
          [class]="getNotificationClasses(notification)"
        >
          <div class="flex items-start space-x-3">
            <!-- Icon -->
            <div class="flex-shrink-0">
              @switch (notification.type) {
                @case ('success') {
                  <svg class="h-5 w-5 text-green-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
                  </svg>
                }
                @case ('error') {
                  <svg class="h-5 w-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
                  </svg>
                }
                @case ('warning') {
                  <svg class="h-5 w-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
                  </svg>
                }
                @case ('info') {
                  <svg class="h-5 w-5 text-blue-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
                  </svg>
                }
              }
            </div>

            <!-- Content -->
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 dark:text-gray-100">
                {{ notification.title }}
              </p>
              @if (notification.message) {
                <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                  {{ notification.message }}
                </p>
              }
              
              <!-- Actions -->
              @if (notification.actions && notification.actions.length > 0) {
                <div class="mt-2 flex space-x-2">
                  @for (action of notification.actions; track action.label) {
                    <button
                      type="button"
                      (click)="executeAction(action, notification.id)"
                      [class]="getActionClasses(action.style)"
                    >
                      {{ action.label }}
                    </button>
                  }
                </div>
              }
            </div>

            <!-- Close button -->
            <div class="flex-shrink-0">
              <button
                type="button"
                (click)="dismiss(notification.id)"
                class="inline-flex text-gray-400 hover:text-gray-500 dark:hover:text-gray-300 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
              >
                <span class="sr-only">Close</span>
                <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
                </svg>
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    @keyframes slideDown {
      from {
        transform: translateY(-100%);
        opacity: 0;
      }
      to {
        transform: translateY(0);
        opacity: 1;
      }
    }
    
    .animate-slide-down {
      animation: slideDown 0.3s ease-out;
    }
  `]
})
export class NotificationComponent {
  protected readonly notificationService = inject(NotificationService);

  dismiss(id: string): void {
    this.notificationService.dismiss(id);
  }

  executeAction(action: any, notificationId: string): void {
    action.action();
    this.dismiss(notificationId);
  }

  getNotificationClasses(notification: Notification): string {
    const baseClasses = 'rounded-lg shadow-lg p-4 bg-white dark:bg-gray-800 border-l-4';
    
    const typeClasses = {
      success: 'border-green-400',
      error: 'border-red-400',
      warning: 'border-yellow-400',
      info: 'border-blue-400'
    };

    return `${baseClasses} ${typeClasses[notification.type]}`;
  }

  getActionClasses(style?: 'primary' | 'secondary'): string {
    const baseClasses = 'text-xs font-medium px-3 py-1 rounded-md transition-colors duration-200';
    
    if (style === 'primary') {
      return `${baseClasses} bg-primary-600 text-white hover:bg-primary-700`;
    }
    
    return `${baseClasses} bg-gray-100 text-gray-800 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600`;
  }
}
