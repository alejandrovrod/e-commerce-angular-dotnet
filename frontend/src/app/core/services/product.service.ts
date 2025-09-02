import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProductDto {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: number;
  brand: string;
  categoryId: string;
  status: ProductStatus;
  isFeatured: boolean;
  isDigital: boolean;
  requiresShipping: boolean;
  isTaxable: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateProductCommand {
  name: string;
  description: string;
  sku: string;
  price: number;
  brand: string;
  categoryId: string;
  isFeatured: boolean;
  isDigital: boolean;
  requiresShipping: boolean;
  isTaxable: boolean;
}

export interface ProductFilters {
  category?: string;
  brand?: string;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  isActive?: boolean;
  isFeatured?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export enum ProductStatus {
  Draft = 1,
  Active = 2,
  Inactive = 3,
  OutOfStock = 4,
  Discontinued = 5
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/product`;

  // GET /api/product - Listar productos con filtros
  getProducts(filters: ProductFilters = {}): Observable<ApiResponse<ProductDto[]>> {
    let params = new HttpParams();
    
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value.toString());
      }
    });

    return this.http.get<ApiResponse<ProductDto[]>>(this.baseUrl, { params });
  }

  // GET /api/product/{id} - Obtener producto por ID
  getProductById(id: string): Observable<ApiResponse<ProductDto>> {
    return this.http.get<ApiResponse<ProductDto>>(`${this.baseUrl}/${id}`);
  }

  // GET /api/product/slug/{slug} - Obtener producto por slug
  getProductBySlug(slug: string): Observable<ApiResponse<ProductDto>> {
    return this.http.get<ApiResponse<ProductDto>>(`${this.baseUrl}/slug/${slug}`);
  }

  // POST /api/product - Crear producto
  createProduct(command: CreateProductCommand): Observable<ApiResponse<ProductDto>> {
    return this.http.post<ApiResponse<ProductDto>>(this.baseUrl, command);
  }

  // PUT /api/product/{id} - Actualizar producto (cuando esté implementado)
  updateProduct(id: string, command: CreateProductCommand): Observable<ApiResponse<ProductDto>> {
    return this.http.put<ApiResponse<ProductDto>>(`${this.baseUrl}/${id}`, command);
  }

  // DELETE /api/product/{id} - Eliminar producto
  deleteProduct(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  // PATCH /api/product/{id}/status - Actualizar estado
  updateProductStatus(id: string, status: ProductStatus): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.baseUrl}/${id}/status`, status.toString());
  }

  // PATCH /api/product/{id}/featured - Toggle destacado
  toggleProductFeatured(id: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.baseUrl}/${id}/featured`, {});
  }

  // GET /api/product/debug - Debug de productos
  debugProducts(): Observable<any> {
    return this.http.get(`${this.baseUrl}/debug`);
  }
}
