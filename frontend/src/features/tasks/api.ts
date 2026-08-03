import { apiClient } from '@/shared/api/client'
import type { CreateTaskItemRequest, TaskItem, UpdateTaskItemRequest } from './types'

export async function getTasks(): Promise<TaskItem[]> {
  const { data } = await apiClient.get<TaskItem[]>('/tasks')
  return data
}

export async function createTask(payload: CreateTaskItemRequest): Promise<TaskItem> {
  const { data } = await apiClient.post<TaskItem>('/tasks', payload)
  return data
}

export async function updateTask(id: number, payload: UpdateTaskItemRequest): Promise<TaskItem> {
  const { data } = await apiClient.put<TaskItem>(`/tasks/${id}`, payload)
  return data
}

export async function deleteTask(id: number): Promise<void> {
  await apiClient.delete(`/tasks/${id}`)
}
