import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { UserRole } from '../../core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerAuthGuard implements CanActivate {
  
  constructor(
    private userStore: UserStore,
    private router: Router
  ) {}
  
  canActivate(): boolean | UrlTree {
    // Verificar si el usuario está autenticado
    if (!this.userStore.isAuthenticated()) {
      console.log('❌ Usuario no autenticado, redirigiendo a login');
      return this.router.createUrlTree(['/auth/login']);
    }

    // Verificar si el usuario tiene rol de cliente
    const userRole = this.userStore.userRole();
    if (userRole !== UserRole.Customer) {
      console.log('❌ Usuario no es customer, rol actual:', userRole);
      // Si no es cliente, redirigir según su rol
      if (userRole === UserRole.Admin) {
        console.log('🔄 Usuario es admin, redirigiendo a admin dashboard');
        return this.router.createUrlTree(['/admin/dashboard']);
      }
      // Si no tiene rol válido, redirigir al login
      console.log('❌ Rol inválido, redirigiendo a login');
      return this.router.createUrlTree(['/auth/login']);
    }

    return true;
  }
}
