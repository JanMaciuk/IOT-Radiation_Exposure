import { EntrancesGraph } from '../components/EntrancesGraph';
import { useGetDashboard } from '../hooks/useGetDashboard';

export const Home = () => {
  const { data } = useGetDashboard();

  const stats = [
    { name: 'Employee count', value: data?.totalEmployees },
    { name: 'Total Zones', value: data?.totalZones },
    { name: 'Entrances today', value: data?.entrancesToday },
    { name: 'Workers above radiation limit', value: data?.workersAboveRadiationLimit },
  ]

  return (
    <div className="flex flex-wrap justify-between w-full py-12">
      {stats.map(({name, value}) => (
        <div key={name} className="flex flex-col flex-1 text-2xl my-4 text-center group">
          <div className="font-bold text-4xl">{value}</div>
          <div>{name}</div>  
        </div>
      ))}

      <EntrancesGraph />
    </div>
  )
}