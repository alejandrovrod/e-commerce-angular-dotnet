import { Injectable, signal, computed, effect } from '@angular/core';

export interface DashboardStats {
  totalSales: number;
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  averageOrderValue: number;
  conversionRate: number;
}

export interface SalesData {
  date: string;
  sales: number;
  orders: number;
}

export interface TopProduct {
  id: string;
  name: string;
  sales: number;
  revenue: number;
  image?: string;
}

export interface RecentOrder {
  id: string;
  customerName: string;
  total: number;
  status: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled';
  date: Date;
}

export interface AdminState {
  dashboardStats: DashboardStats;
  salesData: SalesData[];
  topProducts: TopProduct[];
  recentOrders: RecentOrder[];
  isLoading: boolean;
  error: string | null;
  selectedPeriod: '7d' | '30d' | '90d' | '1y';
}

@Injectable({
  providedIn: 'root'
})
export class AdminStore {
  // Signals para el estado reactivo
  private readonly _dashboardStats = signal<DashboardStats>({
    totalSales: 0,
    totalOrders: 0,
    totalCustomers: 0,
    totalProducts: 0,
    averageOrderValue: 0,
    conversionRate: 0
  });
  private readonly _salesData = signal<SalesData[]>([]);
  private readonly _topProducts = signal<TopProduct[]>([]);
  private readonly _recentOrders = signal<RecentOrder[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _selectedPeriod = signal<'7d' | '30d' | '90d' | '1y'>('30d');

  // Computed signals
  public readonly dashboardStats = this._dashboardStats.asReadonly();
  public readonly salesData = this._salesData.asReadonly();
  public readonly topProducts = this._dashboardStats.asReadonly();
  public readonly recentOrders = this._recentOrders.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly error = this._error.asReadonly();
  public readonly selectedPeriod = this._selectedPeriod.asReadonly();

  public readonly totalRevenue = computed(() => this._dashboardStats().totalSales);
  public readonly orderCount = computed(() => this._dashboardStats().totalOrders);
  public readonly customerCount = computed(() => this._dashboardStats().totalCustomers);
  public readonly productCount = computed(() => this._dashboardStats().totalProducts);
  public readonly avgOrderValue = computed(() => this._dashboardStats().averageOrderValue);
  public readonly conversion = computed(() => this._dashboardStats().conversionRate);

  public readonly salesChartData = computed(() => {
    const data = this._salesData();
    return {
      labels: data.map(d => d.date),
      datasets: [
        {
          label: 'Ventas',
          data: data.map(d => d.sales),
          borderColor: '#3b82f6',
          backgroundColor: 'rgba(59, 130, 246, 0.1)',
          tension: 0.4
        },
        {
          label: 'Pedidos',
          data: data.map(d => d.orders),
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4
        }
      ]
    };
  });

  public readonly topProductsByRevenue = computed(() => {
    return [...this._topProducts()].sort((a, b) => b.revenue - a.revenue);
  });

  public readonly topProductsBySales = computed(() => {
    return [...this._topProducts()].sort((a, b) => b.sales - a.sales);
  });

  public readonly pendingOrders = computed(() => {
    return this._recentOrders().filter(order => order.status === 'pending');
  });

  public readonly processingOrders = computed(() => {
    return this._recentOrders().filter(order => order.status === 'processing');
  });

  public readonly completedOrders = computed(() => {
    return this._recentOrders().filter(order => order.status === 'delivered');
  });

  constructor() {
    // Cargar datos de ejemplo al inicializar
    this.loadSampleData();
  }

  // Actions
  setDashboardStats(stats: DashboardStats): void {
    this._dashboardStats.set(stats);
    this._error.set(null);
  }

  setSalesData(data: SalesData[]): void {
    this._salesData.set(data);
  }

  setTopProducts(products: TopProduct[]): void {
    this._topProducts.set(products);
  }

  setRecentOrders(orders: RecentOrder[]): void {
    this._recentOrders.set(orders);
  }

  setLoading(loading: boolean): void {
    this._isLoading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  setPeriod(period: '7d' | '30d' | '90d' | '1y'): void {
    this._selectedPeriod.set(period);
    // Aquí podrías recargar los datos basados en el período
    this.loadDataForPeriod(period);
  }

  // Helpers
  private loadSampleData(): void {
    // Datos de ejemplo para el dashboard
    const sampleStats: DashboardStats = {
      totalSales: 125000,
      totalOrders: 1247,
      totalCustomers: 892,
      totalProducts: 156,
      averageOrderValue: 100.24,
      conversionRate: 3.2
    };

    const sampleSalesData: SalesData[] = [
      { date: '2024-01-01', sales: 12000, orders: 45 },
      { date: '2024-01-02', sales: 15000, orders: 52 },
      { date: '2024-01-03', sales: 18000, orders: 61 },
      { date: '2024-01-04', sales: 14000, orders: 48 },
      { date: '2024-01-05', sales: 22000, orders: 73 },
      { date: '2024-01-06', sales: 19000, orders: 65 },
      { date: '2024-01-07', sales: 25000, orders: 82 }
    ];

    const sampleTopProducts: TopProduct[] = [
      { id: '1', name: 'Laptop Gaming Pro', sales: 45, revenue: 58499.55, image: '/assets/images/laptop-1.jpg' },
      { id: '2', name: 'Smartphone Ultra', sales: 38, revenue: 34199.62, image: '/assets/images/phone-1.jpg' },
      { id: '3', name: 'Auriculares Wireless', sales: 67, revenue: 13399.33, image: '/assets/images/headphones-1.jpg' },
      { id: '4', name: 'Tablet Pro', sales: 23, revenue: 22999.77, image: '/assets/images/tablet-1.jpg' },
      { id: '5', name: 'Smartwatch Elite', sales: 41, revenue: 8199.59, image: '/assets/images/watch-1.jpg' }
    ];

    const sampleRecentOrders: RecentOrder[] = [
      { id: 'ORD-001', customerName: 'Juan Pérez', total: 1299.99, status: 'delivered', date: new Date('2024-01-07') },
      { id: 'ORD-002', customerName: 'María García', total: 899.99, status: 'shipped', date: new Date('2024-01-07') },
      { id: 'ORD-003', customerName: 'Carlos López', total: 199.99, status: 'processing', date: new Date('2024-01-06') },
      { id: 'ORD-004', customerName: 'Ana Martínez', total: 2499.99, status: 'pending', date: new Date('2024-01-06') },
      { id: 'ORD-005', customerName: 'Luis Rodríguez', total: 599.99, status: 'delivered', date: new Date('2024-01-05') }
    ];

    this._dashboardStats.set(sampleStats);
    this._salesData.set(sampleSalesData);
    this._topProducts.set(sampleTopProducts);
    this._recentOrders.set(sampleRecentOrders);
  }

  private loadDataForPeriod(period: '7d' | '30d' | '90d' | '1y'): void {
    // Aquí implementarías la lógica para cargar datos del período seleccionado
    // Por ahora solo simulamos el loading
    this.setLoading(true);
    
    setTimeout(() => {
      this.setLoading(false);
      // Aquí cargarías los datos reales del backend
    }, 1000);
  }

  // Métodos de utilidad
  getRevenueGrowth(): number {
    const data = this._salesData();
    if (data.length < 2) return 0;
    
    const current = data[data.length - 1].sales;
    const previous = data[data.length - 2].sales;
    
    if (previous === 0) return 0;
    return ((current - previous) / previous) * 100;
  }

  getOrderGrowth(): number {
    const data = this._salesData();
    if (data.length < 2) return 0;
    
    const current = data[data.length - 1].orders;
    const previous = data[data.length - 2].orders;
    
    if (previous === 0) return 0;
    return ((current - previous) / previous) * 100;
  }

  getTopProduct(): TopProduct | undefined {
    return this._topProducts()[0];
  }

  getLowStockProducts(threshold: number = 10): TopProduct[] {
    // Aquí implementarías la lógica para productos con bajo stock
    return this._topProducts().filter(p => p.sales < threshold);
  }

  // Métodos para actualizar datos en tiempo real
  updateOrderStatus(orderId: string, status: RecentOrder['status']): void {
    this._recentOrders.update(orders =>
      orders.map(order =>
        order.id === orderId ? { ...order, status } : order
      )
    );
  }

  addNewOrder(order: RecentOrder): void {
    this._recentOrders.update(orders => [order, ...orders]);
    
    // Actualizar estadísticas
    const stats = this._dashboardStats();
    this._dashboardStats.set({
      ...stats,
      totalOrders: stats.totalOrders + 1,
      totalSales: stats.totalSales + order.total
    });
  }

  // Métodos para exportar datos
  exportDashboardData(): any {
    return {
      stats: this._dashboardStats(),
      salesData: this._salesData(),
      topProducts: this._topProducts(),
      recentOrders: this._recentOrders(),
      period: this._selectedPeriod(),
      exportDate: new Date()
    };
  }
}
