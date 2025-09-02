import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { UIStore } from '../../../stores/ui.store';
import { UserRole } from '../../../core/models/user.model';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center p-4">
      <div class="max-w-md w-full space-y-8">
        <!-- Logo y Título -->
        <div class="text-center">
          <div class="mx-auto h-16 w-16 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center mb-4">
            <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </div>
          <h2 class="text-3xl font-bold text-white mb-2">Admin Panel</h2>
          <p class="text-gray-300">Accede a tu panel de administración</p>
        </div>

        <!-- Formulario de Login -->
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl p-8 shadow-2xl border border-white/20">
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-6">
            <!-- Email -->
            <div>
              <label for="email" class="block text-sm font-medium text-gray-200 mb-2">
                Correo Electrónico
              </label>
              <input
                id="email"
                type="email"
                formControlName="email"
                class="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200"
                placeholder="admin@ecommerce.com"
                [class.border-red-400]="isFieldInvalid('email')"
              />
              <div *ngIf="isFieldInvalid('email')" class="mt-1 text-red-400 text-sm">
                <span *ngIf="loginForm.get('email')?.hasError('required')">El email es requerido</span>
                <span *ngIf="loginForm.get('email')?.hasError('email')">Formato de email inválido</span>
              </div>
            </div>

            <!-- Contraseña -->
            <div>
              <label for="password" class="block text-sm font-medium text-gray-200 mb-2">
                Contraseña
              </label>
              <div class="relative">
                <input
                  id="password"
                  [type]="showPassword ? 'text' : 'password'"
                  formControlName="password"
                  class="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200 pr-12"
                  placeholder="••••••••"
                  [class.border-red-400]="isFieldInvalid('password')"
                />
                <button
                  type="button"
                  (click)="togglePassword()"
                  class="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-white transition-colors"
                >
                  <svg *ngIf="!showPassword" class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                  </svg>
                  <svg *ngIf="showPassword" class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21"></path>
                  </svg>
                </button>
              </div>
              <div *ngIf="isFieldInvalid('password')" class="mt-1 text-red-400 text-sm">
                <span *ngIf="loginForm.get('password')?.hasError('required')">La contraseña es requerida</span>
                <span *ngIf="loginForm.get('password')?.hasError('minlength')">Mínimo 6 caracteres</span>
              </div>
            </div>

            <!-- Recordar sesión -->
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <input
                  id="remember"
                  type="checkbox"
                  formControlName="remember"
                  class="h-4 w-4 text-purple-600 focus:ring-purple-500 border-gray-300 rounded"
                />
                <label for="remember" class="ml-2 block text-sm text-gray-200">
                  Recordar sesión
                </label>
              </div>
              <a href="#" class="text-sm text-purple-400 hover:text-purple-300 transition-colors">
                ¿Olvidaste tu contraseña?
              </a>
            </div>

            <!-- Botón de Login -->
            <button
              type="submit"
              [disabled]="loginForm.invalid || isLoading"
              class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
            >
              <svg *ngIf="isLoading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ isLoading ? 'Iniciando sesión...' : 'Iniciar Sesión' }}
            </button>

            <!-- Error de autenticación -->
            <div *ngIf="authError" class="mt-4 p-3 bg-red-500/20 border border-red-500/30 rounded-lg">
              <p class="text-red-400 text-sm text-center">{{ authError }}</p>
            </div>
          </form>

          <!-- Enlaces adicionales -->
          <div class="mt-6 text-center">
            <p class="text-gray-300 text-sm">
              ¿Eres cliente?
              <a routerLink="/auth/customer-login" class="text-purple-400 hover:text-purple-300 transition-colors">
                Inicia sesión aquí
              </a>
            </p>
          </div>
        </div>

        <!-- Footer -->
        <div class="text-center text-gray-400 text-sm">
          <p>&copy; 2024 E-Commerce Admin Panel. Todos los derechos reservados.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
    }
  `]
})
export class AdminLoginComponent implements OnInit {
  loginForm: FormGroup;
  isLoading = false;
  showPassword = false;
  authError: string | null = null;
  returnUrl: string = '/admin/dashboard';

  constructor(
    private fb: FormBuilder,
    private userStore: UserStore,
    private uiStore: UIStore,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.fb.group({
      email: ['admin@ecommerce.com', [Validators.required, Validators.email]],
      password: ['admin123', [Validators.required, Validators.minLength(6)]],
      remember: [true]
    });
  }

  ngOnInit(): void {
    // Obtener URL de retorno si existe
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin/dashboard';
    
    // Si ya está autenticado como admin, redirigir
    if (this.userStore.isAuthenticated() && this.userStore.isAdmin()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit(): void {
    console.log('🔐 Login: onSubmit llamado');
    console.log('📝 Formulario válido:', this.loginForm.valid);
    console.log('📝 Valores del formulario:', this.loginForm.value);
    
    if (this.loginForm.invalid) {
      console.log('❌ Formulario inválido');
      return;
    }

    this.isLoading = true;
    this.authError = null;

    const { email, password, remember } = this.loginForm.value;
    console.log('🔑 Credenciales:', { email, password, remember });

    // Simular login (aquí conectarías con tu API)
    setTimeout(() => {
      console.log('⏰ Timeout completado, verificando credenciales...');
      
      // Datos de ejemplo para testing
      if (email === 'admin@ecommerce.com' && password === 'admin123') {
        console.log('✅ Credenciales válidas, creando usuario admin...');
        
        // Login como admin
        const adminUser = {
          id: 'admin-1',
          email: 'admin@ecommerce.com',
          name: 'Administrador',
          role: UserRole.Admin,
          avatar: '/assets/images/admin-avatar.jpg'
        };

        console.log('👤 Usuario admin creado:', adminUser);
        console.log('💾 Guardando en UserStore...');
        
        this.userStore.setUser(adminUser);
        
        console.log('🔍 Verificando si se guardó correctamente...');
        console.log('🔍 isAuthenticated:', this.userStore.isAuthenticated());
        console.log('🔍 userRole:', this.userStore.isAdmin());
        console.log('🔍 user:', this.userStore.currentUser());
        
        this.uiStore.showSuccess('¡Bienvenido Admin!', 'Sesión iniciada correctamente');
        
        console.log('🚀 Redirigiendo a admin dashboard...');
        this.router.navigate(['/admin/dashboard']);
        
      } else if (email === 'customer@ecommerce.com' && password === 'customer123') {
        console.log('✅ Credenciales válidas, creando usuario customer...');
        
        // Login como customer
        const customerUser = {
          id: 'customer-1',
          email: 'customer@ecommerce.com',
          name: 'Cliente',
          role: UserRole.Customer,
          avatar: '/assets/images/customer-avatar.jpg'
        };

        console.log('👤 Usuario customer creado:', customerUser);
        console.log('💾 Guardando en UserStore...');
        
        this.userStore.setUser(customerUser);
        
        console.log('🔍 Verificando si se guardó correctamente...');
        console.log('🔍 isAuthenticated:', this.userStore.isAuthenticated());
        console.log('🔍 userRole:', this.userStore.isCustomer());
        console.log('🔍 user:', this.userStore.currentUser());
        
        this.uiStore.showSuccess('¡Bienvenido!', 'Sesión iniciada correctamente');
        
        console.log('🚀 Redirigiendo a:', this.returnUrl);
        this.router.navigate([this.returnUrl]);
        
      } else {
        console.log('❌ Credenciales inválidas');
        this.authError = 'Credenciales inválidas. Intenta con admin@ecommerce.com / admin123 o customer@ecommerce.com / customer123';
      }
      this.isLoading = false;
    }, 1000);
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
