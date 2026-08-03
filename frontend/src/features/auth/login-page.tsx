import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { login } from './api'
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

const loginSchema = z.object({
  username: z.string().min(1, es.auth.validation.usernameRequired),
  password: z.string().min(1, es.auth.validation.passwordRequired),
})

type LoginForm = z.infer<typeof loginSchema>

export function LoginPage() {
  const navigate = useNavigate()
  const setSession = useSessionStore((state) => state.setSession)

  const form = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
    defaultValues: { username: '', password: '' },
  })

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      setSession(data)
      toast.success(es.auth.loginSuccess)
      navigate('/today', { replace: true })
    },
  })

  const onSubmit = form.handleSubmit((values) => mutation.mutate(values))

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-8 bg-background p-4">
      <div className="text-center">
        <h1
          className="font-heading font-bold text-primary uppercase leading-none"
          style={{ fontSize: 'clamp(3rem, 15vw, 6.5rem)' }}
        >
          {es.app.name}
        </h1>
        <p className="mt-3 font-mono text-sm tracking-wide text-muted-foreground">
          {es.auth.tagline}
        </p>
      </div>

      <Card className="w-full max-w-sm border-border bg-card">
        <CardHeader>
          <CardTitle className="font-heading text-lg">{es.auth.loginTitle}</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="grid gap-4" noValidate>
              <FormField
                control={form.control}
                name="username"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.auth.usernameLabel}</FormLabel>
                    <FormControl>
                      <Input
                        placeholder={es.auth.usernamePlaceholder}
                        autoComplete="username"
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
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.auth.passwordLabel}</FormLabel>
                    <FormControl>
                      <Input
                        type="password"
                        placeholder={es.auth.passwordPlaceholder}
                        autoComplete="current-password"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <Button type="submit" className="mt-2" disabled={mutation.isPending}>
                {mutation.isPending ? es.auth.loggingIn : es.auth.loginButton}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
