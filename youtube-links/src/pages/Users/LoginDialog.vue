<script setup lang="ts">
  import useLogin from '@/clients/Users/Login';
  import { Login } from '@/shared/features/users/commands/Login';
  import { ref } from 'vue';
  import type { VForm } from 'vuetify/components';

  const showDialog = ref<boolean>(false);

  const command = ref<Login.Command>({
    email: '',
    password: '',
  });
  const { jwtDto, loading, validationErrors, login } = useLogin(command.value);

  const form = ref<VForm>();
  const showPassword = ref<boolean>(false);

  const submitForm = () => {
    form.value?.validate().then(({ valid: isValid }) => {
      if (isValid) {
        login().then(() => {
          if (jwtDto.value === undefined) {
            showDialog.value = true;
          } else {
            showDialog.value = false;
            resetForm();
            //store jwtDto in local storage
          }
        });
      }
    });
  };

  const resetForm = () => {
    command.value.email = '';
    command.value.password = '';
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
      <v-btn text="Login" variant="tonal" color="primary" v-bind="activatorProps"></v-btn>
    </template>
    <template v-slot:default="{ isActive }">
      <v-form ref="form">
        <v-card title="Login">
          <v-card-item>
            <v-col class="pa-2">
              <v-text-field
                v-model="command.email"
                label="Email"
                variant="outlined"
                type="email"
                :rules="[
                  Login.Validation.email.notEmpty,
                  Login.Validation.email.maximumLength,
                  Login.Validation.email.isEmailAddress,
                ]"
                :error-messages="validationErrors?.email"
                @input="clearValidationErrors('email')" />
              <!-- hide-details /> -->
              <v-text-field
                v-model="command.password"
                class="mt-2"
                label="Password"
                variant="outlined"
                :type="showPassword ? 'text' : 'password'"
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                @click:append="showPassword = !showPassword"
                :rules="[
                  Login.Validation.password.notEmpty,
                  Login.Validation.password.minimumLength,
                  Login.Validation.password.maximumLength,
                ]"
                :error-messages="validationErrors?.password"
                @input="clearValidationErrors('password')" />
            </v-col>
          </v-card-item>
          <v-card-actions>
            <v-spacer />
            <v-btn text="Login" :loading="loading" @click="submitForm" />
          </v-card-actions>
        </v-card>
      </v-form>
    </template>
  </v-dialog>
</template>
