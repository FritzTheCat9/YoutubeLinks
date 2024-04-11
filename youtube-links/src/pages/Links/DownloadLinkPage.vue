<template>
	<v-breadcrumbs :items="items">
		<template v-slot:title="{ item }">
			{{ item.title }}
		</template>
	</v-breadcrumbs>

	<h1>Download link</h1>

	<v-row no-gutters>
		<v-col class="pa-2" cols="12" md="5">
			<v-text-field label="Url" variant="outlined"> </v-text-field>
		</v-col>
		<v-col class="pa-2" cols="12" md="5">
			<v-select label="File Type" :items="['MP3', 'MP4']" variant="outlined"> </v-select>
		</v-col>
		<v-col class="pa-2" cols="12" md="2">
			<v-btn :loading="loading" height="52" variant="tonal" @click="load"> Download </v-btn>
		</v-col>
	</v-row>
</template>

<script setup lang="ts">
	import { ref } from 'vue';

	const items = [
		{
			title: 'Download link',
			disabled: true,
		},
	];
	const loading = ref<boolean>(false);

	const load = () => {
		loading.value = true;
		setTimeout(() => {
			loading.value = false;
		}, 1000);
	};
</script>
