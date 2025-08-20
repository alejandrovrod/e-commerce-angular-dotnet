import { Injectable, signal } from '@angular/core';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message?: string;
  duration?: number;
  persistent?: boolean;
  actions?: NotificationAction[];
}

export interface NotificationAction {
  label: string;
  action: () => void;
  style?: 'primary' | 'secondary';
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly _notifications = signal<Notification[]>([]);
  
  readonly notifications = this._notifications.asReadonly();

  showSuccess(title: string, message?: string, duration = 5000): string {
    return this.addNotification({
      type: 'success',
      title,
      message,
      duration
    });
  }

  showError(title: string, message?: string, persistent = false): string {
    return this.addNotification({
      type: 'error',
      title,
      message,
      persistent,
      duration: persistent ? undefined : 8000
    });
  }

  showWarning(title: string, message?: string, duration = 6000): string {
    return this.addNotification({
      type: 'warning',
      title,
      message,
      duration
    });
  }

  showInfo(title: string, message?: string, duration = 5000): string {
    return this.addNotification({
      type: 'info',
      title,
      message,
      duration
    });
  }

  showCustom(notification: Omit<Notification, 'id'>): string {
    return this.addNotification(notification);
  }

  dismiss(id: string): void {
    this._notifications.update(notifications => 
      notifications.filter(n => n.id !== id)
    );
  }

  dismissAll(): void {
    this._notifications.set([]);
  }

  private addNotification(notification: Omit<Notification, 'id'>): string {
    const id = this.generateId();
    const newNotification: Notification = {
      ...notification,
      id
    };

    this._notifications.update(notifications => [...notifications, newNotification]);

    // Auto-dismiss if duration is specified
    if (notification.duration && !notification.persistent) {
      setTimeout(() => this.dismiss(id), notification.duration);
    }

    return id;
  }

  private generateId(): string {
    return `notification_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }
}
