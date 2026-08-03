import { useState } from 'react'
import { Dumbbell, Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useDeleteExercise, useDeleteTrainingDay, useTrainingDays } from './hooks'
import { TrainingDayFormDialog } from './training-day-form-dialog'
import { ExerciseFormDialog } from './exercise-form-dialog'
import type { Exercise, TrainingDay } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Skeleton } from '@/shared/components/ui/skeleton'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog'

interface ExerciseTarget {
  dayId: number
  exercise: Exercise
}

export default function TrainingPage() {
  const { data: days, isLoading } = useTrainingDays()
  const deleteDay = useDeleteTrainingDay()
  const deleteExercise = useDeleteExercise()

  const [dayFormOpen, setDayFormOpen] = useState(false)
  const [editingDay, setEditingDay] = useState<TrainingDay | null>(null)
  const [dayToDelete, setDayToDelete] = useState<TrainingDay | null>(null)

  const [exerciseFormOpen, setExerciseFormOpen] = useState(false)
  const [exerciseFormDayId, setExerciseFormDayId] = useState<number | null>(null)
  const [editingExercise, setEditingExercise] = useState<Exercise | null>(null)
  const [exerciseToDelete, setExerciseToDelete] = useState<ExerciseTarget | null>(null)

  const openCreateDay = () => {
    setEditingDay(null)
    setDayFormOpen(true)
  }

  const openEditDay = (day: TrainingDay) => {
    setEditingDay(day)
    setDayFormOpen(true)
  }

  const openCreateExercise = (dayId: number) => {
    setExerciseFormDayId(dayId)
    setEditingExercise(null)
    setExerciseFormOpen(true)
  }

  const openEditExercise = (dayId: number, exercise: Exercise) => {
    setExerciseFormDayId(dayId)
    setEditingExercise(exercise)
    setExerciseFormOpen(true)
  }

  const confirmDeleteDay = () => {
    if (!dayToDelete) return
    deleteDay.mutate(dayToDelete.id, { onSuccess: () => setDayToDelete(null) })
  }

  const confirmDeleteExercise = () => {
    if (!exerciseToDelete) return
    deleteExercise.mutate(
      { dayId: exerciseToDelete.dayId, exerciseId: exerciseToDelete.exercise.id },
      { onSuccess: () => setExerciseToDelete(null) },
    )
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.training.title}</h1>
          <p className="text-muted-foreground">{es.training.subtitle}</p>
        </div>
        <Button onClick={openCreateDay}>
          <Plus className="size-4" />
          {es.training.newDay}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-48 w-full rounded-xl" />
          ))}
        </div>
      ) : days && days.length > 0 ? (
        <div className="grid gap-4 sm:grid-cols-2">
          {days.map((day) => (
            <Card key={day.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-2">
                <div>
                  <Badge className="mb-2 bg-primary/15 text-primary">
                    {es.training.days[day.dayOfWeek]}
                  </Badge>
                  <CardTitle className="font-heading text-xl">{day.title}</CardTitle>
                </div>
                <div className="flex shrink-0 items-center gap-1">
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.edit}
                    title={es.common.edit}
                    onClick={() => openEditDay(day)}
                  >
                    <Pencil className="size-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.delete}
                    title={es.common.delete}
                    onClick={() => setDayToDelete(day)}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="grid gap-3">
                {day.exercises.length > 0 ? (
                  <ul className="grid gap-2">
                    {day.exercises.map((exercise) => (
                      <li
                        key={exercise.id}
                        className="flex items-center justify-between gap-2 rounded-lg bg-muted px-3 py-2"
                      >
                        <div className="min-w-0">
                          <p className="truncate text-sm font-medium">{exercise.name}</p>
                          <p className="font-numeric text-xs text-muted-foreground">
                            {exercise.setsReps}
                          </p>
                        </div>
                        <div className="flex shrink-0 items-center gap-1">
                          <Button
                            variant="ghost"
                            size="icon-xs"
                            aria-label={es.common.edit}
                            title={es.common.edit}
                            onClick={() => openEditExercise(day.id, exercise)}
                          >
                            <Pencil className="size-3.5" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon-xs"
                            aria-label={es.common.delete}
                            title={es.common.delete}
                            onClick={() => setExerciseToDelete({ dayId: day.id, exercise })}
                          >
                            <Trash2 className="size-3.5" />
                          </Button>
                        </div>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-sm text-muted-foreground">
                    {es.training.exercises.emptyState}
                  </p>
                )}

                <Button variant="outline" size="sm" onClick={() => openCreateExercise(day.id)}>
                  <Dumbbell className="size-4" />
                  {es.training.exercises.newExercise}
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">{es.training.emptyState}</Card>
      )}

      <TrainingDayFormDialog open={dayFormOpen} onOpenChange={setDayFormOpen} day={editingDay} />

      {exerciseFormDayId !== null && (
        <ExerciseFormDialog
          open={exerciseFormOpen}
          onOpenChange={setExerciseFormOpen}
          dayId={exerciseFormDayId}
          exercise={editingExercise}
        />
      )}

      <AlertDialog
        open={dayToDelete !== null}
        onOpenChange={(open) => !open && setDayToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {es.training.confirmDeleteDayDescription}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDeleteDay} disabled={deleteDay.isPending}>
              {deleteDay.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <AlertDialog
        open={exerciseToDelete !== null}
        onOpenChange={(open) => !open && setExerciseToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDeleteExercise} disabled={deleteExercise.isPending}>
              {deleteExercise.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
