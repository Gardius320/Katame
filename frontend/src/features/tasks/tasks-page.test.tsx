import { render, screen, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { http, HttpResponse } from 'msw'
import { describe, expect, it } from 'vitest'
import { server } from '@/test/msw-server'
import TasksPage from './tasks-page'

const API_BASE = import.meta.env.VITE_API_BASE_URL

function renderWithQueryClient() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <TasksPage />
    </QueryClientProvider>,
  )
}

describe('TasksPage', () => {
  it('muestra el estado vacío cuando no hay tareas', async () => {
    server.use(http.get(`${API_BASE}/tasks`, () => HttpResponse.json([])))

    renderWithQueryClient()

    expect(await screen.findByText('No tienes tareas todavía. Crea la primera.')).toBeInTheDocument()
  })

  it('muestra las tareas devueltas por la API', async () => {
    server.use(
      http.get(`${API_BASE}/tasks`, () =>
        HttpResponse.json([
          { id: 1, title: 'Pagar la tarjeta', status: 'pending', date: null, projectId: null },
        ]),
      ),
    )

    renderWithQueryClient()

    await waitFor(() => expect(screen.getByText('Pagar la tarjeta')).toBeInTheDocument())
    expect(screen.getByText('Pendiente')).toBeInTheDocument()
  })
})
