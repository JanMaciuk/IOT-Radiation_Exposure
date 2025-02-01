import { useMutation } from '@tanstack/react-query';
import { api } from '../utils/api';

export const useRunBackup = () => useMutation({
  mutationKey: ['db-backup'],
  mutationFn: async () => {
    const { data } = await api.post<string>('/db-backup')
    return data;
  }
})