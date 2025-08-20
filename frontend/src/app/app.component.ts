import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { LoadingComponent } from './shared/components/loading/loading.component';
import { NotificationComponent } from './shared/components/notification/notification.component';

import { AuthService } from './core/services/auth.service';
import { LoadingService } from './core/services/loading.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    LoadingComponent,
    NotificationComponent
  ],
  template: `
    <div class="min-h-screen flex flex-col bg-gray-50 dark:bg-gray-900 transition-colors duration-300">
      <!-- Loading Overlay -->
      @if (loadingService.isLoading()) {
        <app-loading />
      }
      
      <!-- Header -->
      <app-header />
      
      <!-- Main Content -->
      <main class="flex-1">
        <router-outlet />
      </main>
      
      <!-- Footer -->
      <app-footer />
      
      <!-- Global Notifications -->
      <app-notification />
    </div>
  `,
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  private readonly authService = inject(AuthService);
  protected readonly loadingService = inject(LoadingService);
  private readonly themeService = inject(ThemeService);

  ngOnInit(): void {
    this.initializeApp();
  }

  private async initializeApp(): Promise<void> {
    try {
      // Initialize theme
      this.themeService.initializeTheme();
      
      // Check authentication status
      await this.authService.checkAuthStatus();
      
      // Load user preferences if authenticated
      if (this.authService.isAuthenticated()) {
        await this.loadUserPreferences();
      }
    } catch (error) {
      console.error('Error initializing app:', error);
    }
  }

  private async loadUserPreferences(): Promise<void> {
    // Load user-specific preferences, shopping cart, etc.
    // This will be implemented when we create the user stores
  }
}
