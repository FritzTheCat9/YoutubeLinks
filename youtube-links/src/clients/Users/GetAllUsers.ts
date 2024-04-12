import { ref } from 'vue';
import { userApiClient } from '../UserApiClient';
import type { GetAllUsers } from '@/shared/features/users/queries/GetAllUsers';
import type { PagedList } from '@/shared/abstractions/PagedList';
import type { UserDto } from '@/shared/features/users/responses/UserDto';

const useGetAllUsers = (query: GetAllUsers.Query) => {
	const usersPagedList = ref<PagedList<UserDto>>();
	const totalCount = ref<number>(0);
	const loading = ref(false);

	const getAllUsers = async () => {
		try {
			loading.value = true;
			usersPagedList.value = await userApiClient.GetAllUsers(query);
			totalCount.value = usersPagedList.value.totalCount;
		} catch (err) {
			// handle error
		} finally {
			loading.value = false;
		}
	};

	return { usersPagedList, totalCount, loading, getAllUsers };
};

export default useGetAllUsers;
