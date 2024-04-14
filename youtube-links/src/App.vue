<script setup lang="ts">
	import { RouterView } from 'vue-router';
	import { ref } from 'vue';
	import { RouteName } from '../src/router/index';
	const drawer = ref<boolean>(true);
	const rail = ref<boolean>(false);
</script>

<template>
	<v-layout>
		<v-app-bar>
			<v-app-bar-nav-icon
				@click.stop="
					drawer = true;
					rail = !rail;
				"
				icon="mdi-menu"></v-app-bar-nav-icon>
			<v-app-bar-title>Youtube Links</v-app-bar-title>
		</v-app-bar>

		<v-navigation-drawer v-model="drawer" :rail="rail" permanent>
			<v-list density="compact" nav>
				<v-list-item link :to="{ name: RouteName.HOME }" title="Home" prepend-icon="mdi-home">
				</v-list-item>
				<v-list-item
					link
					:to="{ name: RouteName.DOWNLOAD_LINK }"
					title="Download Link"
					prepend-icon="mdi-download">
				</v-list-item>
				<v-list-item
					link
					:to="{ name: RouteName.USERS }"
					title="Users"
					prepend-icon="mdi-account-multiple">
				</v-list-item>
				<!-- <v-list-item
					link
					to="/playlists/1"
					title="My Playlists"
					prepend-icon="mdi-filmstrip-box-multiple">
				</v-list-item> -->
				<v-list-item link :to="{ name: RouteName.ABOUT }" title="About" prepend-icon="$vuetify">
				</v-list-item>
				<v-list-item link :to="{ name: RouteName.TEST }" title="Test" prepend-icon="$vuetify">
				</v-list-item>
			</v-list>
		</v-navigation-drawer>

		<v-main>
			<RouterView />
		</v-main>
	</v-layout>
</template>
