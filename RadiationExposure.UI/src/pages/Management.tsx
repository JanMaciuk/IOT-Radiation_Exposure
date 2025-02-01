import { Button } from '@mui/material';
import { PageHeader } from '../components/PageHeader';
import BackupIcon from '@mui/icons-material/Backup';
import { useRunBackup } from '../hooks/useRunBackup';
import { useSnackbar } from 'notistack';


export const Management = () => {
  const { mutateAsync } = useRunBackup();
  const snackbar = useSnackbar();

  const handleRunBackup = async () => {
    const backupResponse = await mutateAsync();

    snackbar.enqueueSnackbar(backupResponse, {
      variant: 'success'
    });
  }

  return (
    <div>
      <PageHeader title='Management' />
      <div className='text-center'>
        <h2>Run database backup</h2>
        <Button
          variant="contained"
          color="info"
          startIcon={<BackupIcon />}
          onClick={handleRunBackup}
        >
          Run backup
        </Button>
      </div>
    </div>
  );
}