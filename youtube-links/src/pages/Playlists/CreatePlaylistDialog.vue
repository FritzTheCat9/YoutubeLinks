<script setup lang="ts">
  import useCreatePlaylist from '@/clients/Playlists/CreatePlaylist';
  import { CreatePlaylist } from '@/shared/features/playlists/commands/CreatePlaylist';
  import { ref } from 'vue';
  import type { VForm } from 'vuetify/components';

  const showDialog = ref<boolean>(false);

  const command = ref<CreatePlaylist.Command>({
    name: '',
    public: true,
  });
  const { loading, validationErrors, success, createPlaylist } = useCreatePlaylist(command.value);

  const form = ref<VForm>();

  const submitForm = () => {
    form.value?.validate().then(({ valid: isValid }) => {
      if (isValid) {
        createPlaylist().then(() => {
          if (success.value === undefined) {
            showDialog.value = true;
          } else {
            showDialog.value = false;
            resetForm();
          }
        });
      }
    });
  };

  const resetForm = () => {
    command.value.name = '';
    command.value.public = true;
    validationErrors.value = undefined;
  };

  const clearValidationErrors = (fieldName: string) => {
    if (validationErrors.value) {
      validationErrors.value[fieldName] = [];
    }
  };
</script>

<template>
  <v-dialog v-model="showDialog" max-width="600" @click:outside="resetForm">
    <template v-slot:activator="{ props: activatorProps }">
      <v-icon icon="mdi-plus" v-bind="activatorProps"></v-icon>
    </template>
    <template v-slot:default="{ isActive }">
      <v-form ref="form">
        <v-card title="Create Playlist">
          <v-card-item>
            <v-col class="pa-2">
              <v-text-field
                v-model="command.name"
                label="Name"
                variant="outlined"
                type="text"
                :rules="[
                  CreatePlaylist.Validation.name.notEmpty,
                  CreatePlaylist.Validation.name.maximumLength,
                ]"
                :error-messages="validationErrors?.name"
                @input="clearValidationErrors('name')" />
              <v-checkbox v-model="command.public" label="Public" class="mt-2" />
            </v-col>
          </v-card-item>
          <v-card-actions>
            <v-spacer />
            <v-btn text="Create" :loading="loading" @click="submitForm" />
          </v-card-actions>
        </v-card>
      </v-form>
    </template>
  </v-dialog>
</template>
