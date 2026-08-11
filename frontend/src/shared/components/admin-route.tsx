import { Navigate, Outlet } from 'react-router-dom'
import { useSessionStore } from '@/shared/store/session-store'

export function AdminRoute() {
  const isAdmin = useSessionStore((state) => state.isAdmin)

  if (!isAdmin) {
    return <Navigate to="/today" replace />
  }

  return <Outlet />
}
