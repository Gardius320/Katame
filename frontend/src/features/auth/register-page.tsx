import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation } from '@tanstack/react-query'
import { Link, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { colombianPhoneRegex, isValidColombianCedula } from '@/shared/lib/colombian-document-id'
import { register } from './api'
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

const registerSchema = z
  .object({
    firstName: z.string().min(1, es.register.validation.firstNameRequired),
    lastName: z.string().min(1, es.register.validation.lastNameRequired),
    documentId: z
      .string()
      .min(1, es.register.validation.documentIdRequired)
      .refine(isValidColombianCedula, es.register.validation.documentIdInvalid),
    email: z
      .string()
      .min(1, es.register.validation.emailRequired)
      .email(es.register.validation.emailInvalid),
    phoneNumber: z
      .string()
      .min(1, es.register.validation.phoneNumberRequired)
      .regex(colombianPhoneRegex, es.register.validation.phoneNumberInvalid),
    password: z
      .string()
      .min(1, es.register.validation.passwordRequired)
      .min(8, es.register.validation.passwordMinLength),
    confirmPassword: z.string().min(1, es.register.validation.confirmPasswordRequired),
  })
  .refine((values) => values.password === values.confirmPassword, {
    message: es.register.validation.passwordsDontMatch,
    path: ['confirmPassword'],
  })

type RegisterForm = z.infer<typeof registerSchema>

export function RegisterPage() {
  const navigate = useNavigate()
  const setSession = useSessionStore((state) => state.setSession)

  const form = useForm<RegisterForm>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      documentId: '',
      email: '',
      phoneNumber: '',
      password: '',
      confirmPassword: '',
    },
  })

  const mutation = useMutation({
    mutationFn: register,
    onSuccess: (data) => {
      setSession(data)
      toast.success(es.register.registerSuccess())
      navigate('/today', { replace: true })
    },
  })

  const onSubmit = form.handleSubmit(({ confirmPassword: _confirmPassword, ...values }) =>
    mutation.mutate(values),
  )

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
          {es.register.subtitle}
        </p>
      </div>

      <Card className="w-full max-w-sm border-border bg-card">
        <CardHeader>
          <CardTitle className="font-heading text-lg">{es.register.title}</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="grid gap-4" noValidate>
              <FormField
                control={form.control}
                name="firstName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.firstNameLabel}</FormLabel>
                    <FormControl>
                      <Input
                        placeholder={es.register.firstNamePlaceholder}
                        autoComplete="given-name"
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
                name="lastName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.lastNameLabel}</FormLabel>
                    <FormControl>
                      <Input
                        placeholder={es.register.lastNamePlaceholder}
                        autoComplete="family-name"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="documentId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.documentIdLabel}</FormLabel>
                    <FormControl>
                      <Input placeholder={es.register.documentIdPlaceholder} {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.emailLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="email"
                        placeholder={es.register.emailPlaceholder}
                        autoComplete="email"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="phoneNumber"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.phoneNumberLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="tel"
                        placeholder={es.register.phoneNumberPlaceholder}
                        autoComplete="tel"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.register.passwordLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="password"
                        autoComplete="new-password"
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
                    <FormLabel>{es.register.confirmPasswordLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="password"
                        autoComplete="new-password"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <Button type="submit" className="mt-2" disabled={mutation.isPending}>
                {mutation.isPending ? es.register.registering : es.register.registerButton}
              </Button>
            </form>
          </Form>

          <p className="mt-4 text-center text-sm text-muted-foreground">
            {es.register.hasAccount}{' '}
            <Link to="/login" className="font-medium text-primary hover:underline">
              {es.register.loginLink}
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  )
}
