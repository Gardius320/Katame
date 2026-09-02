import { apiClient } from '@/shared/api/client'
import type {
  AuthResponse,
  ForgotPasswordRequest,
  LoginRequest,
  RegisterRequest,
  ResetPasswordRequest,
} from './types'

export async function login(payload: LoginRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>('/auth/login', payload)
  return data
}

export async function register(payload: RegisterRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>('/auth/register', payload)
  return data
}

export async function forgotPassword(payload: ForgotPasswordRequest): Promise<void> {
  await apiClient.post('/auth/forgot-password', payload)
}

export async function resetPassword(payload: ResetPasswordRequest): Promise<void> {
  await apiClient.post('/auth/reset-password', payload)
}

export async function logout(refreshToken: string): Promise<void> {
  await apiClient.post('/auth/logout', { refreshToken })
}
