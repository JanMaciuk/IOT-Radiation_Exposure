import { createBrowserRouter, createRoutesFromElements, Route } from 'react-router';
import { Home } from './pages/Home';
import { Layout } from './components/Layout';
import { Zones } from './pages/Zones';
import { Employees } from './pages/Employees';
import { Management } from './pages/Management';

export const routings = createBrowserRouter(
  createRoutesFromElements(
    <Route element={<Layout />}>
      <Route path="/" element={<Home />} />
      <Route path="/zones" element={<Zones />} />
      <Route path="/employees" element={<Employees />} />
      <Route path="/management" element={<Management />} />
    </Route>
  )
);