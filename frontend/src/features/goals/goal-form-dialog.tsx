import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateGoal, useUpdateGoal } from './hooks'
import type { Goal } from './types'
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

const goalFormSchema = z.object({
  title: z
    .string()
    .min(1, es.goals.validation.titleRequired)
    .max(150, es.goals.validation.titleMaxLength),
  category: z
    .string()
    .min(1, es.goals.validation.categoryRequired)
    .max(50, es.goals.validation.categoryMaxLength),
  progressPercentage: z
    .number()
    .min(0, es.goals.validation.progressRange)
    .max(100, es.goals.validation.progressRange),
  dueDate: z.string(),
})

type GoalFormSchema = z.infer<typeof goalFormSchema>

function toDateInputValue(date: string | null): string {
  if (!date) return ''
  return date.slice(0, 10)
}

interface GoalFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  goal?: Goal | null
}

export function GoalFormDialog({ open, onOpenChange, goal }: GoalFormDialogProps) {
  const isEditing = Boolean(goal)
  const createGoal = useCreateGoal()
  const updateGoal = useUpdateGoal()
  const mutation = isEditing ? updateGoal : createGoal

  const form = useForm<GoalFormSchema>({
    resolver: zodResolver(goalFormSchema),
    defaultValues: { title: '', category: '', progressPercentage: 0, dueDate: '' },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        title: goal?.title ?? '',
        category: goal?.category ?? '',
        progressPercentage: goal?.progressPercentage ?? 0,
        dueDate: toDateInputValue(goal?.dueDate ?? null),
      })
    }
  }, [open, goal, form])

  const onSubmit = form.handleSubmit((values) => {
    const payload = {
      title: values.title,
      category: values.category,
      progressPercentage: values.progressPercentage,
      dueDate: values.dueDate ? new Date(values.dueDate).toISOString() : null,
    }

    const onSuccess = () => onOpenChange(false)

    if (isEditing && goal) {
      updateGoal.mutate({ id: goal.id, payload }, { onSuccess })
    } else {
      createGoal.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.goals.editGoal : es.goals.newGoal}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.goals.fields.title}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.goals.fields.titlePlaceholder} autoFocus {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="category"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.goals.fields.category}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.goals.fields.categoryPlaceholder} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="progressPercentage"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.goals.fields.progressPercentage}</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      min={0}
                      max={100}
                      className="font-numeric"
                      name={field.name}
                      onBlur={field.onBlur}
                      ref={field.ref}
                      value={field.value}
                      onChange={(e) => field.onChange(e.target.valueAsNumber)}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="dueDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.goals.fields.dueDate}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
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
