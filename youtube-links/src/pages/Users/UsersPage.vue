<template>
	<v-breadcrumbs :items="items">
		<template v-slot:title="{ item }">
			{{ item.title }}
		</template>
	</v-breadcrumbs>

	<h1>Users</h1>

	<v-data-table-server
		v-model:items-per-page="itemsPerPage"
		:items="usersPagedList?.items"
		:items-length="totalCount"
		:loading="loading"
		loading-text="Loading..."
		:search="query.searchTerm"
		item-value="id"
		:headers="headers"
		:items-per-page-options="itemsPerPageOptions"
		@update:options="updateData">
	</v-data-table-server>
</template>

<script setup lang="ts">
	import { userApiClient } from '@/clients/UserApiClient';
	import type { PagedList } from '@/shared/abstractions/PagedList';
	import { SortOrder, sortingDirectionToEnum } from '@/shared/abstractions/SortOrder';
	import type { GetAllUsers } from '@/shared/features/users/queries/GetAllUsers';
	import type { UserDto } from '@/shared/features/users/responses/UserDto';
	import { onMounted, ref } from 'vue';

	const usersPagedList = ref<PagedList<UserDto>>();
	const loading = ref<boolean>(false);
	const totalCount = ref<number>(0);
	const itemsPerPage = ref<number>(10);
	const query = ref<GetAllUsers.Query>({
		page: 1,
		pageSize: 10,
		sortColumn: '',
		sortOrder: SortOrder.None,
		searchTerm: '',
	});

	const getAllUsers = async () => {
		try {
			loading.value = true;
			usersPagedList.value = await userApiClient.GetAllUsers(query.value);
			totalCount.value = usersPagedList.value.totalCount;
		} catch (error) {
		} finally {
			loading.value = false;
		}
	};

	const updateData = async (options: any) => {
		console.log(options);
		query.value.page = options.page;
		query.value.pageSize = options.itemsPerPage;
		query.value.sortColumn = options?.sortBy[0]?.key ?? '';
		query.value.sortOrder = sortingDirectionToEnum(options?.sortBy[0]?.order);
		console.log('query');
		console.log(query.value);
		// query.value.searchTerm = options.search;
		getAllUsers();
	};

	onMounted(() => {
		// getAllUsers();
	});

	const items = [
		{
			title: 'Users',
			disabled: true,
		},
	];

	const itemsPerPageOptions = [10, 25, 50, 100];
	const headers = [
		{
			title: 'UserName',
			align: 'start',
			sortable: true,
			key: 'userName',
		},
		{
			title: 'Email',
			align: 'start',
			sortable: true,
			key: 'email',
		},
		{
			title: 'Role',
			align: 'start',
			sortable: false,
			key: 'role',
		},
		{
			title: 'Playlists',
			align: 'start',
			sortable: false,
			key: 'playlists',
		},
	] as const;
</script>
