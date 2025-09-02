import { Injectable, signal, computed, effect } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject } from 'rxjs';
import { User, UserRole } from '../core/models/user.model';

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  // Signals para el estado reactivo
  private readonly _user = signal<User | null>(null);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);

  // Computed signals
  public readonly user = this._user.asReadonly();
  public readonly isAuthenticated = computed(() => this._user() !== null);
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly userRole = computed(() => this._user()?.role || null);

  // BehaviorSubjects para compatibilidad con RxJS
  private readonly _userSubject = new BehaviorSubject<User | null>(null);
  public readonly user$ = this._userSubject.asObservable();

  constructor() {
    // Sincronizar signals con BehaviorSubject
    effect(() => {
      this._userSubject.next(this._user());
    });

    // Cargar usuario desde localStorage al inicializar
    this.loadUserFromStorage();
  }

  // Actions
  setUser(user: User | null): void {
    this._user.set(user);
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
  }

  clearError(): void {
    this._error.set(null);
  }

  logout(): void {
    this._user.set(null);
    this._error.set(null);
    this._isLoading.set(false);
    
    // Limpiar localStorage
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  // Helpers
  private loadUserFromStorage(): void {
    try {
      const userStr = localStorage.getItem('user');
      if (userStr) {
        const user = JSON.parse(userStr);
        this._user.set(user);
      }
    } catch (error) {
      console.error('Error loading user from storage:', error);
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }
  }

  // Getters computados
  isCustomer(): boolean {
    return this.userRole() === UserRole.Customer;
  }

  isAdmin(): boolean {
    return this.userRole() === UserRole.Admin;
  }

  hasRole(role: UserRole): boolean {
    return this.userRole() === role;
  }
}
