import { MRT_ColumnDef, useMaterialReactTable, MaterialReactTable } from 'material-react-table';
import { useMemo } from 'react';
import { PageHeader } from '../components/PageHeader';
import { Zone } from '../models/types';
import { useGetZones } from '../hooks/useGetZones';
import { EntrancesTable } from '../components/EntrancesTable';
import { formatDateCell } from '../utils/formatters';

export const Zones = () => {
  const { data: zoneList } = useGetZones();

  const columns = useMemo<MRT_ColumnDef<Zone>[]>(
    () => [
      {
        accessorKey: 'zoneName',
        header: 'Name',
        size: 150
      },
      {
        accessorKey: 'radiation',
        header: 'Radiation',
        size: 150
      },
      {
        accessorKey: 'lastEntrance',
        header: 'Last entrance',
        size: 300,
        Cell: ({ renderedCellValue }) => renderedCellValue ? formatDateCell(renderedCellValue) : 'No entrances'
      },
      {
        accessorKey: 'employeesInsideNow',
        header: 'Employess inside now',
        size: 100
      }
    ], []
  )

  const table = useMaterialReactTable({
    columns,
    data: zoneList ?? [],
    enableExpanding: true,
    renderDetailPanel: (rowData) => (
      <EntrancesTable zoneId={rowData.row.original.id} />
    )
  })
  
  return (
    <>
      <PageHeader title='Zones' />
      <MaterialReactTable table={table} />
    </>
  )
}