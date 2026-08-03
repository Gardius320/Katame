import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export interface AuthSession {
  username: string
  accessToken: string
  refreshToken: string
  accessTokenExpiry: string
}

interface SessionState {
  username: string | null
  accessToken: string | null
  refreshToken: string | null
  accessTokenExpiry: string | null
  setSession: (session: AuthSession) => void
  clearSession: () => void
}

export const useSessionStore = create<SessionState>()(
  persist(
    (set) => ({
      username: null,
      accessToken: null,
      refreshToken: null,
      accessTokenExpiry: null,
      setSession: (session) =>
        set({
          username: session.username,
          accessToken: session.accessToken,
          refreshToken: session.refreshToken,
          accessTokenExpiry: session.accessTokenExpiry,
        }),
      clearSession: () =>
        set({
          username: null,
          accessToken: null,
          refreshToken: null,
          accessTokenExpiry: null,
        }),
    }),
    { name: 'katame-session' },
  ),
)

export function isSessionValid(): boolean {
  const { accessToken, refreshToken } = useSessionStore.getState()
  return Boolean(accessToken && refreshToken)
}
