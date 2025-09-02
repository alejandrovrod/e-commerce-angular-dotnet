import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BrandDto {
  id: string;
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  products?: any[];
}

export interface CreateBrandCommand {
  name: string;
  description: string;
  logoUrl?: string;
  website?: string;
  isActive?: boolean;
}

export interface UpdateBrandCommand {
  id: string;
  name: string;
  description: string;
  logoUrl?: string;
  website?: string;
  isActive: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface GetBrandsQuery {
  search?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  private readonly apiUrl = `${environment.apiUrl}/brand`;

  constructor(private http: HttpClient) {}

  // Obtener todas las marcas
  getBrands(query?: GetBrandsQuery): Observable<ApiResponse<BrandDto[]>> {
    const params: any = {};
    if (query?.search) params.search = query.search;
    if (query?.isActive !== undefined) params.isActive = query.isActive.toString();
    if (query?.page) params.page = query.page.toString();
    if (query?.pageSize) params.pageSize = query.pageSize.toString();
    if (query?.sortBy) params.sortBy = query.sortBy;
    if (query?.sortDirection) params.sortDirection = query.sortDirection;

    return this.http.get<ApiResponse<BrandDto[]>>(this.apiUrl, { params });
  }

  // Obtener marca por ID
  getBrandById(id: string): Observable<ApiResponse<BrandDto>> {
    return this.http.get<ApiResponse<BrandDto>>(`${this.apiUrl}/${id}`);
  }

  // Crear nueva marca
  createBrand(command: CreateBrandCommand): Observable<ApiResponse<BrandDto>> {
    return this.http.post<ApiResponse<BrandDto>>(this.apiUrl, command);
  }

  // Actualizar marca
  updateBrand(command: UpdateBrandCommand): Observable<ApiResponse<BrandDto>> {
    return this.http.put<ApiResponse<BrandDto>>(`${this.apiUrl}/${command.id}`, command);
  }

  // Eliminar marca
  deleteBrand(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  // Endpoint de debug (opcional)
  debugBrands(): Observable<any> {
    return this.http.get(`${this.apiUrl}/debug`);
  }
}