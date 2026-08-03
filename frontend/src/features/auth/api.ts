import { apiClient } from '@/shared/api/client'
import type { AuthResponse, LoginRequest } from './types'

export async function login(payload: LoginRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>('/auth/login', payload)
  return data
}
