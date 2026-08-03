import { MutationCache, QueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { isApiErrorResponse } from './types'

function errorMessage(error: unknown): string {
  return isApiErrorResponse(error) ? error.message : es.errors.generic
}

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 30_000,
      refetchOnWindowFocus: false,
    },
  },
  mutationCache: new MutationCache({
    onError: (error) => {
      toast.error(errorMessage(error))
    },
  }),
})
