import { Outlet } from 'react-router';
import { DrawerMenu } from './DrawerMenu';

export const Layout = () => {
  return (
    <>
      <DrawerMenu />
      <Outlet />
    </>
  );
}