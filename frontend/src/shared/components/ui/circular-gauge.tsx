import * as React from 'react'
import { cva, type VariantProps } from 'class-variance-authority'

import { cn } from '@/shared/lib/utils'

const gaugeArcVariants = cva('transition-[stroke-dashoffset] duration-[800ms] ease-out', {
  variants: {
    variant: {
      default: 'stroke-[#5B8CFF]',
      success: 'stroke-[#4ADE80]',
      warning: 'stroke-[#FBBF24]',
      danger: 'stroke-[#F87171]',
    },
  },
  defaultVariants: {
    variant: 'default',
  },
})

const SIZE_CONFIG = {
  sm: { diameter: 80, strokeWidth: 6 },
  lg: { diameter: 140, strokeWidth: 8 },
} as const

export interface CircularGaugeProps
  extends Omit<React.ComponentProps<'div'>, 'children'>,
    VariantProps<typeof gaugeArcVariants> {
  value: number
  max: number
  label: string
  size?: keyof typeof SIZE_CONFIG
}

function CircularGauge({
  value,
  max,
  label,
  size = 'lg',
  variant = 'default',
  className,
  ...props
}: CircularGaugeProps) {
  const percent = max > 0 ? Math.min(100, Math.max(0, (value / max) * 100)) : 0
  const { diameter, strokeWidth } = SIZE_CONFIG[size]
  const radius = (diameter - strokeWidth) / 2
  const circumference = 2 * Math.PI * radius
  const targetOffset = circumference * (1 - percent / 100)

  const [offset, setOffset] = React.useState(circumference)

  React.useEffect(() => {
    const timer = setTimeout(() => setOffset(targetOffset), 10)
    return () => clearTimeout(timer)
  }, [targetOffset])

  return (
    <div
      data-slot="circular-gauge"
      className={cn('relative inline-flex shrink-0 items-center justify-center', className)}
      style={{ width: diameter, height: diameter }}
      {...props}
    >
      <svg
        width={diameter}
        height={diameter}
        viewBox={`0 0 ${diameter} ${diameter}`}
        style={{ transform: 'rotate(-90deg)', transformOrigin: '50% 50%' }}
      >
        <circle
          cx={diameter / 2}
          cy={diameter / 2}
          r={radius}
          fill="none"
          stroke="rgba(255,255,255,0.06)"
          strokeWidth={strokeWidth}
        />
        <circle
          cx={diameter / 2}
          cy={diameter / 2}
          r={radius}
          fill="none"
          strokeWidth={strokeWidth}
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={offset}
          className={cn(gaugeArcVariants({ variant }))}
        />
      </svg>
      <div className="absolute inset-0 flex flex-col items-center justify-center gap-0.5">
        <p className="text-2xl font-bold text-[#E6E8EC]">{Math.round(percent)}%</p>
        <p className="font-mono text-xs tracking-wide text-[#868C97] uppercase">{label}</p>
      </div>
    </div>
  )
}

export { CircularGauge, gaugeArcVariants }
