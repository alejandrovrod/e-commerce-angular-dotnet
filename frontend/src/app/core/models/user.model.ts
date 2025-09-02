export enum UserRole {
  Customer = 'customer',
  Admin = 'admin',
  SuperAdmin = 'super_admin'
}

export enum UserStatus {
  Active = 'active',
  Inactive = 'inactive',
  Suspended = 'suspended',
  PendingVerification = 'pending_verification'
}

export interface Address {
  id: string;
  type: 'billing' | 'shipping';
  firstName: string;
  lastName: string;
  company?: string;
  address1: string;
  address2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  phone?: string;
  isDefault: boolean;
}

export interface UserPreferences {
  theme: 'light' | 'dark' | 'system';
  language: string;
  currency: string;
  emailNotifications: {
    orderUpdates: boolean;
    promotions: boolean;
    newsletter: boolean;
    productRecommendations: boolean;
  };
  pushNotifications: {
    orderUpdates: boolean;
    promotions: boolean;
    recommendations: boolean;
  };
}

// Modelo simplificado para compatibilidad
export interface User {
  id: string;
  email: string;
  name?: string; // Para compatibilidad con AuthStore
  firstName?: string; // Para compatibilidad con UserStore
  lastName?: string; // Para compatibilidad con UserStore
  role: UserRole;
  status?: UserStatus;
  avatar?: string;
  // Propiedades opcionales para funcionalidad avanzada
  phone?: string;
  dateOfBirth?: Date;
  gender?: 'male' | 'female' | 'other' | 'prefer_not_to_say';
  addresses?: Address[];
  preferences?: UserPreferences;
  emailVerified?: boolean;
  phoneVerified?: boolean;
  twoFactorEnabled?: boolean;
  lastLoginAt?: Date;
  createdAt?: Date;
  updatedAt?: Date;
}

// Helper para convertir entre modelos
export function createSimpleUser(user: User): { id: string; email: string; name: string; role: string; avatar?: string } {
  return {
    id: user.id,
    email: user.email,
    name: user.name || `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email,
    role: user.role,
    avatar: user.avatar
  };
}

export function createFullUser(simpleUser: { id: string; email: string; name: string; role: string; avatar?: string }): User {
  const [firstName, ...lastNameParts] = simpleUser.name.split(' ');
  return {
    id: simpleUser.id,
    email: simpleUser.email,
    firstName: firstName || '',
    lastName: lastNameParts.join(' ') || '',
    name: simpleUser.name,
    role: simpleUser.role as UserRole,
    status: UserStatus.Active,
    avatar: simpleUser.avatar,
    emailVerified: true,
    phoneVerified: false,
    twoFactorEnabled: false,
    addresses: [],
    preferences: {
      theme: 'system',
      language: 'es',
      currency: 'USD',
      emailNotifications: {
        orderUpdates: true,
        promotions: true,
        newsletter: true,
        productRecommendations: true
      },
      pushNotifications: {
        orderUpdates: true,
        promotions: true,
        recommendations: true
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  };
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phone?: string;
  acceptTerms: boolean;
}

export interface AuthResponse {
  user: User;
  tokens: AuthTokens;
}

export interface PasswordResetRequest {
  email: string;
}

export interface PasswordChangeRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface UpdateProfileRequest {
  firstName?: string;
  lastName?: string;
  phone?: string;
  dateOfBirth?: Date;
  gender?: string;
  avatar?: string;
}

export interface UserStats {
  totalUsers: number;
  activeUsers: number;
  newUsersThisMonth: number;
  userGrowthRate: number;
}
