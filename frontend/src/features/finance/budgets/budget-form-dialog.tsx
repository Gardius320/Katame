import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateBudget, useUpdateBudget } from './hooks'
import type { Budget, BudgetPeriod } from './types'
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
  FormDescription,
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

const periodOptions: BudgetPeriod[] = ['weekly', 'biweekly', 'monthly']

const budgetFormSchema = z.object({
  category: z
    .string()
    .min(1, es.finance.budgets.validation.categoryRequired)
    .max(50, es.finance.budgets.validation.categoryMaxLength),
  amount: z.number().positive(es.finance.budgets.validation.amountRequired),
  period: z.enum(['weekly', 'biweekly', 'monthly']),
  anchorDate: z.string().min(1, es.finance.budgets.validation.anchorDateRequired),
})

type BudgetFormSchema = z.infer<typeof budgetFormSchema>

function toDateInputValue(date: string): string {
  return date.slice(0, 10)
}

interface BudgetFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  budget?: Budget | null
}

export function BudgetFormDialog({ open, onOpenChange, budget }: BudgetFormDialogProps) {
  const isEditing = Boolean(budget)
  const createBudget = useCreateBudget()
  const updateBudget = useUpdateBudget()
  const mutation = isEditing ? updateBudget : createBudget

  const form = useForm<BudgetFormSchema>({
    resolver: zodResolver(budgetFormSchema),
    defaultValues: {
      category: '',
      amount: 0,
      period: 'monthly',
      anchorDate: toDateInputValue(new Date().toISOString()),
    },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        category: budget?.category ?? '',
        amount: budget?.amount ?? 0,
        period: budget?.period ?? 'monthly',
        anchorDate: budget
          ? toDateInputValue(budget.anchorDate)
          : toDateInputValue(new Date().toISOString()),
      })
    }
  }, [open, budget, form])

  const onSubmit = form.handleSubmit((values) => {
    const payload = {
      category: values.category,
      amount: values.amount,
      period: values.period,
      anchorDate: new Date(values.anchorDate).toISOString(),
    }

    const onSuccess = () => onOpenChange(false)

    if (isEditing && budget) {
      updateBudget.mutate({ id: budget.id, payload }, { onSuccess })
    } else {
      createBudget.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.finance.budgets.editBudget : es.finance.budgets.newBudget}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="category"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.budgets.fields.category}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.budgets.fields.categoryPlaceholder}
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
              name="amount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.budgets.fields.amount}</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step="0.01"
                      className="font-numeric"
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

            <FormField
              control={form.control}
              name="period"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.budgets.fields.period}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {periodOptions.map((period) => (
                        <SelectItem key={period} value={period}>
                          {es.finance.budgets.period[period]}
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
              name="anchorDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.budgets.fields.anchorDate}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
                  </FormControl>
                  <FormDescription>{es.finance.budgets.fields.anchorDateHint}</FormDescription>
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
