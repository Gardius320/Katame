export type TaskStatus = 'pending' | 'in_progress' | 'done'

export interface TaskItem {
  id: number
  title: string
  status: TaskStatus
  date: string | null
  projectId: number | null
}

export interface CreateTaskItemRequest {
  title: string
  status: TaskStatus
  date?: string | null
}

export interface UpdateTaskItemRequest {
  title: string
  status: TaskStatus
  date?: string | null
}
