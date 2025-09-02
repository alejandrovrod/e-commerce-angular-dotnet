import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CategoryDto {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  parentCategoryId?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateCategoryCommand {
  name: string;
  description: string;
  parentCategoryId?: string;
}

export interface UpdateCategoryCommand {
  id: string;
  name: string;
  description: string;
  parentCategoryId?: string;
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
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/category`;

  // GET /api/category - Listar categorías
  getCategories(): Observable<ApiResponse<CategoryDto[]>> {
    return this.http.get<ApiResponse<CategoryDto[]>>(this.baseUrl);
  }

  // GET /api/category/{id} - Obtener categoría por ID
  getCategoryById(id: string): Observable<ApiResponse<CategoryDto>> {
    return this.http.get<ApiResponse<CategoryDto>>(`${this.baseUrl}/${id}`);
  }

  // GET /api/category/{id}/subcategories - Obtener subcategorías
  getSubcategories(parentId: string): Observable<ApiResponse<CategoryDto[]>> {
    return this.http.get<ApiResponse<CategoryDto[]>>(`${this.baseUrl}/${parentId}/subcategories`);
  }

  // POST /api/category - Crear categoría
  createCategory(command: CreateCategoryCommand): Observable<ApiResponse<CategoryDto>> {
    return this.http.post<ApiResponse<CategoryDto>>(this.baseUrl, command);
  }

  // PUT /api/category/{id} - Actualizar categoría
  updateCategory(id: string, command: UpdateCategoryCommand): Observable<ApiResponse<CategoryDto>> {
    return this.http.put<ApiResponse<CategoryDto>>(`${this.baseUrl}/${id}`, command);
  }

  // DELETE /api/category/{id} - Eliminar categoría
  deleteCategory(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  // GET /api/category/debug - Debug de categorías
  debugCategories(): Observable<any> {
    return this.http.get(`${this.baseUrl}/debug`);
  }
}
