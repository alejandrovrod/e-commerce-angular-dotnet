import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      switch (error.status) {
        case 0:
          errorMessage = 'Network error. Please check your connection.';
          break;
        case 400:
          errorMessage = error.error?.message || 'Bad request';
          break;
        case 401:
          errorMessage = 'You are not authorized. Please log in.';
          // Only logout if it's not a refresh token request
          if (!req.url.includes('/auth/refresh')) {
            authService.logout();
          }
          break;
        case 403:
          errorMessage = 'Access forbidden. You don\'t have permission.';
          break;
        case 404:
          errorMessage = 'The requested resource was not found.';
          break;
        case 409:
          errorMessage = error.error?.message || 'Conflict occurred';
          break;
        case 422:
          errorMessage = 'Validation failed';
          // Handle validation errors
          if (error.error?.errors) {
            const validationErrors = Object.values(error.error.errors).flat();
            errorMessage = validationErrors.join(', ');
          }
          break;
        case 429:
          errorMessage = 'Too many requests. Please try again later.';
          break;
        case 500:
          errorMessage = 'Internal server error. Please try again later.';
          router.navigate(['/500']);
          break;
        case 502:
        case 503:
        case 504:
          errorMessage = 'Service temporarily unavailable. Please try again later.';
          break;
        default:
          errorMessage = error.error?.message || `Error ${error.status}: ${error.statusText}`;
      }

      // Show error notification (except for certain status codes)
      if (![401, 500].includes(error.status)) {
        notificationService.showError(errorMessage);
      }

      // Log error for debugging
      console.error('HTTP Error:', {
        status: error.status,
        statusText: error.statusText,
        message: errorMessage,
        url: req.url,
        error: error.error
      });

      return throwError(() => ({
        ...error,
        userMessage: errorMessage
      }));
    })
  );
};
