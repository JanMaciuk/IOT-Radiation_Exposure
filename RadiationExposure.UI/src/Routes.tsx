import { createBrowserRouter, createRoutesFromElements, Route } from 'react-router';
import { Home } from './pages/Home';
import { Layout } from './components/Layout';
import { Zones } from './pages/Zones';

export const routings = createBrowserRouter(
  createRoutesFromElements(
    <Route element={<Layout />}>
      <Route path="/" element={<Home />} />
      <Route path="/zones" element={<Zones />} />
    </Route>
  )
);