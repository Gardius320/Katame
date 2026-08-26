import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Pencil } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { useFinancialProfile, useUpdateFinancialProfile } from './hooks'
import type { SavingsGoal } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
import { Input } from '@/shared/components/ui/input'
import { Skeleton } from '@/shared/components/ui/skeleton'
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

const incomeFormSchema = z.object({
  monthlyIncome: z.number().min(0, es.finance.savings.validation.monthlyIncomeInvalid),
})

type IncomeFormSchema = z.infer<typeof incomeFormSchema>

interface IncomeSummaryCardProps {
  goals: SavingsGoal[]
}

// Tarjeta arriba de la lista de metas: muestra (y deja editar) el ingreso mensual
// del usuario, y resume cuánto planea ahorrar al mes en total entre todas sus
// metas -- y qué porcentaje de su ingreso representa eso.
export function IncomeSummaryCard({ goals }: IncomeSummaryCardProps) {
  const { data: profile, isLoading } = useFinancialProfile()
  const updateProfile = useUpdateFinancialProfile()
  const [editOpen, setEditOpen] = useState(false)

  const form = useForm<IncomeFormSchema>({
    resolver: zodResolver(incomeFormSchema),
    defaultValues: { monthlyIncome: 0 },
  })

  useEffect(() => {
    if (editOpen) {
      form.reset({ monthlyIncome: profile?.monthlyIncome ?? 0 })
    }
  }, [editOpen, profile, form])

  const onSubmit = form.handleSubmit((values) => {
    updateProfile.mutate(
      { monthlyIncome: values.monthlyIncome },
      { onSuccess: () => setEditOpen(false) },
    )
  })

  if (isLoading) {
    return <Skeleton className="h-24 w-full rounded-xl" />
  }

  const monthlyIncome = profile?.monthlyIncome ?? 0
  const totalMonthlySavings = goals.reduce(
    (sum, goal) => sum + (goal.monthlyContributionTarget ?? 0),
    0,
  )
  const percent =
    monthlyIncome > 0 ? Math.round((totalMonthlySavings / monthlyIncome) * 100) : null

  return (
    <>
      <Card className="gap-2 p-4">
        <div className="flex items-center justify-between gap-2">
          <div>
            <p className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
              {es.finance.savings.incomeCard.incomeLabel}
            </p>
            <p className="font-numeric text-xl font-semibold">
              {monthlyIncome > 0
                ? formatCurrency(monthlyIncome)
                : es.finance.savings.incomeCard.noIncomeSet}
            </p>
          </div>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={es.finance.savings.incomeCard.editIncome}
            title={es.finance.savings.incomeCard.editIncome}
            onClick={() => setEditOpen(true)}
          >
            <Pencil className="size-4" />
          </Button>
        </div>

        {monthlyIncome > 0 && totalMonthlySavings > 0 ? (
          <p className="font-numeric text-sm text-muted-foreground">
            {es.finance.savings.incomeCard.totalSavingsLabel
              .replace('{amount}', formatCurrency(totalMonthlySavings))
              .replace('{percent}', String(percent))}
          </p>
        ) : monthlyIncome === 0 ? (
          <p className="text-sm text-muted-foreground">
            {es.finance.savings.incomeCard.noIncomeHint}
          </p>
        ) : null}
      </Card>

      <Dialog open={editOpen} onOpenChange={setEditOpen}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle className="font-heading">
              {es.finance.savings.incomeCard.editIncome}
            </DialogTitle>
          </DialogHeader>

          <Form {...form}>
            <form onSubmit={onSubmit} className="grid gap-4" noValidate>
              <FormField
                control={form.control}
                name="monthlyIncome"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.savings.incomeCard.incomeLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        step="0.01"
                        className="font-numeric"
                        autoFocus
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

              <DialogFooter className="mt-2">
                <Button
                  type="submit"
                  disabled={updateProfile.isPending}
                  className="w-full sm:w-auto"
                >
                  {updateProfile.isPending ? es.common.saving : es.common.save}
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </DialogContent>
      </Dialog>
    </>
  )
}
