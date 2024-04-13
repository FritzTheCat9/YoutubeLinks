import { ref } from 'vue';
import { userApiClient } from '../UserApiClient';
import type { GetAllUsers } from '@/shared/features/users/queries/GetAllUsers';
import type { PagedList } from '@/shared/abstractions/PagedList';
import type { UserDto } from '@/shared/features/users/responses/UserDto';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';

const useGetAllUsers = (query: GetAllUsers.Query) => {
	const usersPagedList = ref<PagedList<UserDto>>();
	const totalCount = ref<number>(0);
	const loading = ref(false);
	const validationErrors = ref<Record<string, string[]>>();

	const { handleExceptions } = useExceptionHandler();

	const getAllUsers = async (): Promise<void> => {
		validationErrors.value = undefined;

		try {
			loading.value = true;
			usersPagedList.value = await userApiClient.GetAllUsers(query);
			totalCount.value = usersPagedList.value.totalCount;
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

	return { usersPagedList, totalCount, loading, getAllUsers, validationErrors };
};

export default useGetAllUsers;
