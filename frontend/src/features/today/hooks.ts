import { useQuery } from '@tanstack/react-query'
import { getToday } from './api'

export function useToday() {
  return useQuery({ queryKey: ['today'], queryFn: getToday })
}
