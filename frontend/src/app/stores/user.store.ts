import { Injectable, computed, signal, effect } from '@angular/core';
import { User, UserRole } from '../core/models/user.model';

export interface UserState {
  currentUser: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class UserStore {
  // Private state signals with modern Angular syntax
  private readonly _currentUser = signal<User | null>(null);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);

  // Public readonly computed signals
  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();

  // Computed signals
  readonly isAuthenticated = computed(() => this._currentUser() !== null);
  readonly userRole = computed(() => this._currentUser()?.role || null);
  readonly isAdmin = computed(() => this._currentUser()?.role === UserRole.Admin);
  readonly isCustomer = computed(() => this._currentUser()?.role === UserRole.Customer);
  readonly userInitials = computed(() => {
    const user = this._currentUser();
    if (!user) return '';
    const name = user.name || `${user.firstName || ''} ${user.lastName || ''}`.trim();
    if (name) {
      const parts = name.split(' ');
      return parts.length >= 2 
        ? `${parts[0].charAt(0)}${parts[1].charAt(0)}`.toUpperCase()
        : name.charAt(0).toUpperCase();
    }
    return user.email.charAt(0).toUpperCase();
  });
  readonly userFullName = computed(() => {
    const user = this._currentUser();
    if (!user) return '';
    return user.name || `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email;
  });

  constructor() {
    // Cargar usuario desde localStorage al inicializar
    this.loadUserFromStorage();
  }

  // State management methods
  setUser(user: User | null): void {
    this._currentUser.set(user);
    this._error.set(null);
    
    if (user) {
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('token', 'mock-token-' + user.id); // Reemplazar con token real
    } else {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
    this._isLoading.set(false);
  }

  updateUser(updates: Partial<User>): void {
    const currentUser = this._currentUser();
    if (currentUser) {
      const updatedUser = { ...currentUser, ...updates };
      this._currentUser.set(updatedUser);
      // Persistir cambios en localStorage
      localStorage.setItem('user', JSON.stringify(updatedUser));
    }
  }

  clearUser(): void {
    this._currentUser.set(null);
    this._error.set(null);
    this._isLoading.set(false);
    
    // Limpiar localStorage
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  // Authentication methods
  logout(): void {
    this.clearUser();
  }

  // Utility methods
  reset(): void {
    this.clearUser();
  }

  // Private methods
  private loadUserFromStorage(): void {
    try {
      const userStr = localStorage.getItem('user');
      if (userStr) {
        const user = JSON.parse(userStr);
        this._currentUser.set(user);
      }
    } catch (error) {
      console.error('Error loading user from storage:', error);
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }
  }
}
