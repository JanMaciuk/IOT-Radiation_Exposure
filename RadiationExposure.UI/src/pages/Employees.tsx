import { useMemo } from 'react';
import { useGetEmployees } from '../hooks/useGetEmployees'
import {
  MaterialReactTable,
  useMaterialReactTable,
  type MRT_ColumnDef,
} from 'material-react-table';
import { Employee } from '../models/types';
import { PageHeader } from '../components/PageHeader';
import { formatDateCell } from '../utils/formatters';
import { EmployeePanel } from '../components/EmployeePanel';

export const Employees = () => {
  const { data: employeeList } = useGetEmployees();

  const columns = useMemo<MRT_ColumnDef<Employee>[]>(
    () => [
      {
        accessorKey: 'name',
        header: 'First name',
        size: 150
      },
      {
        accessorKey: 'surname',
        header: 'Last name',
        size: 150
      },
      {
        accessorKey: 'lastZoneName',
        header: 'Last entered zone',
        size: 200,
      },
      {
        accessorKey: 'lastEntranceDate',
        header: 'Last entrance',
        size: 300,
        Cell: ({ renderedCellValue }) => renderedCellValue ? formatDateCell(renderedCellValue) : 'No entrances'
      },
      {
        accessorKey: 'radiationDoseInLastWeek',
        header: 'Radiation dose in last week',
        size: 150,
      },
      {
        accessorKey: 'radiationDoseInLastMonth',
        header: 'Radiation dose in last month',
        size: 150,
      },
    ], []
  )

  const table = useMaterialReactTable({
    columns,
    data: employeeList ?? [],
    enableExpanding: true,
    renderDetailPanel: (rowData) => (
      <EmployeePanel employeeId={rowData.row.original.id} />
    )
  })

  return (
    <>
      <PageHeader title='Employees' />
      <MaterialReactTable table={table} />
    </>
  )
}