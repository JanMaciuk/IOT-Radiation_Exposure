import { stats } from '../mocks/stats'

export const Home = () => {
  return (
    <div className="flex flex-wrap justify-between w-full py-12">
      {stats.map(({name, value}) => (
        <div key={name} className="flex flex-col flex-1 text-2xl my-4 text-center group">
          <div className="font-bold text-4xl">{value}</div>
          <div>{name}</div>  
        </div>
      ))}
    </div>
  )
}