import { Injectable, signal, computed, effect } from '@angular/core';

export interface UIState {
  theme: 'light' | 'dark';
  sidebarOpen: boolean;
  mobileMenuOpen: boolean;
  notifications: Notification[];
  loadingStates: Record<string, boolean>;
  modals: Record<string, boolean>;
}

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class UIStore {
  // Signals para el estado reactivo
  private readonly _theme = signal<'light' | 'dark'>('light');
  private readonly _sidebarOpen = signal<boolean>(true);
  private readonly _mobileMenuOpen = signal<boolean>(false);
  private readonly _notifications = signal<Notification[]>([]);
  private readonly _loadingStates = signal<Record<string, boolean>>({});
  private readonly _modals = signal<Record<string, boolean>>({});

  // Computed signals
  public readonly theme = this._theme.asReadonly();
  public readonly sidebarOpen = this._sidebarOpen.asReadonly();
  public readonly mobileMenuOpen = this._mobileMenuOpen.asReadonly();
  public readonly notifications = this._notifications.asReadonly();
  public readonly loadingStates = this._loadingStates.asReadonly();
  public readonly modals = this._modals.asReadonly();

  public readonly isDarkMode = computed(() => this._theme() === 'dark');
  public readonly isLightMode = computed(() => this._theme() === 'light');
  public readonly hasNotifications = computed(() => this._notifications().length > 0);
  public readonly unreadNotifications = computed(() => 
    this._notifications().filter(n => !n.timestamp || 
      (Date.now() - n.timestamp.getTime()) < 5000) // 5 segundos como "no leído"
  );

  constructor() {
    // Cargar tema desde localStorage al inicializar
    this.loadThemeFromStorage();

    // Aplicar tema al body cuando cambie
    effect(() => {
      this.applyTheme();
    });

    // Auto-remover notificaciones expiradas
    setInterval(() => {
      this.cleanupExpiredNotifications();
    }, 1000);
  }

  // Actions para tema
  toggleTheme(): void {
    const newTheme = this._theme() === 'light' ? 'dark' : 'light';
    this._theme.set(newTheme);
    localStorage.setItem('theme', newTheme);
  }

  setTheme(theme: 'light' | 'dark'): void {
    this._theme.set(theme);
    localStorage.setItem('theme', theme);
  }

  // Actions para sidebar
  toggleSidebar(): void {
    this._sidebarOpen.update(open => !open);
  }

  openSidebar(): void {
    this._sidebarOpen.set(true);
  }

  closeSidebar(): void {
    this._sidebarOpen.set(false);
  }

  // Actions para mobile menu
  toggleMobileMenu(): void {
    this._mobileMenuOpen.update(open => !open);
  }

  openMobileMenu(): void {
    this._mobileMenuOpen.set(true);
  }

  closeMobileMenu(): void {
    this._mobileMenuOpen.set(false);
  }

  // Actions para notificaciones
  addNotification(notification: Omit<Notification, 'id' | 'timestamp'>): void {
    const newNotification: Notification = {
      ...notification,
      id: this.generateId(),
      timestamp: new Date()
    };

    this._notifications.update(notifications => [newNotification, ...notifications]);

    // Auto-remover notificación si tiene duración
    if (notification.duration) {
      setTimeout(() => {
        this.removeNotification(newNotification.id);
      }, notification.duration);
    }
  }

  removeNotification(id: string): void {
    this._notifications.update(notifications => 
      notifications.filter(n => n.id !== id)
    );
  }

  clearNotifications(): void {
    this._notifications.set([]);
  }

  // Actions para loading states
  setLoading(key: string, loading: boolean): void {
    this._loadingStates.update(states => ({
      ...states,
      [key]: loading
    }));
  }

  isLoading(key: string): boolean {
    return this._loadingStates()[key] || false;
  }

  // Actions para modales
  openModal(key: string): void {
    this._modals.update(modals => ({
      ...modals,
      [key]: true
    }));
  }

  closeModal(key: string): void {
    this._modals.update(modals => ({
      ...modals,
      [key]: false
    }));
  }

  isModalOpen(key: string): boolean {
    return this._modals()[key] || false;
  }

  // Helpers
  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  private loadThemeFromStorage(): void {
    try {
      const savedTheme = localStorage.getItem('theme') as 'light' | 'dark';
      if (savedTheme && (savedTheme === 'light' || savedTheme === 'dark')) {
        this._theme.set(savedTheme);
      } else {
        // Detectar tema del sistema
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        this._theme.set(prefersDark ? 'dark' : 'light');
      }
    } catch (error) {
      console.error('Error loading theme from storage:', error);
    }
  }

  private applyTheme(): void {
    const theme = this._theme();
    document.documentElement.classList.remove('light', 'dark');
    document.documentElement.classList.add(theme);
    
    // También aplicar a body para compatibilidad
    document.body.classList.remove('light', 'dark');
    document.body.classList.add(theme);
  }

  private cleanupExpiredNotifications(): void {
    const now = Date.now();
    this._notifications.update(notifications => 
      notifications.filter(n => {
        if (!n.duration) return true;
        return (now - n.timestamp.getTime()) < n.duration;
      })
    );
  }

  // Métodos de utilidad
  showSuccess(title: string, message: string, duration: number = 5000): void {
    this.addNotification({ type: 'success', title, message, duration });
  }

  showError(title: string, message: string, duration: number = 8000): void {
    this.addNotification({ type: 'error', title, message, duration });
  }

  showWarning(title: string, message: string, duration: number = 6000): void {
    this.addNotification({ type: 'warning', title, message, duration });
  }

  showInfo(title: string, message: string, duration: number = 4000): void {
    this.addNotification({ type: 'info', title, message, duration });
  }

  // Métodos para cerrar todo
  closeAll(): void {
    this._sidebarOpen.set(false);
    this._mobileMenuOpen.set(false);
    this._modals.update(() => ({}));
  }

  // Métodos para responsive
  isMobile(): boolean {
    return window.innerWidth < 768;
  }

  isTablet(): boolean {
    return window.innerWidth >= 768 && window.innerWidth < 1024;
  }

  isDesktop(): boolean {
    return window.innerWidth >= 1024;
  }
}
