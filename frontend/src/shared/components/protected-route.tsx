import { Navigate, Outlet } from 'react-router-dom'
import { useSessionStore } from '@/shared/store/session-store'

export function ProtectedRoute() {
  const accessToken = useSessionStore((state) => state.accessToken)

  if (!accessToken) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}
