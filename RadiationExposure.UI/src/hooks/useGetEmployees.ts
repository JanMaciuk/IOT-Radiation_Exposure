import { useQuery } from '@tanstack/react-query';
import { api } from '../utils/api';
import { Employee } from '../models/types';

export const useGetEmployees = () => useQuery({
  queryKey: ['employees'],
  queryFn: async () => {
    const { data } = await api.get<Employee[]>('/employees')
    return data;
  }
})