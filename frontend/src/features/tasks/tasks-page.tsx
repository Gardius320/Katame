import { useState } from 'react'
import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useDeleteTask, useTasks } from './hooks'
import { TaskFormDialog } from './task-form-dialog'
import { TaskStatusBadge } from './task-status-badge'
import type { TaskItem } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
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

function formatTaskDate(date: string | null): string | null {
  if (!date) return null
  return format(new Date(date), "d 'de' MMMM", { locale: dateFnsEs })
}

export default function TasksPage() {
  const { data: tasks, isLoading } = useTasks()
  const deleteTask = useDeleteTask()

  const [formOpen, setFormOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<TaskItem | null>(null)
  const [taskToDelete, setTaskToDelete] = useState<TaskItem | null>(null)

  const openCreateForm = () => {
    setEditingTask(null)
    setFormOpen(true)
  }

  const openEditForm = (task: TaskItem) => {
    setEditingTask(task)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!taskToDelete) return
    deleteTask.mutate(taskToDelete.id, { onSuccess: () => setTaskToDelete(null) })
  }

  return (
    <div className="grid gap-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:gap-4">
        <div className="min-w-0">
          <h1 className="font-heading text-2xl font-semibold">{es.tasks.title}</h1>
          <p className="text-muted-foreground">{es.tasks.subtitle}</p>
        </div>
        <Button onClick={openCreateForm} className="w-full sm:w-auto">
          <Plus className="size-4" />
          {es.tasks.newTask}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-3">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-16 w-full rounded-xl" />
          ))}
        </div>
      ) : tasks && tasks.length > 0 ? (
        <div className="grid gap-3">
          {tasks.map((task) => {
            const formattedDate = formatTaskDate(task.date)
            return (
              <Card key={task.id} className="flex-row items-center justify-between gap-4 px-4 py-4">
                <div className="flex min-w-0 items-center gap-3">
                  <TaskStatusBadge status={task.status} />
                  <div className="min-w-0">
                    <p className="truncate font-medium">{task.title}</p>
                    {formattedDate && (
                      <p className="font-numeric text-xs text-muted-foreground">{formattedDate}</p>
                    )}
                  </div>
                </div>
                <div className="flex shrink-0 items-center gap-1">
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.edit}
                    title={es.common.edit}
                    onClick={() => openEditForm(task)}
                  >
                    <Pencil className="size-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.delete}
                    title={es.common.delete}
                    onClick={() => setTaskToDelete(task)}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </Card>
            )
          })}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">{es.tasks.emptyState}</Card>
      )}

      <TaskFormDialog open={formOpen} onOpenChange={setFormOpen} task={editingTask} />

      <AlertDialog
        open={taskToDelete !== null}
        onOpenChange={(open) => !open && setTaskToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteTask.isPending}>
              {deleteTask.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
