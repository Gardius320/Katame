export interface ApiErrorResponse {
  status: number
  message: string
  errors: string[]
}

export function isApiErrorResponse(value: unknown): value is ApiErrorResponse {
  return (
    typeof value === 'object' &&
    value !== null &&
    'message' in value &&
    typeof (value as Record<string, unknown>).message === 'string'
  )
}
