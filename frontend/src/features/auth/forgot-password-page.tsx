import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { es } from '@/shared/i18n/es'
import { forgotPassword } from './api'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/components/ui/form'

const forgotPasswordSchema = z.object({
  email: z
    .string()
    .min(1, es.forgotPassword.validation.emailRequired)
    .email(es.forgotPassword.validation.emailInvalid),
})

type ForgotPasswordForm = z.infer<typeof forgotPasswordSchema>

export function ForgotPasswordPage() {
  const form = useForm<ForgotPasswordForm>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: { email: '' },
  })

  const mutation = useMutation({ mutationFn: forgotPassword })

  const onSubmit = form.handleSubmit((values) => mutation.mutate(values))

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-8 bg-background p-4">
      <div className="text-center">
        <img
          src="/icons/icon.svg"
          alt={es.app.name}
          className="mx-auto"
          style={{ width: 'clamp(6rem, 20vw, 11rem)' }}
        />
        <p className="mt-3 font-mono text-sm tracking-wide text-muted-foreground">
          {es.forgotPassword.subtitle}
        </p>
      </div>

      <Card className="w-full max-w-sm border-border bg-card">
        <CardHeader>
          <CardTitle className="font-heading text-lg">{es.forgotPassword.title}</CardTitle>
        </CardHeader>
        <CardContent>
          {mutation.isSuccess ? (
            <p className="text-sm text-muted-foreground">{es.forgotPassword.successMessage}</p>
          ) : (
            <Form {...form}>
              <form onSubmit={onSubmit} className="grid gap-4" noValidate>
                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>{es.forgotPassword.emailLabel}</FormLabel>
                      <FormControl>
                        <Input
                          type="email"
                          placeholder={es.forgotPassword.emailPlaceholder}
                          autoComplete="email"
                          autoFocus
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button type="submit" className="mt-2" disabled={mutation.isPending}>
                  {mutation.isPending ? es.forgotPassword.submitting : es.forgotPassword.submitButton}
                </Button>
              </form>
            </Form>
          )}

          <p className="mt-4 text-center text-sm text-muted-foreground">
            <Link to="/login" className="font-medium text-primary hover:underline">
              {es.forgotPassword.backToLogin}
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  )
}
