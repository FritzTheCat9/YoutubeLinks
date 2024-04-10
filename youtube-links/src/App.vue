<template>
  <v-layout>
    <v-app-bar>
      <v-app-bar-nav-icon @click.stop="drawer = true; rail = !rail;" icon="mdi-menu"></v-app-bar-nav-icon>
      <v-app-bar-title>Youtube Links</v-app-bar-title>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" :rail="rail" permanent>
      <v-list density="compact" nav>
        <v-list-item link to="/" title="Home" prepend-icon="mdi-home">
        </v-list-item>
        <v-list-item link to="/download-link" title="Download Link" prepend-icon="mdi-download">
        </v-list-item>
        <v-list-item link to="/users" title="Users" prepend-icon="mdi-account-multiple">
        </v-list-item>
        <v-list-item link to="/my-playlists" title="My Playlists" prepend-icon="mdi-filmstrip-box-multiple">
        </v-list-item>

        <v-list-item link to="/about" title="About" prepend-icon="$vuetify">
        </v-list-item>
        <v-list-item link to="/test" title="Test" prepend-icon="$vuetify">
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <RouterView />
    </v-main>
  </v-layout>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import { ref } from 'vue';

const drawer = ref<boolean>(true);
const rail = ref<boolean>(false);

</script>