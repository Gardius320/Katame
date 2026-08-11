export interface User {
  id: number
  username: string
  firstName: string
  lastName: string
  documentId: string
  phoneNumber: string
  email: string
  isAdmin: boolean
  createdAt: string
}

export interface CreateUserRequest {
  username: string
  firstName: string
  lastName: string
  documentId: string
  phoneNumber: string
  email: string
  password: string
  isAdmin: boolean
}

export interface UpdateUserRequest {
  username: string
  firstName: string
  lastName: string
  documentId: string
  phoneNumber: string
  email: string
  password: string | null
  isAdmin: boolean
}
