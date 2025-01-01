import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from 'react-router'
import { routings } from './Routes'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={routings} />
  </StrictMode>,
);
