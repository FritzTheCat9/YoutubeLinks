import { ref } from 'vue';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import type { CreatePlaylist } from '@/shared/features/playlists/commands/CreatePlaylist';
import { playlistApiClient } from '../PlaylistApiClient';

const useCreatePlaylist = (command: CreatePlaylist.Command) => {
  const loading = ref(false);
  const validationErrors = ref<Record<string, string[]>>();
  const success = ref(false);

  const { handleExceptions } = useExceptionHandler();

  const createPlaylist = async (): Promise<void> => {
    validationErrors.value = undefined;
    success.value = false;

    try {
      loading.value = true;
      await playlistApiClient.createPlaylist(command);
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

  return { loading, validationErrors, success, createPlaylist };
};

export default useCreatePlaylist;
