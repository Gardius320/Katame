import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { useSessionStore } from '@/shared/store/session-store'
import { es } from '@/shared/i18n/es'
import { isApiErrorResponse, type ApiErrorResponse } from './types'
import type { AuthResponse, RefreshRequest } from '@/features/auth/types'

const baseURL = import.meta.env.VITE_API_BASE_URL

export const apiClient = axios.create({ baseURL })

// Instancia sin interceptores: evita recursión al renovar el token.
const refreshClient = axios.create({ baseURL })

const AUTH_PATH_PREFIX = '/auth'
const REFRESH_MARGIN_MS = 60_000

let refreshPromise: Promise<string | null> | null = null

function isAuthRequest(url?: string): boolean {
  return Boolean(url && url.startsWith(AUTH_PATH_PREFIX))
}

async function refreshAccessToken(): Promise<string | null> {
  const { refreshToken } = useSessionStore.getState()
  if (!refreshToken) return null

  if (!refreshPromise) {
    refreshPromise = refreshClient
      .post<AuthResponse>('/auth/refresh', { refreshToken } satisfies RefreshRequest)
      .then((response) => {
        useSessionStore.getState().setSession(response.data)
        return response.data.accessToken
      })
      .catch(() => {
        useSessionStore.getState().clearSession()
        return null
      })
      .finally(() => {
        refreshPromise = null
      })
  }

  return refreshPromise
}

apiClient.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
  if (isAuthRequest(config.url)) {
    return config
  }

  const { accessToken, accessTokenExpiry } = useSessionStore.getState()
  let tokenToUse = accessToken

  if (accessToken && accessTokenExpiry) {
    const expiresInMs = new Date(accessTokenExpiry).getTime() - Date.now()
    if (expiresInMs < REFRESH_MARGIN_MS) {
      tokenToUse = (await refreshAccessToken()) ?? accessToken
    }
  }

  if (tokenToUse) {
    config.headers.set('Authorization', `Bearer ${tokenToUse}`)
  }

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiErrorResponse>) => {
    const originalRequest = error.config as
      (InternalAxiosRequestConfig & { _retried?: boolean }) | undefined

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retried &&
      !isAuthRequest(originalRequest.url)
    ) {
      originalRequest._retried = true
      const newToken = await refreshAccessToken()

      if (newToken) {
        originalRequest.headers.set('Authorization', `Bearer ${newToken}`)
        return apiClient(originalRequest)
      }

      if (window.location.pathname !== '/login') {
        window.location.assign('/login')
      }
    }

    return Promise.reject(normalizeError(error))
  },
)

function normalizeError(error: AxiosError<ApiErrorResponse>): ApiErrorResponse {
  if (isApiErrorResponse(error.response?.data)) {
    return error.response.data
  }

  if (error.code === 'ERR_NETWORK') {
    return { status: 0, message: es.errors.network, errors: [] }
  }

  return { status: error.response?.status ?? 500, message: es.errors.generic, errors: [] }
}
