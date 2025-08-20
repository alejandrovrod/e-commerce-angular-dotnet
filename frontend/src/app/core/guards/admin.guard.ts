import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { UserRole } from '../models/user.model';

export const adminGuard: CanActivateFn = (route, state) => {
  const userStore = inject(UserStore);
  const router = inject(Router);

  const userRole = userStore.userRole();
  
  if (userRole === UserRole.Admin || userRole === UserRole.SuperAdmin) {
    return true;
  }

  // Redirect to home page if not admin
  router.navigate(['/'], { replaceUrl: true });
  return false;
};
