export interface LoginRequest {
  username: string
  password: string
}

export interface RefreshRequest {
  refreshToken: string
}

export interface AuthResponse {
  username: string
  accessToken: string
  refreshToken: string
  accessTokenExpiry: string
}
