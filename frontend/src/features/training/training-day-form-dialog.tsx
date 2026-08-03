import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateTrainingDay, useUpdateTrainingDay } from './hooks'
import type { DayOfWeek, TrainingDay } from './types'
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

const dayOfWeekOptions: DayOfWeek[] = [
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
  'Sunday',
]

const dayFormSchema = z.object({
  title: z
    .string()
    .min(1, es.training.validation.dayTitleRequired)
    .max(100, es.training.validation.dayTitleMaxLength),
  dayOfWeek: z.enum(['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']),
})

type DayFormValues = z.infer<typeof dayFormSchema>

interface TrainingDayFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  day?: TrainingDay | null
}

export function TrainingDayFormDialog({ open, onOpenChange, day }: TrainingDayFormDialogProps) {
  const isEditing = Boolean(day)
  const createDay = useCreateTrainingDay()
  const updateDay = useUpdateTrainingDay()
  const mutation = isEditing ? updateDay : createDay

  const form = useForm<DayFormValues>({
    resolver: zodResolver(dayFormSchema),
    defaultValues: { title: '', dayOfWeek: 'Monday' },
  })

  useEffect(() => {
    if (open) {
      form.reset({ title: day?.title ?? '', dayOfWeek: day?.dayOfWeek ?? 'Monday' })
    }
  }, [open, day, form])

  const onSubmit = form.handleSubmit((values) => {
    const onSuccess = () => onOpenChange(false)

    if (isEditing && day) {
      updateDay.mutate({ id: day.id, payload: values }, { onSuccess })
    } else {
      createDay.mutate(values, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.training.editDay : es.training.newDay}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.training.fields.dayTitle}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.training.fields.dayTitlePlaceholder}
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
              name="dayOfWeek"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.training.fields.dayOfWeek}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {dayOfWeekOptions.map((day) => (
                        <SelectItem key={day} value={day}>
                          {es.training.days[day]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
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
