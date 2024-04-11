<template>
    <h1>Users</h1>
    <v-data-table-server v-model:items-per-page="itemsPerPage" :items="usersPagedList?.items" :items-length="totalCount"
        :loading="loading" loading-text="Loading..." :search="query.searchTerm" item-value="name" :headers="headers"
        @update:options="getAllUsers">
    </v-data-table-server>

    <ul v-if="usersPagedList?.items">
        <li v-for="(user, index) in usersPagedList?.items" :key="index">
            {{ user.userName }} - {{ user.email }}
        </li>
    </ul>
    <p v-else>Loading...</p>
</template>

<script setup lang="ts">
import { userApiClient } from '@/clients/UserApiClient';
import type { PagedList } from '@/shared/abstractions/PagedList';
import { SortOrder } from '@/shared/abstractions/SortOrder';
import type { GetAllUsers } from '@/shared/features/users/queries/GetAllUsers';
import type { UserDto } from '@/shared/features/users/responses/UserDto';
import { onMounted, ref } from 'vue';

const usersPagedList = ref<PagedList<UserDto>>();
const loading = ref<boolean>(false);
const totalCount = ref<number>(0);
const itemsPerPage = ref<number>(1);
const query = ref<GetAllUsers.Query>({
    page: 1,
    pageSize: 10,
    sortColumn: '',
    sortOrder: SortOrder.None,
    searchTerm: '',
})

const delay = (ms: number) => new Promise(res => setTimeout(res, ms));

const getAllUsers = async () => {
    try {
        loading.value = true;
        await delay(5000);
        usersPagedList.value = await userApiClient.GetAllUsers(query.value);
        totalCount.value = usersPagedList.value.totalCount;
        console.log('result:', usersPagedList);
    } catch (error) {
        console.error('error:', error);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    // getAllUsers();
});

let headers = [
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