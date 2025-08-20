import { Injectable, computed, signal } from '@angular/core';
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
  readonly userInitials = computed(() => {
    const user = this._currentUser();
    if (!user) return '';
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
  });
  readonly userFullName = computed(() => {
    const user = this._currentUser();
    if (!user) return '';
    return `${user.firstName} ${user.lastName}`;
  });

  // State management methods
  setUser(user: User | null): void {
    this._currentUser.set(user);
    this._error.set(null);
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
      this._currentUser.set({ ...currentUser, ...updates });
    }
  }

  clearUser(): void {
    this._currentUser.set(null);
    this._error.set(null);
    this._isLoading.set(false);
  }

  // Utility methods
  reset(): void {
    this._currentUser.set(null);
    this._isLoading.set(false);
    this._error.set(null);
  }
}
