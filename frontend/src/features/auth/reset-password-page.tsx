import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation } from '@tanstack/react-query'
import { Link, useSearchParams } from 'react-router-dom'
import { es } from '@/shared/i18n/es'
import { resetPassword } from './api'
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

const resetPasswordSchema = z
  .object({
    password: z
      .string()
      .min(1, es.resetPassword.validation.passwordRequired)
      .min(8, es.resetPassword.validation.passwordMinLength),
    confirmPassword: z.string().min(1, es.resetPassword.validation.confirmPasswordRequired),
  })
  .refine((values) => values.password === values.confirmPassword, {
    message: es.resetPassword.validation.passwordsDontMatch,
    path: ['confirmPassword'],
  })

type ResetPasswordForm = z.infer<typeof resetPasswordSchema>

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get('token')

  const form = useForm<ResetPasswordForm>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: { password: '', confirmPassword: '' },
  })

  const mutation = useMutation({
    mutationFn: (values: ResetPasswordForm) =>
      resetPassword({ token: token ?? '', newPassword: values.password }),
  })

  const onSubmit = form.handleSubmit((values) => mutation.mutate(values))

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-8 bg-background p-4">
      <div className="text-center">
        <h1
          className="font-brush text-primary uppercase leading-none"
          style={{ fontSize: 'clamp(3rem, 15vw, 6.5rem)' }}
        >
          {es.app.name}
        </h1>
        <p className="mt-3 font-mono text-sm tracking-wide text-muted-foreground">
          {es.resetPassword.subtitle}
        </p>
      </div>

      <Card className="w-full max-w-sm border-border bg-card">
        <CardHeader>
          <CardTitle className="font-heading text-lg">{es.resetPassword.title}</CardTitle>
        </CardHeader>
        <CardContent>
          {!token || mutation.isError ? (
            <div className="grid gap-4 text-center">
              <p className="text-sm text-muted-foreground">{es.resetPassword.invalidLinkMessage}</p>
              <Link to="/forgot-password">
                <Button className="w-full">{es.resetPassword.requestNewLink}</Button>
              </Link>
            </div>
          ) : mutation.isSuccess ? (
            <div className="grid gap-4 text-center">
              <p className="text-sm text-muted-foreground">{es.resetPassword.successMessage}</p>
              <Link to="/login">
                <Button className="w-full">{es.auth.loginButton}</Button>
              </Link>
            </div>
          ) : (
            <Form {...form}>
              <form onSubmit={onSubmit} className="grid gap-4" noValidate>
                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>{es.resetPassword.passwordLabel}</FormLabel>
                      <FormControl>
                        <Input
                          type="password"
                          autoComplete="new-password"
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
                  name="confirmPassword"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>{es.resetPassword.confirmPasswordLabel}</FormLabel>
                      <FormControl>
                        <Input type="password" autoComplete="new-password" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button type="submit" className="mt-2" disabled={mutation.isPending}>
                  {mutation.isPending ? es.resetPassword.submitting : es.resetPassword.submitButton}
                </Button>
              </form>
            </Form>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
