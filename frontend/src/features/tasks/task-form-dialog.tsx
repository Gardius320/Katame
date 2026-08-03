import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateTask, useUpdateTask } from './hooks'
import type { TaskItem, TaskStatus } from './types'
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

const taskFormSchema = z.object({
  title: z
    .string()
    .min(1, es.tasks.validation.titleRequired)
    .max(150, es.tasks.validation.titleMaxLength),
  status: z.enum(['pending', 'in_progress', 'done']),
  date: z.string(),
})

type TaskFormValues = z.infer<typeof taskFormSchema>

const statusOptions: TaskStatus[] = ['pending', 'in_progress', 'done']

function toDateInputValue(date: string | null): string {
  if (!date) return ''
  return date.slice(0, 10)
}

interface TaskFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  task?: TaskItem | null
}

export function TaskFormDialog({ open, onOpenChange, task }: TaskFormDialogProps) {
  const isEditing = Boolean(task)
  const createTask = useCreateTask()
  const updateTask = useUpdateTask()
  const mutation = isEditing ? updateTask : createTask

  const form = useForm<TaskFormValues>({
    resolver: zodResolver(taskFormSchema),
    defaultValues: { title: '', status: 'pending', date: '' },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        title: task?.title ?? '',
        status: task?.status ?? 'pending',
        date: toDateInputValue(task?.date ?? null),
      })
    }
  }, [open, task, form])

  const onSubmit = form.handleSubmit((values) => {
    const payload = {
      title: values.title,
      status: values.status,
      date: values.date ? new Date(values.date).toISOString() : null,
    }

    const onSuccess = () => onOpenChange(false)

    if (isEditing && task) {
      updateTask.mutate({ id: task.id, payload }, { onSuccess })
    } else {
      createTask.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.tasks.editTask : es.tasks.newTask}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.tasks.fields.title}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.tasks.fields.titlePlaceholder} autoFocus {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="status"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.tasks.fields.status}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {statusOptions.map((status) => (
                        <SelectItem key={status} value={status}>
                          {es.tasks.status[status]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="date"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.tasks.fields.date}</FormLabel>
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
