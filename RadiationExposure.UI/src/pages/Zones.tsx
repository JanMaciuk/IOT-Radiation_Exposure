import { Table, TableCell, TableHead, TableRow } from '@mui/material';

export const Zones = () => {
  return (
    <>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Room Name</TableCell>
            <TableCell>Room Number</TableCell>
            <TableCell>Capacity</TableCell>
            <TableCell>Location</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
      </Table> 
    </>
  );
}