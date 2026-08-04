export type ProjectStatus = 'active' | 'on_hold' | 'completed'

export interface Project {
  id: number
  name: string
  description: string
  status: ProjectStatus
}

export interface ProjectFormValues {
  name: string
  description: string
  status: ProjectStatus
}
