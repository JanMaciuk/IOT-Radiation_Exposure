import { createBrowserRouter, createRoutesFromElements, Route } from 'react-router';
import { Home } from './pages/Home';
import { Layout } from './components/Layout';

export const routings = createBrowserRouter(
  createRoutesFromElements(
    <Route element={<Layout />}>
      <Route path="/" element={<Home />} />
    </Route>
  )
);