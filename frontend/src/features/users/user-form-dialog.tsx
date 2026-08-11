import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { ecuadorianPhoneRegex, isValidEcuadorianCedula } from '@/shared/lib/ecuadorian-document-id'
import { useCreateUser, useUpdateUser } from './hooks'
import type { User } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Checkbox } from '@/shared/components/ui/checkbox'
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

function buildUserFormSchema(isEditing: boolean) {
  return z.object({
    username: z
      .string()
      .min(3, es.users.validation.usernameMinLength)
      .max(50, es.users.validation.usernameMaxLength),
    firstName: z.string().min(1, es.users.validation.firstNameRequired),
    lastName: z.string().min(1, es.users.validation.lastNameRequired),
    documentId: z
      .string()
      .min(1, es.users.validation.documentIdRequired)
      .refine(isValidEcuadorianCedula, es.users.validation.documentIdInvalid),
    phoneNumber: z
      .string()
      .min(1, es.users.validation.phoneNumberRequired)
      .regex(ecuadorianPhoneRegex, es.users.validation.phoneNumberInvalid),
    email: z
      .string()
      .min(1, es.users.validation.emailRequired)
      .email(es.users.validation.emailInvalid),
    password: isEditing
      ? z
          .string()
          .refine((value) => value.length === 0 || value.length >= 8, {
            message: es.users.validation.passwordMinLength,
          })
      : z
          .string()
          .min(1, es.users.validation.passwordRequired)
          .min(8, es.users.validation.passwordMinLength),
    isAdmin: z.boolean(),
  })
}

type UserFormSchema = z.infer<ReturnType<typeof buildUserFormSchema>>

interface UserFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  user?: User | null
}

export function UserFormDialog({ open, onOpenChange, user }: UserFormDialogProps) {
  const isEditing = Boolean(user)
  const createUser = useCreateUser()
  const updateUser = useUpdateUser()
  const mutation = isEditing ? updateUser : createUser

  const form = useForm<UserFormSchema>({
    resolver: zodResolver(buildUserFormSchema(isEditing)),
    defaultValues: {
      username: '',
      firstName: '',
      lastName: '',
      documentId: '',
      phoneNumber: '',
      email: '',
      password: '',
      isAdmin: false,
    },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        username: user?.username ?? '',
        firstName: user?.firstName ?? '',
        lastName: user?.lastName ?? '',
        documentId: user?.documentId ?? '',
        phoneNumber: user?.phoneNumber ?? '',
        email: user?.email ?? '',
        password: '',
        isAdmin: user?.isAdmin ?? false,
      })
    }
  }, [open, user, form])

  const onSubmit = form.handleSubmit((values) => {
    const onSuccess = () => onOpenChange(false)

    if (isEditing && user) {
      updateUser.mutate(
        {
          id: user.id,
          payload: {
            username: values.username,
            firstName: values.firstName,
            lastName: values.lastName,
            documentId: values.documentId,
            phoneNumber: values.phoneNumber,
            email: values.email,
            password: values.password.length > 0 ? values.password : null,
            isAdmin: values.isAdmin,
          },
        },
        { onSuccess },
      )
    } else {
      createUser.mutate(
        {
          username: values.username,
          firstName: values.firstName,
          lastName: values.lastName,
          documentId: values.documentId,
          phoneNumber: values.phoneNumber,
          email: values.email,
          password: values.password,
          isAdmin: values.isAdmin,
        },
        { onSuccess },
      )
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.users.editUser : es.users.newUser}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="username"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.users.fields.username}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.users.fields.usernamePlaceholder} autoFocus {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="firstName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.users.fields.firstName}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.users.fields.firstNamePlaceholder} {...field} />
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
                  <FormLabel>{es.users.fields.lastName}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.users.fields.lastNamePlaceholder} {...field} />
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
                  <FormLabel>{es.users.fields.documentId}</FormLabel>
                  <FormControl>
                    <Input placeholder={es.users.fields.documentIdPlaceholder} {...field} />
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
                  <FormLabel>{es.users.fields.phoneNumber}</FormLabel>
                  <FormControl>
                    <Input
                      type="tel"
                      placeholder={es.users.fields.phoneNumberPlaceholder}
                      {...field}
                    />
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
                  <FormLabel>{es.users.fields.email}</FormLabel>
                  <FormControl>
                    <Input
                      type="email"
                      placeholder={es.users.fields.emailPlaceholder}
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
                  <FormLabel>{es.users.fields.password}</FormLabel>
                  <FormControl>
                    <Input
                      type="password"
                      autoComplete="new-password"
                      placeholder={
                        isEditing
                          ? es.users.fields.passwordPlaceholderEdit
                          : es.users.fields.passwordPlaceholderCreate
                      }
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="isAdmin"
              render={({ field }) => (
                <FormItem className="flex flex-row items-center gap-2">
                  <FormControl>
                    <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                  </FormControl>
                  <FormLabel className="font-normal">{es.users.fields.isAdmin}</FormLabel>
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
