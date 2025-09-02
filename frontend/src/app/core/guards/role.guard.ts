import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { UserRole } from '../../core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  
  constructor(
    private userStore: UserStore,
    private router: Router
  ) {}
  
  canActivate(): boolean | UrlTree {
    // Verificar si el usuario está autenticado
    if (!this.userStore.isAuthenticated()) {
      return this.router.createUrlTree(['/auth/login']);
    }

    // Verificar el rol del usuario
    const userRole = this.userStore.userRole();
    
    // Permitir acceso según el rol
    if (userRole === UserRole.Admin) {
      return true;
    } else if (userRole === UserRole.Customer) {
      return true;
    }
    
    // Si no tiene rol válido, redirigir al login
    return this.router.createUrlTree(['/auth/login']);
  }
}
