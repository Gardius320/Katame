import { Check, CircleDashed, Loader2 } from 'lucide-react'
import { Badge } from '@/shared/components/ui/badge'
import { cn } from '@/shared/lib/utils'
import { es } from '@/shared/i18n/es'
import type { TaskStatus } from './types'

const statusStyles: Record<TaskStatus, string> = {
  pending: 'bg-muted text-muted-foreground border-transparent',
  in_progress: 'bg-primary/15 text-primary border-transparent',
  done: 'bg-positive/15 text-positive border-transparent',
}

const statusIcons: Record<TaskStatus, typeof Check> = {
  pending: CircleDashed,
  in_progress: Loader2,
  done: Check,
}

interface TaskStatusBadgeProps {
  status: TaskStatus
}

export function TaskStatusBadge({ status }: TaskStatusBadgeProps) {
  const Icon = statusIcons[status]

  return (
    <Badge
      key={status}
      className={cn('gap-1 font-medium', statusStyles[status], status === 'done' && 'katame-seal')}
    >
      <Icon className={cn('size-3', status === 'in_progress' && 'animate-spin')} />
      {es.tasks.status[status]}
    </Badge>
  )
}
