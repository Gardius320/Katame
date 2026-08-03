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
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from '@/shared/components/ui/card'
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
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <Card className="w-full max-w-sm border-border bg-card">
        <CardHeader>
          <CardTitle className="font-heading text-2xl">{es.auth.loginTitle}</CardTitle>
          <CardDescription>{es.auth.loginSubtitle}</CardDescription>
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
