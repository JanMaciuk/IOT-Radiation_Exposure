import { useGetLastMonthEntrances } from '../hooks/useGetLastMonthEntrances';
import { CircularProgress } from '@mui/material';
import { DateCountGraph } from './DateCountGraph';

export const EntrancesGraph = () => {
  const { data, isLoading } = useGetLastMonthEntrances();

  return (
    <div className="w-full h-64">
      <h2 className="text-2xl font-bold mb-4 text-center">Entrances in the last month</h2>
      {isLoading && (
        <div className='flex justify-center items-center w-full h-full'>
          <CircularProgress />
        </div>
      )}
      {data && (
        <DateCountGraph 
          records={data.map(d => ({ date: d.date, count: d.count }))}
        />
      )}
    </div>
  );
}


