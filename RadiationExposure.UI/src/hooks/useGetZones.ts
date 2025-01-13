import { useQuery } from '@tanstack/react-query';
import { Zone } from '../models/types';
import { api } from '../utils/api';

export const useGetZones = () => useQuery({
  queryKey: ['zones'],
  queryFn: async () => {
    const { data } = await api.get<Zone[]>('/zones')
    return data;
  }
})