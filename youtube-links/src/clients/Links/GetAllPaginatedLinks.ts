import { ref } from 'vue';
import type { PagedList } from '@/shared/abstractions/PagedList';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import { linkApiClient } from '../LinkApiClient';
import type { GetAllPaginatedLinks } from '@/shared/features/links/queries/GetAllPaginatedLinks';
import type { LinkDto } from '@/shared/features/links/responses/LinkDto';

const useGetAllPaginatedLinks = (query: GetAllPaginatedLinks.Query) => {
  const linksPagedList = ref<PagedList<LinkDto>>();
  const totalCount = ref<number>(0);
  const loading = ref(false);
  const validationErrors = ref<Record<string, string[]>>();

  const { handleExceptions } = useExceptionHandler();

  const getAllPaginatedLinks = async (): Promise<void> => {
    validationErrors.value = undefined;

    try {
      loading.value = true;
      linksPagedList.value = await linkApiClient.getAllPaginatedLinks(query);
      totalCount.value = linksPagedList.value.totalCount;
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

  return { linksPagedList, totalCount, loading, validationErrors, getAllPaginatedLinks };
};

export default useGetAllPaginatedLinks;
