import { useState } from 'react'
import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { useDeleteUser, useUsers } from './hooks'
import { UserFormDialog } from './user-form-dialog'
import type { User } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Skeleton } from '@/shared/components/ui/skeleton'
import { Tooltip, TooltipContent, TooltipTrigger } from '@/shared/components/ui/tooltip'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog'

function formatCreatedAt(date: string): string {
  return format(new Date(date), "d 'de' MMMM 'de' yyyy", { locale: dateFnsEs })
}

export default function UsersPage() {
  const { data: users, isLoading } = useUsers()
  const deleteUser = useDeleteUser()
  const currentUsername = useSessionStore((state) => state.username)

  const [formOpen, setFormOpen] = useState(false)
  const [editingUser, setEditingUser] = useState<User | null>(null)
  const [userToDelete, setUserToDelete] = useState<User | null>(null)

  const adminCount = (users ?? []).filter((user) => user.isAdmin).length

  const openCreateForm = () => {
    setEditingUser(null)
    setFormOpen(true)
  }

  const openEditForm = (user: User) => {
    setEditingUser(user)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!userToDelete) return
    deleteUser.mutate(userToDelete.id, { onSuccess: () => setUserToDelete(null) })
  }

  const getDeleteDisabledReason = (user: User): string | null => {
    if (user.username === currentUsername) return es.users.deleteDisabledSelf
    if (user.isAdmin && adminCount <= 1) return es.users.deleteDisabledLastAdmin
    return null
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.users.title}</h1>
          <p className="text-muted-foreground">{es.users.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.users.newUser}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-2">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-16 w-full rounded-xl" />
          ))}
        </div>
      ) : users && users.length > 0 ? (
        <div className="grid gap-2">
          {users.map((user) => {
            const deleteDisabledReason = getDeleteDisabledReason(user)

            return (
              <Card
                key={user.id}
                className="flex-row items-center justify-between gap-4 px-4 py-4"
              >
                <div className="flex min-w-0 items-center gap-3">
                  <Badge
                    className={
                      user.isAdmin
                        ? 'bg-primary/15 text-primary'
                        : 'bg-secondary text-secondary-foreground'
                    }
                  >
                    {user.isAdmin ? es.users.admin : es.users.regular}
                  </Badge>
                  <div className="min-w-0">
                    <p className="truncate font-medium">
                      {user.firstName} {user.lastName}{' '}
                      <span className="font-normal text-muted-foreground">({user.username})</span>
                    </p>
                    <p className="truncate text-xs text-muted-foreground">
                      {user.email} · {es.users.createdAt} {formatCreatedAt(user.createdAt)}
                    </p>
                  </div>
                </div>
                <div className="flex shrink-0 items-center gap-1">
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.edit}
                    title={es.common.edit}
                    onClick={() => openEditForm(user)}
                  >
                    <Pencil className="size-4" />
                  </Button>
                  {deleteDisabledReason ? (
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span tabIndex={0}>
                          <Button
                            variant="ghost"
                            size="icon-sm"
                            aria-label={es.common.delete}
                            disabled
                          >
                            <Trash2 className="size-4" />
                          </Button>
                        </span>
                      </TooltipTrigger>
                      <TooltipContent>{deleteDisabledReason}</TooltipContent>
                    </Tooltip>
                  ) : (
                    <Button
                      variant="ghost"
                      size="icon-sm"
                      aria-label={es.common.delete}
                      title={es.common.delete}
                      onClick={() => setUserToDelete(user)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  )}
                </div>
              </Card>
            )
          })}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">{es.users.emptyState}</Card>
      )}

      <UserFormDialog open={formOpen} onOpenChange={setFormOpen} user={editingUser} />

      <AlertDialog open={userToDelete !== null} onOpenChange={(open) => !open && setUserToDelete(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteUser.isPending}>
              {deleteUser.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
