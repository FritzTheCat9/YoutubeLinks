<script setup lang="ts">
	import useGetAllUserPlaylists from '@/clients/Playlists/GetAllUserPlaylists';
	import { SortOrder, sortingDirectionToEnum } from '@/shared/abstractions/SortOrder';
	import type { GetAllUserPlaylists } from '@/shared/features/playlists/queries/GetAllUserPlaylists';
	import { ref } from 'vue';
	import { RouteName } from '../../router/index';

	const items = [
		{
			title: 'Users',
			disabled: false,
			to: {
				name: RouteName.USERS,
			},
		},
		{
			title: 'Playlists',
			disabled: true,
		},
	];

	const headers = [
		{
			title: 'Name',
			align: 'start',
			sortable: true,
			key: 'name',
		},
		{
			title: 'Public',
			align: 'start',
			sortable: false,
			key: 'public',
		},
		{
			title: 'Links',
			align: 'start',
			sortable: false,
			key: 'links',
		},
		{
			title: 'Actions',
			align: 'start',
			sortable: false,
			key: 'actions',
		},
	] as const;

	const itemsPerPageOptions = [10, 25, 50, 100];

	const props = defineProps<{
		userId: number;
	}>();

	const query = ref<GetAllUserPlaylists.Query>({
		page: 1,
		pageSize: 10,
		sortColumn: '',
		sortOrder: SortOrder.None,
		searchTerm: '',
		userId: props.userId,
	});

	const { playlistsPagedList, totalCount, loading, validationErrors, getAllUserPlaylists } =
		useGetAllUserPlaylists(query.value);

	const updateData = async (options: any) => {
		query.value.page = options.page;
		query.value.pageSize = options.itemsPerPage;
		query.value.sortColumn = options?.sortBy[0]?.key ?? '';
		query.value.sortOrder = sortingDirectionToEnum(options?.sortBy[0]?.order);
		query.value.searchTerm = options.search;

		getAllUserPlaylists();
	};

	const search = (e: KeyboardEvent) => {
		const inputElement = e.target as HTMLInputElement;
		if (inputElement) {
			query.value.searchTerm = inputElement.value;
		}
	};
</script>

<template>
	<v-breadcrumbs :items="items">
		<template v-slot:title="{ item }">
			{{ item.title }}
		</template>
	</v-breadcrumbs>

	<h1>Playlists</h1>

	<v-text-field
		label="Search"
		variant="outlined"
		append-inner-icon="mdi-magnify"
		class="pa-2"
		@keydown.enter="search($event)" />

	<v-data-table-server
		:items-per-page="query.pageSize"
		:items="playlistsPagedList?.items"
		:items-length="totalCount"
		:loading="loading"
		loading-text="Loading..."
		:search="query.searchTerm"
		:headers="headers"
		:items-per-page-options="itemsPerPageOptions"
		@update:options="updateData">
		<template v-slot:item.public="{ item }">
			<v-icon :icon="item.public ? 'mdi-eye' : 'mdi-eye-off'" />
		</template>
		<template v-slot:item.links="{ item }">
			<router-link :to="`/${RouteName.LINKS}/${props.userId}/${item.id}`">
				<v-icon icon="mdi-format-list-bulleted" color="primary"></v-icon>
			</router-link>
		</template>
		<template v-slot:item.actions="{ item }">
			<!-- <v-icon icon="mdi-plus"></v-icon> -->
		</template>
	</v-data-table-server>

	<div v-if="validationErrors">
		{{ validationErrors }}
	</div>
</template>
