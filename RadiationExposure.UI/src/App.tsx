import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from 'react-router'
import { routings } from './Routes'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { SnackbarProvider } from 'notistack'

const queryProvider = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <SnackbarProvider maxSnack={3}>
      <QueryClientProvider client={queryProvider}>
        <RouterProvider router={routings} />
      </QueryClientProvider>
    </SnackbarProvider>
  </StrictMode >,
);
