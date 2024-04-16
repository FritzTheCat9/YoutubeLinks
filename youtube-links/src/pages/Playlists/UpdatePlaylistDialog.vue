<script setup lang="ts">
  import useUpdatePlaylist from '@/clients/Playlists/UpdatePlaylist';
  import { UpdatePlaylist } from '@/shared/features/playlists/commands/UpdatePlaylist';
  import { ref } from 'vue';
  import type { VForm } from 'vuetify/components';

  const props = defineProps<{
    command: UpdatePlaylist.Command;
  }>();

  const idCopy = props.command.id;
  const nameCopy = props.command.name;
  const publicCopy = props.command.public;

  const showDialog = ref<boolean>(false);

  const command = ref<UpdatePlaylist.Command>({
    id: props.command.id,
    name: props.command.name,
    public: props.command.public,
  });
  const { loading, validationErrors, success, updatePlaylist } = useUpdatePlaylist(command.value);

  const form = ref<VForm>();

  const submitForm = () => {
    form.value?.validate().then(({ valid: isValid }) => {
      if (isValid) {
        updatePlaylist().then(() => {
          if (success.value === undefined) {
            showDialog.value = true;
          } else {
            showDialog.value = false;
            resetForm();
            // call parent function to update the table
          }
        });
      }
    });
  };

  const onOpen = () => {
    command.value = props.command;
  };

  const resetForm = () => {
    command.value.id = idCopy;
    command.value.name = nameCopy;
    command.value.public = publicCopy;
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
      <v-icon icon="mdi-pencil" v-bind="activatorProps" @click="onOpen"></v-icon>
    </template>
    <template v-slot:default="{ isActive }">
      <v-form ref="form">
        <v-card title="Update Playlist">
          <v-card-item>
            <v-col class="pa-2">
              <v-text-field
                v-model="command.name"
                label="Name"
                variant="outlined"
                type="text"
                :rules="[
                  UpdatePlaylist.Validation.name.notEmpty,
                  UpdatePlaylist.Validation.name.maximumLength,
                ]"
                :error-messages="validationErrors?.name"
                @input="clearValidationErrors('name')" />
              <v-checkbox v-model="command.public" label="Public" class="mt-2" />
            </v-col>
          </v-card-item>
          <v-card-actions>
            <v-spacer />
            <v-btn text="Update" :loading="loading" @click="submitForm" />
          </v-card-actions>
        </v-card>
      </v-form>
    </template>
  </v-dialog>
</template>
