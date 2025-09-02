import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface InventoryDto {
  id: string;
  productId: string;
  quantity: number;
  reservedQuantity: number;
  availableQuantity: number;
  minimumStock: number;
  maximumStock: number;
  reorderPoint: number;
  lastUpdated: Date;
  location?: string;
  notes?: string;
}

export interface CreateInventoryCommand {
  productId: string;
  quantity: number;
  minimumStock: number;
  maximumStock: number;
  reorderPoint: number;
  location?: string;
  notes?: string;
}

export interface UpdateInventoryCommand {
  id: string;
  quantity: number;
  minimumStock: number;
  maximumStock: number;
  reorderPoint: number;
  location?: string;
  notes?: string;
}

export interface AdjustStockCommand {
  productId: string;
  quantity: number;
  reason: string;
  notes?: string;
}

export interface ReserveStockCommand {
  productId: string;
  quantity: number;
  orderId?: string;
  notes?: string;
}

export interface ReleaseStockCommand {
  productId: string;
  quantity: number;
  orderId?: string;
  notes?: string;
}

export interface InventoryMovementDto {
  id: string;
  productId: string;
  productName?: string;
  movementType: 'adjustment' | 'reservation' | 'release' | 'sale' | 'return';
  quantity: number;
  previousQuantity: number;
  newQuantity: number;
  reason: string;
  notes?: string;
  userId?: string;
  userName?: string;
  orderId?: string;
  createdAt: Date;
}

export interface InventoryHistoryFilters {
  productId?: string;
  movementType?: string;
  dateFrom?: Date;
  dateTo?: Date;
  userId?: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/inventory`;

  // GET /api/inventory - Listar inventario
  getInventory(): Observable<ApiResponse<InventoryDto[]>> {
    return this.http.get<ApiResponse<InventoryDto[]>>(this.baseUrl);
  }

  // GET /api/inventory/{id} - Obtener inventario por ID
  getInventoryById(id: string): Observable<ApiResponse<InventoryDto>> {
    return this.http.get<ApiResponse<InventoryDto>>(`${this.baseUrl}/${id}`);
  }

  // GET /api/inventory/product/{productId} - Obtener inventario por producto
  getInventoryByProductId(productId: string): Observable<ApiResponse<InventoryDto>> {
    return this.http.get<ApiResponse<InventoryDto>>(`${this.baseUrl}/product/${productId}`);
  }

  // GET /api/inventory/low-stock - Obtener productos con stock bajo
  getLowStock(): Observable<ApiResponse<InventoryDto[]>> {
    return this.http.get<ApiResponse<InventoryDto[]>>(`${this.baseUrl}/low-stock`);
  }

  // POST /api/inventory - Crear registro de inventario
  createInventory(command: CreateInventoryCommand): Observable<ApiResponse<InventoryDto>> {
    return this.http.post<ApiResponse<InventoryDto>>(this.baseUrl, command);
  }

  // PUT /api/inventory/{id} - Actualizar inventario
  updateInventory(id: string, command: UpdateInventoryCommand): Observable<ApiResponse<InventoryDto>> {
    return this.http.put<ApiResponse<InventoryDto>>(`${this.baseUrl}/${id}`, command);
  }

  // PATCH /api/inventory/adjust - Ajustar stock
  adjustStock(command: AdjustStockCommand): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.baseUrl}/adjust`, command);
  }

  // PATCH /api/inventory/reserve - Reservar stock
  reserveStock(command: ReserveStockCommand): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.baseUrl}/reserve`, command);
  }

  // PATCH /api/inventory/release - Liberar stock
  releaseStock(command: ReleaseStockCommand): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.baseUrl}/release`, command);
  }

  // GET /api/inventory/history - Obtener historial de movimientos
  getInventoryHistory(filters?: InventoryHistoryFilters): Observable<ApiResponse<InventoryMovementDto[]>> {
    let params = new URLSearchParams();
    
    if (filters) {
      if (filters.productId) params.append('productId', filters.productId);
      if (filters.movementType) params.append('movementType', filters.movementType);
      if (filters.dateFrom) params.append('dateFrom', filters.dateFrom.toISOString());
      if (filters.dateTo) params.append('dateTo', filters.dateTo.toISOString());
      if (filters.userId) params.append('userId', filters.userId);
    }
    
    const queryString = params.toString();
    const url = queryString ? `${this.baseUrl}/history?${queryString}` : `${this.baseUrl}/history`;
    
    return this.http.get<ApiResponse<InventoryMovementDto[]>>(url);
  }

  // GET /api/inventory/history/{productId} - Obtener historial por producto
  getProductInventoryHistory(productId: string): Observable<ApiResponse<InventoryMovementDto[]>> {
    return this.http.get<ApiResponse<InventoryMovementDto[]>>(`${this.baseUrl}/history/${productId}`);
  }

  // GET /api/inventory/debug - Debug de inventario
  debugInventory(): Observable<any> {
    return this.http.get(`${this.baseUrl}/debug`);
  }
}
