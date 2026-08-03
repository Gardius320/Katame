import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { Toaster } from '@/shared/components/ui/sonner'
import { ErrorBoundary } from '@/shared/components/error-boundary'
import { ProtectedRoute } from '@/shared/components/protected-route'
import { AppShell } from '@/shared/components/app-shell'
import { useThemeSync } from '@/shared/hooks/use-theme-sync'
import { queryClient } from '@/shared/api/query-client'
import { es } from '@/shared/i18n/es'

const LoginPage = lazy(() =>
  import('@/features/auth/login-page').then((m) => ({ default: m.LoginPage })),
)
const TodayPage = lazy(() => import('@/features/today/today-page'))
const FinancePage = lazy(() => import('@/features/finance/finance-page'))
const TrainingPage = lazy(() => import('@/features/training/training-page'))
const TasksPage = lazy(() => import('@/features/tasks/tasks-page'))
const GoalsPage = lazy(() => import('@/features/goals/goals-page'))
const ProjectsPage = lazy(() => import('@/features/projects/projects-page'))
const SubscriptionsPage = lazy(() => import('@/features/subscriptions/subscriptions-page'))

function RouteFallback() {
  return <div className="p-10 text-center text-muted-foreground">{es.common.loading}</div>
}

function AppRoutes() {
  useThemeSync()

  return (
    <Suspense fallback={<RouteFallback />}>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedRoute />}>
          <Route element={<AppShell />}>
            <Route index element={<Navigate to="/today" replace />} />
            <Route path="today" element={<TodayPage />} />
            <Route path="finance" element={<FinancePage />} />
            <Route path="training" element={<TrainingPage />} />
            <Route path="tasks" element={<TasksPage />} />
            <Route path="goals" element={<GoalsPage />} />
            <Route path="projects" element={<ProjectsPage />} />
            <Route path="subscriptions" element={<SubscriptionsPage />} />
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/today" replace />} />
      </Routes>
    </Suspense>
  )
}

function App() {
  return (
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
        <Toaster richColors position="bottom-right" />
        {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
      </QueryClientProvider>
    </ErrorBoundary>
  )
}

export default App
