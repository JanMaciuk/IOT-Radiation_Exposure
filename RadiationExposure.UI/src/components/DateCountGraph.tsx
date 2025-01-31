import { BarChart } from '@mui/x-charts';

export const DateCountGraph = ({ records, recordLabel }: { 
  records: { date: Date, count: number }[], 
  recordLabel?: string }
) => {
  return (
    <BarChart
      xAxis={[{ data: records.map(d => new Date(d.date).toLocaleDateString()), scaleType: 'band' }]}
      yAxis={[{ label: 'Entrance count', tickMinStep: 1 }]}
      series={[{ data: records.map(d => d.count), label: recordLabel, type: 'bar' }]}
    />
  );
}