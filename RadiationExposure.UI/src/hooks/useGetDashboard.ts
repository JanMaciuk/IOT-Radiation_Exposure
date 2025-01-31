import { useQuery } from '@tanstack/react-query';
import { DashboardStats, Entrance } from '../models/types';
import { api } from '../utils/api';

export const useGetDashboard = () => useQuery({
  queryKey: ['dashboard'],
  queryFn: async () => {
    const { data } = await api.get<DashboardStats>(`/dashboard`)
    return data;
  }
})