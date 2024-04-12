<script setup lang="ts">
	import useGetAllUsers from '@/clients/Users/GetAllUsers';
	import { SortOrder, sortingDirectionToEnum } from '@/shared/abstractions/SortOrder';
	import type { GetAllUsers } from '@/shared/features/users/queries/GetAllUsers';
	import { ref } from 'vue';

	const items = [
		{
			title: 'Users',
			disabled: true,
		},
	];

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

	const itemsPerPageOptions = [10, 25, 50, 100];

	const query = ref<GetAllUsers.Query>({
		page: 1,
		pageSize: 1,
		sortColumn: '',
		sortOrder: SortOrder.None,
		searchTerm: '',
	});

	const { usersPagedList, totalCount, loading, getAllUsers } = useGetAllUsers(query.value);

	const updateData = async (options: any) => {
		query.value.page = options.page;
		query.value.pageSize = options.itemsPerPage;
		query.value.sortColumn = options?.sortBy[0]?.key ?? '';
		query.value.sortOrder = sortingDirectionToEnum(options?.sortBy[0]?.order);
		query.value.searchTerm = options.search;
		getAllUsers();
	};
</script>

<template>
	<v-breadcrumbs :items="items">
		<template v-slot:title="{ item }">
			{{ item.title }}
		</template>
	</v-breadcrumbs>

	<h1>Users</h1>

	<v-data-table-server
		:items-per-page="query.pageSize"
		:items="usersPagedList?.items"
		:items-length="totalCount"
		:loading="loading"
		loading-text="Loading..."
		:search="query.searchTerm"
		:headers="headers"
		:items-per-page-options="itemsPerPageOptions"
		@update:options="updateData">
	</v-data-table-server>
</template>
