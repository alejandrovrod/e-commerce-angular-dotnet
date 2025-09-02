import { Routes } from '@angular/router';
import { AdminAuthGuard } from './core/guards/admin-auth.guard';
import { CustomerAuthGuard } from './core/guards/customer-auth.guard';

export const routes: Routes = [
  // Rutas públicas
  {
    path: '',
    redirectTo: '/shop',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/admin-login/admin-login.component').then(m => m.AdminLoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/customer-register/customer-register.component').then(m => m.CustomerRegisterComponent)
      }
    ]
  },

  // Rutas del Admin (protegidas)
  {
    path: 'admin',
    canActivate: [AdminAuthGuard],
    loadComponent: () => import('./shared/layouts/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./features/admin/products/admin-products.component').then(m => m.AdminProductsComponent)
      },
      {
        path: 'products/create',
        loadComponent: () => import('./features/admin/products/product-form/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'products/edit/:id',
        loadComponent: () => import('./features/admin/products/product-form/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'inventory/adjust',
        loadComponent: () => import('./features/admin/inventory/inventory-adjustment/inventory-adjustment.component').then(m => m.InventoryAdjustmentComponent)
      },
      {
        path: 'inventory/history',
        loadComponent: () => import('./features/admin/inventory/inventory-history/inventory-history.component').then(m => m.InventoryHistoryComponent)
      },
      {
        path: 'inventory/alerts',
        loadComponent: () => import('./features/admin/inventory/low-stock-alerts/low-stock-alerts.component').then(m => m.LowStockAlertsComponent)
      },
      {
        path: 'inventory/dashboard',
        loadComponent: () => import('./features/admin/inventory/inventory-dashboard/inventory-dashboard.component').then(m => m.InventoryDashboardComponent)
      },
      {
        path: 'inventory/bulk',
        loadComponent: () => import('./features/admin/inventory/bulk-inventory-operations/bulk-inventory-operations.component').then(m => m.BulkInventoryOperationsComponent)
      },
      {
        path: 'categories',
        loadComponent: () => import('./features/admin/categories/admin-categories.component').then(m => m.AdminCategoriesComponent)
      },
      {
        path: 'categories/create',
        loadComponent: () => import('./features/admin/categories/category-form/category-form.component').then(m => m.CategoryFormComponent)
      },
      {
        path: 'categories/edit/:id',
        loadComponent: () => import('./features/admin/categories/category-form/category-form.component').then(m => m.CategoryFormComponent)
      },
      {
        path: 'brands',
        loadComponent: () => import('./features/admin/brands/admin-brands.component').then(m => m.AdminBrandsComponent)
      },
      {
        path: 'brands/create',
        loadComponent: () => import('./features/admin/brands/brand-form/brand-form.component').then(m => m.BrandFormComponent)
      },
      {
        path: 'brands/edit/:id',
        loadComponent: () => import('./features/admin/brands/brand-form/brand-form.component').then(m => m.BrandFormComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/admin/orders/admin-orders.component').then(m => m.AdminOrdersComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./features/admin/users/admin-users.component').then(m => m.AdminUsersComponent)
      },
      {
        path: 'analytics',
        loadComponent: () => import('./features/admin/analytics/admin-analytics.component').then(m => m.AdminAnalyticsComponent)
      },
      {
        path: 'settings',
        loadComponent: () => import('./features/admin/settings/admin-settings.component').then(m => m.AdminSettingsComponent)
      }
    ]
  },

  // Rutas del Customer (protegidas)
  {
    path: 'shop',
    loadComponent: () => import('./features/customer/shop/customer-shop.component').then(m => m.CustomerShopComponent)
  },
  {
    path: 'product/:id',
    loadComponent: () => import('./features/customer/product-detail/product-detail.component').then(m => m.ProductDetailComponent)
  },
  {
    path: 'cart',
    canActivate: [CustomerAuthGuard],
    loadComponent: () => import('./features/customer/cart/customer-cart.component').then(m => m.CustomerCartComponent)
  },
  {
    path: 'checkout',
    canActivate: [CustomerAuthGuard],
    loadComponent: () => import('./features/customer/checkout/customer-checkout.component').then(m => m.CustomerCheckoutComponent)
  },
  {
    path: 'orders',
    canActivate: [CustomerAuthGuard],
    loadComponent: () => import('./features/customer/orders/customer-orders.component').then(m => m.CustomerOrdersComponent)
  },
  {
    path: 'profile',
    canActivate: [CustomerAuthGuard],
    loadComponent: () => import('./features/customer/profile/customer-profile.component').then(m => m.CustomerProfileComponent)
  },

  // Ruta de fallback
  {
    path: '**',
    redirectTo: '/shop'
  }
];
