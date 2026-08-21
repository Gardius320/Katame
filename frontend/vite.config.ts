/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { VitePWA } from 'vite-plugin-pwa'
import { fileURLToPath } from 'node:url'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['icons/icon.svg', 'icons/icon-maskable.svg', 'icons/apple-touch-icon.png'],
      manifest: {
        name: 'Katame',
        short_name: 'Katame',
        description:
          'Finanzas, entrenamiento, tareas, metas, proyectos y suscripciones en un solo lugar.',
        lang: 'es',
        theme_color: '#15171C',
        background_color: '#15171C',
        display: 'standalone',
        start_url: '/',
        icons: [
          { src: '/icons/icon-192.png', sizes: '192x192', type: 'image/png', purpose: 'any' },
          { src: '/icons/icon-512.png', sizes: '512x512', type: 'image/png', purpose: 'any' },
          {
            src: '/icons/icon-maskable-192.png',
            sizes: '192x192',
            type: 'image/png',
            purpose: 'maskable',
          },
          {
            src: '/icons/icon-maskable-512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable',
          },
        ],
      },
      devOptions: {
        enabled: false,
      },
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5173,
  },
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    // Fija VITE_API_BASE_URL para los tests: no depende de que exista un
    // .env local (que no está versionado). Sin esto, en CI la variable
    // queda undefined y el mock de MSW (registrado con la misma variable)
    // no coincide con la URL real que arma axios, y el test falla.
    env: {
      VITE_API_BASE_URL: 'http://localhost:5057/api',
    },
  },
})
