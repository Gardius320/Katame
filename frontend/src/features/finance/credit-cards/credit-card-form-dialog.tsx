import { useEffect, useRef, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { ImageUp, X } from 'lucide-react'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { useCreateCreditCard, useUpdateCreditCard } from './hooks'
import type { CreditCard } from './types'
import { BankCombobox } from './bank-combobox'
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
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/components/ui/form'

// Los logos se redimensionan en el navegador antes de guardarse como base64,
// así que no hace falta subir archivos a ningún storage externo: 200x200 es
// de sobra para un logo de banco y mantiene el payload chico (el backend
// también valida un tamaño máximo por las dudas).
const MAX_LOGO_DIMENSION = 200

function resizeImageToDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onerror = () => reject(reader.error ?? new Error('No se pudo leer el archivo.'))
    reader.onload = () => {
      const img = new Image()
      img.onerror = () => reject(new Error('El archivo no es una imagen válida.'))
      img.onload = () => {
        const scale = Math.min(1, MAX_LOGO_DIMENSION / Math.max(img.width, img.height))
        const width = Math.max(1, Math.round(img.width * scale))
        const height = Math.max(1, Math.round(img.height * scale))
        const canvas = document.createElement('canvas')
        canvas.width = width
        canvas.height = height
        const ctx = canvas.getContext('2d')
        if (!ctx) {
          reject(new Error('No se pudo procesar la imagen.'))
          return
        }
        ctx.drawImage(img, 0, 0, width, height)
        resolve(canvas.toDataURL('image/png'))
      }
      img.src = reader.result as string
    }
    reader.readAsDataURL(file)
  })
}

const creditCardFormSchema = z.object({
  name: z
    .string()
    .min(1, es.finance.creditCards.validation.nameRequired)
    .max(100, es.finance.creditCards.validation.nameMaxLength),
  bank: z.string().nullable(),
  statementDay: z
    .number()
    .int()
    .min(1, es.finance.creditCards.validation.statementDayRange)
    .max(31, es.finance.creditCards.validation.statementDayRange),
  paymentDay: z
    .number()
    .int()
    .min(1, es.finance.creditCards.validation.paymentDayRange)
    .max(31, es.finance.creditCards.validation.paymentDayRange),
  creditLimit: z.number().positive(es.finance.creditCards.validation.creditLimitRequired),
  logoDataUrl: z.string().nullable(),
})

type CreditCardFormSchema = z.infer<typeof creditCardFormSchema>

interface CreditCardFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  card?: CreditCard | null
}

export function CreditCardFormDialog({ open, onOpenChange, card }: CreditCardFormDialogProps) {
  const isEditing = Boolean(card)
  const createCard = useCreateCreditCard()
  const updateCard = useUpdateCreditCard()
  const mutation = isEditing ? updateCard : createCard
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [isProcessingLogo, setIsProcessingLogo] = useState(false)

  const form = useForm<CreditCardFormSchema>({
    resolver: zodResolver(creditCardFormSchema),
    defaultValues: {
      name: '',
      bank: null,
      statementDay: 1,
      paymentDay: 1,
      creditLimit: 0,
      logoDataUrl: null,
    },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        name: card?.name ?? '',
        bank: card?.bank ?? null,
        statementDay: card?.statementDay ?? 1,
        paymentDay: card?.paymentDay ?? 1,
        creditLimit: card?.creditLimit ?? 0,
        logoDataUrl: card?.logoDataUrl ?? null,
      })
    }
  }, [open, card, form])

  const logoDataUrl = form.watch('logoDataUrl')

  const handleLogoChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    event.target.value = ''
    if (!file) return

    if (!file.type.startsWith('image/')) {
      toast.error(es.finance.creditCards.validation.logoInvalidType)
      return
    }

    setIsProcessingLogo(true)
    try {
      const dataUrl = await resizeImageToDataUrl(file)
      form.setValue('logoDataUrl', dataUrl, { shouldDirty: true })
    } catch {
      toast.error(es.finance.creditCards.validation.logoProcessingError)
    } finally {
      setIsProcessingLogo(false)
    }
  }

  const onSubmit = form.handleSubmit((values) => {
    const onSuccess = () => onOpenChange(false)

    if (isEditing && card) {
      updateCard.mutate({ id: card.id, payload: values }, { onSuccess })
    } else {
      createCard.mutate(values, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.finance.creditCards.editCard : es.finance.creditCards.newCard}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <div className="grid gap-2">
              <FormLabel>{es.finance.creditCards.fields.logo}</FormLabel>
              <div className="flex items-center gap-3">
                <div className="flex size-16 shrink-0 items-center justify-center overflow-hidden rounded-lg border border-border bg-muted">
                  {logoDataUrl ? (
                    <img src={logoDataUrl} alt="" className="size-full object-contain" />
                  ) : (
                    <ImageUp className="size-6 text-muted-foreground" />
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    className="hidden"
                    onChange={handleLogoChange}
                  />
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    disabled={isProcessingLogo}
                    onClick={() => fileInputRef.current?.click()}
                  >
                    {isProcessingLogo
                      ? es.finance.creditCards.fields.logoProcessing
                      : es.finance.creditCards.fields.logoUpload}
                  </Button>
                  {logoDataUrl && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      onClick={() => form.setValue('logoDataUrl', null, { shouldDirty: true })}
                    >
                      <X className="size-4" />
                      {es.finance.creditCards.fields.logoRemove}
                    </Button>
                  )}
                </div>
              </div>
            </div>

            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.creditCards.fields.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.creditCards.fields.namePlaceholder}
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
              name="bank"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.creditCards.fields.bank}</FormLabel>
                  <FormControl>
                    <BankCombobox value={field.value} onChange={field.onChange} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="statementDay"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.creditCards.fields.statementDay}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        max={31}
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
                name="paymentDay"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.creditCards.fields.paymentDay}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        max={31}
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
            </div>

            <FormField
              control={form.control}
              name="creditLimit"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.creditCards.fields.creditLimit}</FormLabel>
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

            <DialogFooter className="mt-2">
              <Button
                type="submit"
                disabled={mutation.isPending || isProcessingLogo}
                className="w-full sm:w-auto"
              >
                {mutation.isPending ? es.common.saving : es.common.save}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
