import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserStore } from '../../stores/user.store';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStore);
  const router = inject(Router);

  if (authService.isAuthenticated() && userStore.isAuthenticated()) {
    return true;
  }

  // Store the attempted URL for redirecting after login
  const returnUrl = state.url !== '/auth/login' ? state.url : '/';
  
  // Redirect to login page with return url
  router.navigate(['/auth/login'], { 
    queryParams: { returnUrl },
    replaceUrl: true 
  });
  
  return false;
};
