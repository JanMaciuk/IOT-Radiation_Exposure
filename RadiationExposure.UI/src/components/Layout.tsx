import { Outlet } from 'react-router';
import { DrawerMenu } from './DrawerMenu';

export const Layout = () => {
  return (
    <div className='flex'>
      <DrawerMenu />
      <div className='bg-white m-12 flex-grow rounded-3xl p-8'>
        <div className='w-3/4 m-auto'>
          <Outlet />
        </div>
      </div>
    </div>
  );
}