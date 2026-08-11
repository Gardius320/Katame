import * as React from 'react'
import { cva, type VariantProps } from 'class-variance-authority'
import { TrendingDown, TrendingUp, type LucideIcon } from 'lucide-react'

import { cn } from '@/shared/lib/utils'

const statCardVariants = cva('relative flex flex-col gap-4 rounded-2xl border p-5', {
  variants: {
    variant: {
      default:
        'border-white/[0.06] bg-gradient-to-br from-[#1E2128] via-[#232730] to-[#1A1D23] shadow-[0_0_24px_-8px_rgba(91,140,255,0.15)]',
      accent:
        'border-white/[0.06] bg-gradient-to-br from-[#1E2128] via-[#232730] to-[#1A1D23] shadow-[0_0_28px_-6px_rgba(91,140,255,0.45)]',
      warning:
        'border-white/[0.06] bg-gradient-to-br from-[#1E2128] via-[#232730] to-[#1A1D23] shadow-[0_0_28px_-6px_rgba(245,158,11,0.35)]',
      hero: 'border-transparent bg-gradient-to-br from-[#5B8CFF] to-[#8B7EFF] shadow-[0_8px_32px_-8px_rgba(91,140,255,0.4)]',
    },
  },
  defaultVariants: {
    variant: 'default',
  },
})

const iconChipVariants = cva('flex size-10 shrink-0 items-center justify-center rounded-full', {
  variants: {
    variant: {
      default: 'bg-primary/10 text-primary',
      accent: 'bg-primary/10 text-primary',
      warning: 'bg-amber-400/10 text-amber-400',
      hero: 'bg-white/15 text-white',
    },
  },
  defaultVariants: {
    variant: 'default',
  },
})

export interface StatCardTrend {
  /** Percentage change. Positive renders green with an up arrow, negative renders red with a down arrow. */
  value: number
}

export interface StatCardProps
  extends Omit<React.ComponentProps<'div'>, 'children'>,
    VariantProps<typeof statCardVariants> {
  label: string
  value: string | number
  icon: LucideIcon
  trend?: StatCardTrend
}

function StatCard({
  label,
  value,
  icon: Icon,
  trend,
  variant = 'default',
  className,
  ...props
}: StatCardProps) {
  const isPositive = trend !== undefined && trend.value >= 0
  const isHero = variant === 'hero'

  return (
    <div
      data-slot="stat-card"
      className={cn(statCardVariants({ variant }), className)}
      {...props}
    >
      <div className="flex items-center gap-3">
        <div className={cn(iconChipVariants({ variant }))}>
          <Icon className="size-5" />
        </div>
        <p
          className={cn(
            'font-mono text-xs tracking-wide uppercase',
            isHero ? 'text-white' : 'text-[#868C97]',
          )}
        >
          {label}
        </p>
      </div>

      <div className="flex items-end justify-between gap-2">
        <p className={cn('text-3xl font-bold', isHero ? 'text-white' : 'text-[#E6E8EC]')}>
          {value}
        </p>
        {trend && (
          <div
            className={cn(
              'flex items-center gap-1 text-sm font-medium',
              isHero && 'rounded-full bg-white/15 px-2 py-0.5',
              isPositive ? 'text-[#4ADE80]' : 'text-[#F87171]',
            )}
          >
            {isPositive ? <TrendingUp className="size-4" /> : <TrendingDown className="size-4" />}
            <span className="font-numeric">
              {isPositive ? '+' : ''}
              {trend.value}%
            </span>
          </div>
        )}
      </div>
    </div>
  )
}

export { StatCard, statCardVariants }
