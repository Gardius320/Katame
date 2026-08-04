import { Check, CircleDashed, PauseCircle } from 'lucide-react'
import { Badge } from '@/shared/components/ui/badge'
import { cn } from '@/shared/lib/utils'
import { es } from '@/shared/i18n/es'
import type { ProjectStatus } from './types'

const statusStyles: Record<ProjectStatus, string> = {
  active: 'bg-primary/15 text-primary border-transparent',
  on_hold: 'bg-muted text-muted-foreground border-transparent',
  completed: 'bg-positive/15 text-positive border-transparent',
}

const statusIcons: Record<ProjectStatus, typeof Check> = {
  active: CircleDashed,
  on_hold: PauseCircle,
  completed: Check,
}

interface ProjectStatusBadgeProps {
  status: ProjectStatus
}

export function ProjectStatusBadge({ status }: ProjectStatusBadgeProps) {
  const Icon = statusIcons[status]

  return (
    <Badge
      key={status}
      className={cn(
        'gap-1 font-medium',
        statusStyles[status],
        status === 'completed' && 'katame-seal',
      )}
    >
      <Icon className="size-3" />
      {es.projects.status[status]}
    </Badge>
  )
}
