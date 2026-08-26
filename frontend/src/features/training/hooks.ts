import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import {
  addExercise,
  createTrainingDay,
  deleteExercise,
  deleteTrainingDay,
  getTrainingDays,
  getTrainingStreak,
  markTrainingCompleted,
  updateExercise,
  updateTrainingDay,
} from './api'
import type {
  CreateTrainingDayRequest,
  ExerciseFormValues,
  UpdateTrainingDayRequest,
} from './types'

const trainingDaysQueryKey = ['training-days']
const trainingStreakQueryKey = ['training-streak']

export function useTrainingDays() {
  return useQuery({ queryKey: trainingDaysQueryKey, queryFn: getTrainingDays })
}

export function useCreateTrainingDay() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateTrainingDayRequest) => createTrainingDay(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.toasts.dayCreated)
    },
  })
}

export function useUpdateTrainingDay() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateTrainingDayRequest }) =>
      updateTrainingDay(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.toasts.dayUpdated)
    },
  })
}

export function useDeleteTrainingDay() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteTrainingDay(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.toasts.dayDeleted)
    },
  })
}

export function useAddExercise() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ dayId, payload }: { dayId: number; payload: ExerciseFormValues }) =>
      addExercise(dayId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.exercises.toasts.created)
    },
  })
}

export function useUpdateExercise() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      dayId,
      exerciseId,
      payload,
    }: {
      dayId: number
      exerciseId: number
      payload: ExerciseFormValues
    }) => updateExercise(dayId, exerciseId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.exercises.toasts.updated)
    },
  })
}

export function useTrainingStreak() {
  return useQuery({ queryKey: trainingStreakQueryKey, queryFn: getTrainingStreak })
}

export function useMarkTrainingCompleted() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: markTrainingCompleted,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: trainingStreakQueryKey })
      if (!data.isNewCompletion) {
        toast.info(es.training.streak.alreadyMarkedToday)
      }
    },
  })
}

export function useDeleteExercise() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ dayId, exerciseId }: { dayId: number; exerciseId: number }) =>
      deleteExercise(dayId, exerciseId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: trainingDaysQueryKey })
      toast.success(es.training.exercises.toasts.deleted)
    },
  })
}
