import { useQuery } from '@tanstack/react-query';
import { api } from '../utils/api';

export const useGetLastMonthEntrances = () => useQuery({
  queryKey: ['last-month-entrances'],
  queryFn: async () => {
    const { data } = await api.get<{ date: Date, count: number }[]>(`/last-month-entrances`)
    return data;
  }
})