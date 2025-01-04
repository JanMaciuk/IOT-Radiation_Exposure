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
      text: "Workers",
      link: "/workers",
      icon: <Person />,
    },
    {
      text: "Management",
      link: "/management",
      icon: <ManageSearch />,
    },
  ];

  return (
    <Box className='sticky w-32 p-4 mt-12 min-h-screen'>
      <List className="flex flex-col">
        {navElements.map((navElement) => (
          <NavElement key={navElement.text} {...navElement} />
        ))}
      </List>
    </Box>
  );
};
