import { useQuery } from '@tanstack/react-query';
import { Entrance } from '../models/types';
import { api } from '../utils/api';

export const useGetZoneEntrances = (zoneId: string | number, enabled: boolean) => useQuery({
  enabled,
  queryKey: ['zones', 'entrances', zoneId],
  queryFn: async () => {
    const { data } = await api.get<Entrance[]>(`/zones/${zoneId}/entrances`)
    return data;
  }
})