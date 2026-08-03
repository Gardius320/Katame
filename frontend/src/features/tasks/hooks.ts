import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createTask, deleteTask, getTasks, updateTask } from './api'
import type { CreateTaskItemRequest, UpdateTaskItemRequest } from './types'

const tasksQueryKey = ['tasks']

export function useTasks() {
  return useQuery({ queryKey: tasksQueryKey, queryFn: getTasks })
}

export function useCreateTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateTaskItemRequest) => createTask(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: tasksQueryKey })
      toast.success(es.tasks.toasts.created)
    },
  })
}

export function useUpdateTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateTaskItemRequest }) =>
      updateTask(id, payload),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: tasksQueryKey })
      toast.success(
        variables.payload.status === 'done' ? es.tasks.toasts.completed : es.tasks.toasts.updated,
      )
    },
  })
}

export function useDeleteTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteTask(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: tasksQueryKey })
      toast.success(es.tasks.toasts.deleted)
    },
  })
}
