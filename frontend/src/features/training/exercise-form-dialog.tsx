import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useAddExercise, useUpdateExercise } from './hooks'
import type { Exercise } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/shared/components/ui/dialog'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/components/ui/form'

const exerciseFormSchema = z.object({
  name: z
    .string()
    .min(1, es.training.exercises.validation.nameRequired)
    .max(100, es.training.exercises.validation.nameMaxLength),
  setsReps: z
    .string()
    .min(1, es.training.exercises.validation.setsRepsRequired)
    .max(50, es.training.exercises.validation.setsRepsMaxLength),
})

type ExerciseFormValues = z.infer<typeof exerciseFormSchema>

interface ExerciseFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  dayId: number
  exercise?: Exercise | null
}

export function ExerciseFormDialog({
  open,
  onOpenChange,
  dayId,
  exercise,
}: ExerciseFormDialogProps) {
  const isEditing = Boolean(exercise)
  const addExercise = useAddExercise()
  const updateExercise = useUpdateExercise()
  const mutation = isEditing ? updateExercise : addExercise

  const form = useForm<ExerciseFormValues>({
    resolver: zodResolver(exerciseFormSchema),
    defaultValues: { name: '', setsReps: '' },
  })

  useEffect(() => {
    if (open) {
      form.reset({ name: exercise?.name ?? '', setsReps: exercise?.setsReps ?? '' })
    }
  }, [open, exercise, form])

  const onSubmit = form.handleSubmit((values) => {
    const onSuccess = () => onOpenChange(false)

    if (isEditing && exercise) {
      updateExercise.mutate({ dayId, exerciseId: exercise.id, payload: values }, { onSuccess })
    } else {
      addExercise.mutate({ dayId, payload: values }, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.training.exercises.editExercise : es.training.exercises.newExercise}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.training.exercises.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.training.exercises.namePlaceholder}
                      autoFocus
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="setsReps"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.training.exercises.setsReps}</FormLabel>
                  <FormControl>
                    <Input
                      className="font-numeric"
                      placeholder={es.training.exercises.setsRepsPlaceholder}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <DialogFooter className="mt-2">
              <Button type="submit" disabled={mutation.isPending} className="w-full sm:w-auto">
                {mutation.isPending ? es.common.saving : es.common.save}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
