import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { evaluateAchievements, getAchievements } from './api'

const achievementsQueryKey = ['achievements']

export function useAchievements() {
  return useQuery({ queryKey: achievementsQueryKey, queryFn: getAchievements })
}

/**
 * Se llama después de acciones que podrían desbloquear un logro (aportar a
 * una meta, marcar un entrenamiento) y al entrar a las pantallas de
 * Presupuestos o Logros. No muestra toasts propios -- quien la use decide
 * qué hacer con los logros nuevos que devuelve (normalmente, abrir el modal
 * de celebración con el primero).
 */
export function useEvaluateAchievements() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: evaluateAchievements,
    onSuccess: (newlyUnlocked) => {
      if (newlyUnlocked.length > 0) {
        queryClient.invalidateQueries({ queryKey: achievementsQueryKey })
      }
    },
  })
}
