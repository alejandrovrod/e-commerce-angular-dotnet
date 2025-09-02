import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { UserRole } from '../../core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AdminAuthGuard implements CanActivate {
  
  constructor(
    private userStore: UserStore,
    private router: Router
  ) {}
  
  canActivate(): boolean | UrlTree {
    console.log('🔒 AdminAuthGuard: canActivate llamado');
    console.log('🔍 isAuthenticated:', this.userStore.isAuthenticated());
    console.log('🔍 userRole:', this.userStore.userRole());
    
    // Verificar si el usuario está autenticado
    if (!this.userStore.isAuthenticated()) {
      console.log('❌ Usuario no autenticado, redirigiendo a login');
      return this.router.createUrlTree(['/auth/login']);
    }

    // Verificar si el usuario tiene rol de administrador
    const userRole = this.userStore.userRole();
    if (userRole !== UserRole.Admin) {
      console.log('❌ Usuario no es admin, rol actual:', userRole);
      // Si no es administrador, redirigir según su rol
      if (userRole === UserRole.Customer) {
        console.log('🔄 Usuario es customer, redirigiendo a shop');
        return this.router.createUrlTree(['/shop']);
      }
      // Si no tiene rol válido, redirigir al login
      console.log('❌ Rol inválido, redirigiendo a login');
      return this.router.createUrlTree(['/auth/login']);
    }

    console.log('✅ Usuario autenticado como admin, permitiendo acceso');
    return true;
  }
}
