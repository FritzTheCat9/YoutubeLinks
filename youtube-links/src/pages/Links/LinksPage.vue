<script setup lang="ts">
  import { SortOrder, sortingDirectionToEnum } from '@/shared/abstractions/SortOrder';
  import { ref } from 'vue';
  import { RouteName } from '../../router/index';
  import type { GetAllPaginatedLinks } from '@/shared/features/links/queries/GetAllPaginatedLinks';
  import useGetAllPaginatedLinks from '@/clients/Links/GetAllPaginatedLinks';
  import type { VDataTableServer } from 'vuetify/components';

  const props = defineProps<{
    userId: number;
    playlistId: number;
  }>();

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
      disabled: false,
      to: {
        path: `/${RouteName.PLAYLISTS}/${props.userId}`,
      },
    },
    {
      title: 'Links',
      disabled: true,
    },
  ];

  const headers = [
    {
      title: 'Title',
      align: 'start',
      sortable: true,
      key: 'title',
    },
    {
      title: 'Modified',
      align: 'start',
      sortable: true,
      key: 'modified',
    },
    {
      title: 'Downloaded',
      align: 'start',
      sortable: false,
      key: 'downloaded',
    },
    {
      title: 'Actions',
      align: 'start',
      sortable: false,
      key: 'actions',
    },
  ] as const;

  const itemsPerPageOptions = [10, 25, 50, 100];

  const query = ref<GetAllPaginatedLinks.Query>({
    page: 1,
    pageSize: 10,
    sortColumn: 'modified',
    sortOrder: SortOrder.Descending,
    searchTerm: '',
    playlistId: props.playlistId,
  });

  const { linksPagedList, totalCount, loading, validationErrors, getAllPaginatedLinks } =
    useGetAllPaginatedLinks(query.value);

  const updateData = async (options: any) => {
    query.value.page = options.page;
    query.value.pageSize = options.itemsPerPage;
    query.value.sortColumn = options?.sortBy[0]?.key ?? '';
    query.value.sortOrder = sortingDirectionToEnum(options?.sortBy[0]?.order);
    query.value.searchTerm = options.search;

    getAllPaginatedLinks();
  };

  const search = (e: KeyboardEvent) => {
    const inputElement = e.target as HTMLInputElement;
    if (inputElement) {
      query.value.searchTerm = inputElement.value;
    }
  };

  const formatDate = (dateString: string | Date) => {
    const date = new Date(dateString);
    return date.toLocaleString('pl-PL', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
  };

  const table = ref<VDataTableServer>(); // TODO: set default table sorting
  // table.value?.$options.sortBy[0].key = 'modified';
  // table.value.sortBy[0].order = SortOrder.Descending;
</script>

<template>
  <v-breadcrumbs :items="items">
    <template v-slot:title="{ item }">
      {{ item.title }}
    </template>
  </v-breadcrumbs>

  <!-- TODO: create link form  -->

  <h1>Links</h1>
  <!-- <span class="mdi mdi-flag-outline"></span> -->
  <!-- <span class="mdi mdi-flag"></span> -->
  <!-- <span class="mdi mdi-view-grid-outline"></span> -->

  <v-text-field
    label="Search"
    variant="outlined"
    append-inner-icon="mdi-magnify"
    class="pa-2"
    @keydown.enter="search($event)" />

  <v-data-table-server
    ref="table"
    :items-per-page="query.pageSize"
    :items="linksPagedList?.items"
    :items-length="totalCount"
    :loading="loading"
    loading-text="Loading..."
    :search="query.searchTerm"
    :headers="headers"
    :items-per-page-options="itemsPerPageOptions"
    @update:options="updateData">
    <template v-slot:item.modified="{ item }">
      {{ formatDate(item.modified) }}
    </template>
    <template v-slot:item.downloaded="{ item }">
      <v-icon :icon="item.downloaded ? 'mdi-check-bold' : 'mdi-download'" />
    </template>
    <template v-slot:item.actions="{ item }">
      <!-- <span class="mdi mdi-content-copy"></span> -->
      <!-- <span class="mdi mdi-download"></span> -->
      <!-- <span class="mdi mdi-filmstrip-box-multiple"></span> -->
      <!-- <span class="mdi mdi-pencil"></span> -->
      <!-- <span class="mdi mdi-delete"></span> -->
    </template>
  </v-data-table-server>
</template>
