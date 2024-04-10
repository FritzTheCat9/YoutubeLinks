<template>
    <div class="about">
        <h1>This is test page</h1>
        <v-btn variant="tonal" class="ml-2" @click="handleClick">
            vuetify button test
        </v-btn>
    </div>
</template>

<script setup lang="ts">
const handleClick = () => {
    console.log("BUTTON CLICKED");
}
</script>

<style>
/* @media (min-width: 1024px) {
    .about {
        min-height: 100vh;
        display: flex;
        align-items: center;
    }
} */
</style>