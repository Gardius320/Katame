export type DayOfWeek =
  'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday'

export interface Exercise {
  id: number
  trainingDayId: number
  name: string
  setsReps: string
}

export interface TrainingDay {
  id: number
  dayOfWeek: DayOfWeek
  title: string
  exercises: Exercise[]
}

export interface CreateTrainingDayRequest {
  dayOfWeek: DayOfWeek
  title: string
}

export interface UpdateTrainingDayRequest {
  dayOfWeek: DayOfWeek
  title: string
}

export interface ExerciseFormValues {
  name: string
  setsReps: string
}

export interface TrainingStreak {
  currentStreakDays: number
  longestStreakDays: number
  isNewCompletion: boolean
}
