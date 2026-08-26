import { apiClient } from '@/shared/api/client'
import type {
  CreateTrainingDayRequest,
  Exercise,
  ExerciseFormValues,
  TrainingDay,
  TrainingStreak,
  UpdateTrainingDayRequest,
} from './types'

export async function getTrainingDays(): Promise<TrainingDay[]> {
  const { data } = await apiClient.get<TrainingDay[]>('/training/days')
  return data
}

export async function createTrainingDay(payload: CreateTrainingDayRequest): Promise<TrainingDay> {
  const { data } = await apiClient.post<TrainingDay>('/training/days', payload)
  return data
}

export async function updateTrainingDay(
  id: number,
  payload: UpdateTrainingDayRequest,
): Promise<TrainingDay> {
  const { data } = await apiClient.put<TrainingDay>(`/training/days/${id}`, payload)
  return data
}

export async function deleteTrainingDay(id: number): Promise<void> {
  await apiClient.delete(`/training/days/${id}`)
}

export async function addExercise(dayId: number, payload: ExerciseFormValues): Promise<Exercise> {
  const { data } = await apiClient.post<Exercise>(`/training/days/${dayId}/exercises`, payload)
  return data
}

export async function updateExercise(
  dayId: number,
  exerciseId: number,
  payload: ExerciseFormValues,
): Promise<Exercise> {
  const { data } = await apiClient.put<Exercise>(
    `/training/days/${dayId}/exercises/${exerciseId}`,
    payload,
  )
  return data
}

export async function deleteExercise(dayId: number, exerciseId: number): Promise<void> {
  await apiClient.delete(`/training/days/${dayId}/exercises/${exerciseId}`)
}

export async function getTrainingStreak(): Promise<TrainingStreak> {
  const { data } = await apiClient.get<TrainingStreak>('/training/streak')
  return data
}

export async function markTrainingCompleted(): Promise<TrainingStreak> {
  const { data } = await apiClient.post<TrainingStreak>('/training/completions')
  return data
}
