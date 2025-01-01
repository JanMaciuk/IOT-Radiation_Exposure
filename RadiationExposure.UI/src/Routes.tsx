import { createBrowserRouter, createRoutesFromElements, Route } from 'react-router';
import { Home } from './pages/Home';

export const routings = createBrowserRouter(
  createRoutesFromElements(
    <Route>
      <Route path="/" element={<Home />} />
    </Route>
  )
);