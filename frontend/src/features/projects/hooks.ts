import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createProject, deleteProject, getProjects, updateProject } from './api'
import type { ProjectFormValues } from './types'

const projectsQueryKey = ['projects']

export function useProjects() {
  return useQuery({ queryKey: projectsQueryKey, queryFn: getProjects })
}

export function useCreateProject() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: ProjectFormValues) => createProject(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: projectsQueryKey })
      toast.success(es.projects.toasts.created)
    },
  })
}

export function useUpdateProject() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: ProjectFormValues }) =>
      updateProject(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: projectsQueryKey })
      toast.success(es.projects.toasts.updated)
    },
  })
}

export function useDeleteProject() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: projectsQueryKey })
      toast.success(es.projects.toasts.deleted)
    },
  })
}
