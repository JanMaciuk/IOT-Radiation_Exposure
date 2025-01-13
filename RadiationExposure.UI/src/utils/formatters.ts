import { ReactNode } from 'react';

export function formatDuration(duration: number) {
  if (duration < 60) {
    return `${duration} min`;
  }
  const hours = Math.floor(duration / 60);
  const minutes = Math.round(duration % 60);
  return `${hours}h ${minutes}min`;
}

export function formatDateCell(cell: ReactNode) {
  const cellValue = cell as string | number;
  const date = cellValue ? new Date(cellValue) : null;
  return date ? date.toLocaleString() : 'Invalid date';
}