import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';

import { 
  User, 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest, 
  AuthTokens,
  PasswordResetRequest,
  PasswordChangeRequest 
} from '../models/user.model';
import { UserStore } from '../../stores/user.store';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly userStore = inject(UserStore);
  
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private tokenRefreshInProgress = false;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  constructor() {
    this.initializeAuth();
  }

  // Authentication methods
  login(credentials: LoginRequest): Observable<AuthResponse> {
    this.userStore.setLoading(true);
    
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.handleAuthResponse(response);
      }),
      catchError(error => {
        this.userStore.setError(error.error?.message || 'Login failed');
        return throwError(() => error);
      })
    );
  }

  register(userData: RegisterRequest): Observable<AuthResponse> {
    this.userStore.setLoading(true);
    
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, userData).pipe(
      tap(response => {
        this.handleAuthResponse(response);
      }),
      catchError(error => {
        this.userStore.setError(error.error?.message || 'Registration failed');
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    // Call logout endpoint
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      complete: () => {
        this.clearAuthData();
        this.router.navigate(['/auth/login']);
      }
    });
  }

  refreshToken(): Observable<AuthTokens> {
    if (this.tokenRefreshInProgress) {
      // Return a new observable that waits for the current refresh to complete
      return new Observable(observer => {
        const checkRefresh = () => {
          if (!this.tokenRefreshInProgress) {
            const token = this.getAccessToken();
            if (token) {
              observer.next({ 
                accessToken: token, 
                refreshToken: this.getRefreshToken() || '',
                expiresIn: 3600,
                tokenType: 'Bearer'
              });
            } else {
              observer.error('Token refresh failed');
            }
            observer.complete();
          } else {
            setTimeout(checkRefresh, 100);
          }
        };
        checkRefresh();
      });
    }

    this.tokenRefreshInProgress = true;
    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      this.tokenRefreshInProgress = false;
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<AuthTokens>(`${this.apiUrl}/refresh`, { 
      refreshToken 
    }).pipe(
      tap(tokens => {
        this.storeTokens(tokens);
        this.tokenRefreshInProgress = false;
      }),
      catchError(error => {
        this.tokenRefreshInProgress = false;
        this.logout();
        return throwError(() => error);
      })
    );
  }

  // Password management
  requestPasswordReset(request: PasswordResetRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/password-reset`, request);
  }

  resetPassword(token: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/password-reset/confirm`, {
      token,
      newPassword
    });
  }

  changePassword(request: PasswordChangeRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/password-change`, request);
  }

  // Email verification
  verifyEmail(token: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/verify-email`, { token });
  }

  resendVerificationEmail(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/verify-email/resend`, {});
  }

  // User profile
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`).pipe(
      tap(user => {
        this.userStore.setUser(user);
      }),
      catchError(error => {
        if (error.status === 401) {
          this.logout();
        }
        return throwError(() => error);
      })
    );
  }

  // Authentication state
  isAuthenticated(): boolean {
    return !!this.getAccessToken() && this.userStore.isAuthenticated();
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  // Check if token is expired
  isTokenExpired(): boolean {
    const token = this.getAccessToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= expirationTime;
    } catch {
      return true;
    }
  }

  // Initialize authentication on app start
  async checkAuthStatus(): Promise<void> {
    const token = this.getAccessToken();
    
    if (!token) {
      this.userStore.clearUser();
      return;
    }

    if (this.isTokenExpired()) {
      try {
        await this.refreshToken().toPromise();
        await this.getCurrentUser().toPromise();
      } catch {
        this.logout();
      }
    } else {
      try {
        await this.getCurrentUser().toPromise();
      } catch {
        this.logout();
      }
    }
  }

  // Private methods
  private handleAuthResponse(response: AuthResponse): void {
    this.storeTokens(response.tokens);
    this.userStore.setUser(response.user);
    this.userStore.setLoading(false);
  }

  private storeTokens(tokens: AuthTokens): void {
    localStorage.setItem(this.TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, tokens.refreshToken);
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.userStore.clearUser();
  }

  private initializeAuth(): void {
    // Check if user was previously authenticated
    const token = this.getAccessToken();
    if (token && !this.isTokenExpired()) {
      // Token exists and is valid, get current user
      this.getCurrentUser().subscribe({
        error: () => this.logout()
      });
    }
  }
}
