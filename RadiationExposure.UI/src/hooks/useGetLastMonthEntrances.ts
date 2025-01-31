import { useQuery } from '@tanstack/react-query';
import { api } from '../utils/api';

export const useGetLastMonthEntrances = (employeeId?: string | number) => useQuery({
  queryKey: ['last-month-entrances'],
  queryFn: async () => {
    const { data } = await api.get<{ date: Date, count: number }[]>(`/last-month-entrances` + (employeeId ? `/${employeeId}` : ''));
    return data;
  }
})