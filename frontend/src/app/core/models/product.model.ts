export interface ProductImage {
  id: string;
  url: string;
  alt: string;
  isPrimary: boolean;
  order: number;
}

export interface ProductVariant {
  id: string;
  name: string;
  value: string;
  price?: number; // Additional price for this variant
  stock?: number; // Stock specific to this variant
  sku?: string;
}

export interface ProductCategory {
  id: string;
  name: string;
  slug: string;
  description?: string;
  image?: string;
  parentId?: string;
  level: number;
  isActive: boolean;
  productCount: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface ProductReview {
  id: string;
  userId: string;
  userName: string;
  userAvatar?: string;
  rating: number;
  title: string;
  comment: string;
  pros?: string[];
  cons?: string[];
  isVerifiedPurchase: boolean;
  helpfulCount: number;
  images?: ProductImage[];
  createdAt: Date;
  updatedAt: Date;
}

export interface ProductSpecification {
  name: string;
  value: string;
  group?: string;
}

export interface Product {
  id: string;
  name: string;
  slug: string;
  description: string;
  shortDescription?: string;
  sku: string;
  price: number;
  compareAtPrice?: number; // Original price for showing discounts
  costPrice?: number; // For admin/internal use
  brand: string;
  category: ProductCategory;
  images: ProductImage[];
  variants: ProductVariant[];
  specifications: ProductSpecification[];
  tags: string[];
  weight?: number;
  dimensions?: {
    length: number;
    width: number;
    height: number;
    unit: 'cm' | 'in';
  };
  stock: number;
  lowStockThreshold: number;
  trackQuantity: boolean;
  allowBackorder: boolean;
  isActive: boolean;
  isFeatured: boolean;
  isDigital: boolean;
  requiresShipping: boolean;
  taxable: boolean;
  metaTitle?: string;
  metaDescription?: string;
  rating: number;
  reviewCount: number;
  totalSales: number;
  viewCount: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface ProductFilter {
  categoryId?: string;
  brands?: string[];
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  inStock?: boolean;
  isFeatured?: boolean;
  tags?: string[];
  hasDiscount?: boolean;
}

export interface ProductSort {
  field: 'name' | 'price' | 'rating' | 'createdAt' | 'totalSales' | 'category.name';
  direction: 'asc' | 'desc';
}

export interface ProductSearchParams {
  query?: string;
  categoryId?: string;
  filters?: ProductFilter;
  sort?: ProductSort;
  page?: number;
  pageSize?: number;
}

export interface ProductListResponse {
  products: Product[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  shortDescription?: string;
  sku: string;
  price: number;
  compareAtPrice?: number;
  brand: string;
  categoryId: string;
  images: File[];
  variants?: Omit<ProductVariant, 'id'>[];
  specifications?: ProductSpecification[];
  tags?: string[];
  weight?: number;
  dimensions?: Product['dimensions'];
  stock: number;
  lowStockThreshold?: number;
  trackQuantity?: boolean;
  allowBackorder?: boolean;
  isActive?: boolean;
  isFeatured?: boolean;
  isDigital?: boolean;
  requiresShipping?: boolean;
  taxable?: boolean;
  metaTitle?: string;
  metaDescription?: string;
}

export interface UpdateProductRequest extends Partial<CreateProductRequest> {
  id: string;
}

export interface ProductStats {
  totalProducts: number;
  activeProducts: number;
  featuredProducts: number;
  outOfStockProducts: number;
  lowStockProducts: number;
  averageRating: number;
  totalReviews: number;
  totalSales: number;
  totalRevenue: number;
}

export interface CategoryTree extends ProductCategory {
  children: CategoryTree[];
}

export interface ProductAnalytics {
  productId: string;
  views: number;
  sales: number;
  revenue: number;
  conversionRate: number;
  averageRating: number;
  reviewCount: number;
  period: 'day' | 'week' | 'month' | 'year';
  date: Date;
}
