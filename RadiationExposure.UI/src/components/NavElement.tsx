import { Link } from 'react-router';

interface NavElementProps {
  text: string;
  link: string;
  icon: React.ReactNode;
}

export const NavElement = ({ text, link, icon }: NavElementProps) => {
  return (
    <Link to={link} key={text} color="inherit">
      <div
        className='flex flex-col justify-center hover:transition duration-100 my-4 text-sm hover:text-black text-center group'
      >
        <div className="duration-200 rounded-3xl w-2/3 p-1 mx-auto group-hover:bg-[#E8DEF8]">{icon}</div>
        <div className="font-medium">{text}</div>
      </div>
    </Link>
  );
}