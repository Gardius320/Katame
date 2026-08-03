/**
 * Todo el texto visible para el usuario vive acá. Los componentes trabajan
 * con claves en inglés (p. ej. status "pending") y traducen al mostrar.
 */
export const es = {
  app: {
    name: 'Katame',
  },

  nav: {
    today: 'Hoy',
    finance: 'Finanzas',
    training: 'Entrenamiento',
    tasks: 'Tareas',
    goals: 'Metas',
    projects: 'Proyectos',
    subscriptions: 'Suscripciones',
  },

  common: {
    loading: 'Cargando...',
    save: 'Guardar',
    saving: 'Guardando...',
    cancel: 'Cancelar',
    delete: 'Eliminar',
    deleting: 'Eliminando...',
    edit: 'Editar',
    create: 'Crear',
    confirm: 'Confirmar',
    confirmDeleteTitle: '¿Eliminar este elemento?',
    confirmDeleteDescription: 'Esta acción no se puede deshacer.',
    retry: 'Reintentar',
    close: 'Cerrar',
    logout: 'Cerrar sesión',
    noResults: 'No hay nada para mostrar todavía.',
  },

  theme: {
    toggleToLight: 'Cambiar a modo claro',
    toggleToDark: 'Cambiar a modo oscuro',
  },

  comingSoon: {
    title: 'Próximamente',
    description: 'Este módulo todavía no está disponible. Estamos trabajando en él.',
  },

  errors: {
    generic: 'Ocurrió un error inesperado. Intenta de nuevo más tarde.',
    network: 'No pudimos conectarnos con el servidor. Revisa tu conexión.',
    sessionExpired: 'Tu sesión expiró. Inicia sesión de nuevo.',
  },

  auth: {
    tagline: 'Panel personal',
    loginTitle: 'Iniciar sesión',
    usernameLabel: 'Usuario',
    usernamePlaceholder: 'tu_usuario',
    passwordLabel: 'Contraseña',
    passwordPlaceholder: '••••••••',
    loginButton: 'Entrar',
    loggingIn: 'Ingresando...',
    loginSuccess: 'Bienvenido de nuevo',
    validation: {
      usernameRequired: 'El nombre de usuario es obligatorio.',
      passwordRequired: 'La contraseña es obligatoria.',
    },
  },

  tasks: {
    title: 'Tareas',
    subtitle: 'Organiza tus pendientes del día a día',
    newTask: 'Nueva tarea',
    editTask: 'Editar tarea',
    emptyState: 'No tienes tareas todavía. Crea la primera.',
    fields: {
      title: 'Título',
      titlePlaceholder: 'Ej. Pagar la tarjeta de crédito',
      status: 'Estado',
      date: 'Fecha',
      datePlaceholder: 'Sin fecha',
    },
    status: {
      pending: 'Pendiente',
      in_progress: 'En progreso',
      done: 'Hecho',
    },
    validation: {
      titleRequired: 'El título es obligatorio.',
      titleMaxLength: 'El título no puede superar los 150 caracteres.',
      statusInvalid: 'Selecciona un estado válido.',
    },
    toasts: {
      created: 'Tarea creada.',
      updated: 'Tarea actualizada.',
      deleted: 'Tarea eliminada.',
      completed: '¡Tarea completada!',
    },
  },

  training: {
    title: 'Entrenamiento',
    subtitle: 'Organiza tu rutina semanal',
    newDay: 'Nuevo día',
    editDay: 'Editar día',
    emptyState: 'No tienes días de entrenamiento todavía. Crea el primero.',
    confirmDeleteDayDescription:
      'Se eliminará el día y todos sus ejercicios. Esta acción no se puede deshacer.',
    fields: {
      dayTitle: 'Título',
      dayTitlePlaceholder: 'Ej. Empuje',
      dayOfWeek: 'Día de la semana',
    },
    days: {
      Monday: 'Lunes',
      Tuesday: 'Martes',
      Wednesday: 'Miércoles',
      Thursday: 'Jueves',
      Friday: 'Viernes',
      Saturday: 'Sábado',
      Sunday: 'Domingo',
    },
    validation: {
      dayTitleRequired: 'El título es obligatorio.',
      dayTitleMaxLength: 'El título no puede superar los 100 caracteres.',
    },
    toasts: {
      dayCreated: 'Día de entrenamiento creado.',
      dayUpdated: 'Día actualizado.',
      dayDeleted: 'Día eliminado.',
    },
    exercises: {
      emptyState: 'Todavía no agregaste ejercicios a este día.',
      newExercise: 'Agregar ejercicio',
      editExercise: 'Editar ejercicio',
      name: 'Ejercicio',
      namePlaceholder: 'Ej. Press banca',
      setsReps: 'Series x repeticiones',
      setsRepsPlaceholder: 'Ej. 4x8',
      validation: {
        nameRequired: 'El nombre del ejercicio es obligatorio.',
        nameMaxLength: 'El nombre no puede superar los 100 caracteres.',
        setsRepsRequired: 'Las series y repeticiones son obligatorias.',
        setsRepsMaxLength: 'Las series y repeticiones no pueden superar los 50 caracteres.',
      },
      toasts: {
        created: 'Ejercicio agregado.',
        updated: 'Ejercicio actualizado.',
        deleted: 'Ejercicio eliminado.',
      },
    },
  },
} as const

export type Es = typeof es
