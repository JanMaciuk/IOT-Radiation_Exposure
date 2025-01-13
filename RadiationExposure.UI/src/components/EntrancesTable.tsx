import { MaterialReactTable, MRT_ColumnDef, useMaterialReactTable } from 'material-react-table';
import { useGetZoneEntrances } from '../hooks/useGetZoneEntrances';
import { useMemo } from 'react';
import { Entrance } from '../models/types';
import { formatDateCell, formatDuration } from '../utils/formatters';

export const EntrancesTable = ({ zoneId }: { zoneId: string | number }) => {
  const { data } = useGetZoneEntrances(zoneId, zoneId !== '');

  const columns = useMemo<MRT_ColumnDef<Entrance>[]>(
    () => [
      {
        accessorKey: 'employeeName',
        header: 'Employee',
        size: 150,
      },
      {
        accessorKey: 'entryTime',
        header: 'Entry Time',
        size: 150,
        Cell: ({ renderedCellValue }) => formatDateCell(renderedCellValue),
      },
      {
        accessorKey: 'exitTime',
        header: 'Exit Time',
        size: 150,
        Cell: ({ renderedCellValue }) => formatDateCell(renderedCellValue) || 'Still inside',
      },
      {
        accessorKey: 'duration',
        header: 'Duration',
        size: 150,
        Cell: ({ renderedCellValue }) => formatDuration(renderedCellValue as number),
      },
      {
        accessorKey: 'radiationDose',
        header: 'Radiation Dose',
        size: 150,
      },
    ],
    []
  );

  const table = useMaterialReactTable({
    columns,
    data: data || [],
  });

  return (
    <MaterialReactTable table={table} />
  );
}