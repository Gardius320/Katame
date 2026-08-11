import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Area, AreaChart, ResponsiveContainer, Tooltip, YAxis } from 'recharts'
import { CalendarClock, Dumbbell, ListTodo, Wallet } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { TaskStatusBadge } from '@/features/tasks/task-status-badge'
import { useToday } from './hooks'
import { Card } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Skeleton } from '@/shared/components/ui/skeleton'
import { StatCard } from '@/shared/components/ui/stat-card'

function formatDate(date: string): string {
  return format(new Date(date), "d 'de' MMMM", { locale: dateFnsEs })
}

function computeBalanceTrendPercent(trend: { amount: number }[]): number | undefined {
  if (trend.length < 2) return undefined
  const first = trend[0].amount
  const last = trend[trend.length - 1].amount
  if (first === 0) return undefined
  return Math.round(((last - first) / Math.abs(first)) * 100)
}

export default function TodayPage() {
  const { data, isLoading } = useToday()

  if (isLoading || !data) {
    return (
      <div className="grid gap-6">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.today.title}</h1>
          <p className="text-muted-foreground">{es.today.subtitle}</p>
        </div>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-28 w-full rounded-2xl" />
          ))}
        </div>
        <div className="grid gap-4 sm:grid-cols-2">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-48 w-full rounded-xl" />
          ))}
        </div>
      </div>
    )
  }

  const balanceTrendPercent = computeBalanceTrendPercent(data.balanceTrend)

  return (
    <div className="grid gap-6">
      <div>
        <h1 className="font-heading text-2xl font-semibold">{es.today.title}</h1>
        <p className="text-muted-foreground">{es.today.subtitle}</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard
          label={es.today.balance.title}
          value={formatCurrency(data.balance)}
          icon={Wallet}
          variant="hero"
          trend={balanceTrendPercent !== undefined ? { value: balanceTrendPercent } : undefined}
        />
        <StatCard
          label={es.today.upcoming.title}
          value={data.upcomingDueDates.length}
          icon={CalendarClock}
          variant={data.upcomingDueDates.length > 0 ? 'warning' : 'default'}
        />
        <StatCard
          label={es.today.training.title}
          value={data.todayTraining?.exercises.length ?? 0}
          icon={Dumbbell}
        />
        <StatCard
          label={es.today.tasks.title}
          value={data.urgentTasks.length}
          icon={ListTodo}
          variant={data.urgentTasks.length > 0 ? 'warning' : 'default'}
        />
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <Card className="gap-3 p-4">
          <h2 className="font-heading text-sm font-semibold text-muted-foreground">
            {es.today.balance.title}
          </h2>
          <p className="font-numeric text-2xl font-semibold">{formatCurrency(data.balance)}</p>
          <ResponsiveContainer width="100%" height={80}>
            <AreaChart data={data.balanceTrend}>
              <defs>
                <linearGradient id="balanceGradient" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="var(--primary)" stopOpacity={0.4} />
                  <stop offset="95%" stopColor="var(--primary)" stopOpacity={0} />
                </linearGradient>
              </defs>
              <YAxis hide domain={['auto', 'auto']} />
              <Tooltip
                formatter={(value) => formatCurrency(Number(value))}
                labelFormatter={(label) => formatDate(String(label))}
                contentStyle={{
                  backgroundColor: 'var(--popover)',
                  border: '1px solid var(--border)',
                  borderRadius: 8,
                  color: 'var(--popover-foreground)',
                }}
              />
              <Area
                type="monotone"
                dataKey="amount"
                stroke="var(--primary)"
                fill="url(#balanceGradient)"
                strokeWidth={2}
              />
            </AreaChart>
          </ResponsiveContainer>
        </Card>

        <Card className="gap-3 p-4">
          <h2 className="font-heading text-sm font-semibold text-muted-foreground">
            {es.today.upcoming.title}
          </h2>
          {data.upcomingDueDates.length > 0 ? (
            <ul className="grid gap-3">
              {data.upcomingDueDates.map((due, index) => (
                <li key={index} className="flex items-center justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium">{due.name}</p>
                    <div className="flex items-center gap-2">
                      <Badge variant="outline" className="text-xs">
                        {es.today.upcoming.type[due.type]}
                      </Badge>
                      <p className="text-xs text-muted-foreground">{formatDate(due.dueDate)}</p>
                    </div>
                  </div>
                  {due.amount !== null && (
                    <p className="font-numeric shrink-0 text-sm text-destructive">
                      {formatCurrency(due.amount)}
                    </p>
                  )}
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-sm text-muted-foreground">{es.today.upcoming.emptyState}</p>
          )}
        </Card>

        <Card className="gap-3 p-4">
          <h2 className="font-heading text-sm font-semibold text-muted-foreground">
            {es.today.training.title}
          </h2>
          {data.todayTraining ? (
            <div className="grid gap-2">
              <p className="font-heading font-semibold">{data.todayTraining.title}</p>
              <ul className="grid gap-1">
                {data.todayTraining.exercises.map((exercise) => (
                  <li key={exercise.id} className="flex items-center justify-between text-sm">
                    <span className="truncate">{exercise.name}</span>
                    <span className="font-numeric shrink-0 text-muted-foreground">
                      {exercise.setsReps}
                    </span>
                  </li>
                ))}
              </ul>
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">{es.today.training.emptyState}</p>
          )}
        </Card>

        <Card className="gap-3 p-4">
          <h2 className="font-heading text-sm font-semibold text-muted-foreground">
            {es.today.tasks.title}
          </h2>
          {data.urgentTasks.length > 0 ? (
            <ul className="grid gap-2">
              {data.urgentTasks.map((task) => (
                <li key={task.id} className="flex items-center gap-2">
                  <TaskStatusBadge status={task.status} />
                  <span className="truncate text-sm">{task.title}</span>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-sm text-muted-foreground">{es.today.tasks.emptyState}</p>
          )}
        </Card>
      </div>
    </div>
  )
}
