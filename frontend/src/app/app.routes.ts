import { Routes } from '@angular/router';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent),
    title: 'Home - E-Commerce'
  },

  // Error pages
  {
    path: '404',
    loadComponent: () => import('./shared/pages/not-found/not-found.component').then(m => m.NotFoundComponent),
    title: 'Page Not Found - E-Commerce'
  },
  {
    path: '500',
    loadComponent: () => import('./shared/pages/server-error/server-error.component').then(m => m.ServerErrorComponent),
    title: 'Server Error - E-Commerce'
  },

  // Wildcard route - must be last
  {
    path: '**',
    redirectTo: '/404'
  }
];
