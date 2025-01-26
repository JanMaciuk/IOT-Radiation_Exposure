import Box from "@mui/material/Box";
import List from "@mui/material/List";
import { DashboardSharp, ManageSearch, Person, Room } from "@mui/icons-material";
import { NavElement } from "./NavElement";

export const DrawerMenu = () => {
  const navElements = [
    {
      text: "Dashboard",
      link: "/",
      icon: <DashboardSharp />,
    },
    {
      text: "Zones",
      link: "/zones",
      icon: <Room />,
    },
    {
      text: "Employees",
      link: "/employees",
      icon: <Person />,
    },
    {
      text: "Management",
      link: "/management",
      icon: <ManageSearch />,
    },
  ];

  return (
    <nav>
      <header className='text-2xl font-bold p-4'>RadiationExp.</header>
      <Box className='sticky w-32 p-4 mt-8 min-h-screen'>
        <List className="flex flex-col">
          {navElements.map((navElement) => (
            <NavElement key={navElement.text} {...navElement} />
          ))}
        </List>
      </Box>
    </nav>
  );
};
