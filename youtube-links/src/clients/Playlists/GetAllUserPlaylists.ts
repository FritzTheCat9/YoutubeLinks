import { ref } from 'vue';
import type { PagedList } from '@/shared/abstractions/PagedList';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import type { GetAllUserPlaylists } from '@/shared/features/playlists/queries/GetAllUserPlaylists';
import type { PlaylistDto } from '@/shared/features/playlists/responses/PlaylistDto';
import { playlistApiClient } from '../PlaylistApiClient';

const useGetAllUserPlaylists = (query: GetAllUserPlaylists.Query) => {
  const playlistsPagedList = ref<PagedList<PlaylistDto>>();
  const totalCount = ref<number>(0);
  const loading = ref(false);
  const validationErrors = ref<Record<string, string[]>>();

  const { handleExceptions } = useExceptionHandler();

  const getAllUserPlaylists = async (): Promise<void> => {
    validationErrors.value = undefined;

    try {
      loading.value = true;
      playlistsPagedList.value = await playlistApiClient.getAllUserPlaylists(query);
      totalCount.value = playlistsPagedList.value.totalCount;
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

  return { playlistsPagedList, totalCount, loading, validationErrors, getAllUserPlaylists };
};

export default useGetAllUserPlaylists;
