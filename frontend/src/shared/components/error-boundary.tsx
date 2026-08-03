import { Component, type ErrorInfo, type ReactNode } from 'react'
import { Button } from '@/shared/components/ui/button'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false }

  static getDerivedStateFromError(): State {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error('Error no controlado en la interfaz:', error, info)
  }

  handleReload = () => {
    this.setState({ hasError: false })
    window.location.reload()
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="flex min-h-screen flex-col items-center justify-center gap-4 bg-background p-6 text-center text-foreground">
          <h1 className="font-heading text-2xl font-semibold">Algo salió mal</h1>
          <p className="max-w-sm text-muted-foreground">
            Ocurrió un error inesperado al mostrar esta pantalla. Intenta recargar la página.
          </p>
          <Button onClick={this.handleReload}>Recargar</Button>
        </div>
      )
    }

    return this.props.children
  }
}
