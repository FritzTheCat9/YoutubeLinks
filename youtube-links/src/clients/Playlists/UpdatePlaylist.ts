import { ref } from 'vue';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import { playlistApiClient } from '../PlaylistApiClient';
import type { UpdatePlaylist } from '@/shared/features/playlists/commands/UpdatePlaylist';

const useUpdatePlaylist = (command: UpdatePlaylist.Command) => {
  const loading = ref(false);
  const validationErrors = ref<Record<string, string[]>>();
  const success = ref(false);

  const { handleExceptions } = useExceptionHandler();

  const updatePlaylist = async (): Promise<void> => {
    validationErrors.value = undefined;
    success.value = false;

    try {
      loading.value = true;
      await playlistApiClient.updatePlaylist(command);
      success.value = true;
    } catch (ex) {
      if (ex instanceof MyValidationException) {
        validationErrors.value = (ex as MyValidationException).errors;
      } else {
        handleExceptions(ex as Error);
      }
    } finally {
      loading.value = false;
    }
  };

  return { loading, validationErrors, success, updatePlaylist };
};

export default useUpdatePlaylist;
