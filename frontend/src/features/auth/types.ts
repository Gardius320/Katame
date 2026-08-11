export interface LoginRequest {
  username: string
  password: string
}

export interface RefreshRequest {
  refreshToken: string
}

export interface RegisterRequest {
  firstName: string
  lastName: string
  documentId: string
  email: string
  phoneNumber: string
  password: string
}

export interface AuthResponse {
  username: string
  firstName: string
  isAdmin: boolean
  accessToken: string
  refreshToken: string
  accessTokenExpiry: string
}

export interface ForgotPasswordRequest {
  email: string
}

export interface ResetPasswordRequest {
  token: string
  newPassword: string
}
