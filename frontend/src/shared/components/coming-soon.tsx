import { es } from '@/shared/i18n/es'

interface ComingSoonProps {
  title: string
}

export function ComingSoon({ title }: ComingSoonProps) {
  return (
    <div className="rounded-xl border border-border bg-card p-10 text-center">
      <h1 className="font-heading text-2xl font-semibold">{title}</h1>
      <p className="mt-2 text-muted-foreground">{es.comingSoon.description}</p>
    </div>
  )
}
