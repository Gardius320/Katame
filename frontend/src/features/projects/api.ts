import { apiClient } from '@/shared/api/client'
import type { Project, ProjectFormValues } from './types'

export async function getProjects(): Promise<Project[]> {
  const { data } = await apiClient.get<Project[]>('/projects')
  return data
}

export async function createProject(payload: ProjectFormValues): Promise<Project> {
  const { data } = await apiClient.post<Project>('/projects', payload)
  return data
}

export async function updateProject(id: number, payload: ProjectFormValues): Promise<Project> {
  const { data } = await apiClient.put<Project>(`/projects/${id}`, payload)
  return data
}

export async function deleteProject(id: number): Promise<void> {
  await apiClient.delete(`/projects/${id}`)
}
