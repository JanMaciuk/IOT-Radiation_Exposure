export const PageHeader = ({title}: { title: string }) => {
  return (
    <header>
      <h1 className='text-4xl font-bold text-center mb-8'>{title}</h1>
    </header>
  );
}