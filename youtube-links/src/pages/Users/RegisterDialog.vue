<script setup lang="ts">
  import useRegister from '@/clients/Users/Register';
  import { Register } from '@/shared/features/users/commands/Register';
  import { ThemeColor } from '@/shared/features/users/helpers/ThemeColor';
  import { ref } from 'vue';
  import type { VForm } from 'vuetify/components';

  const showDialog = ref<boolean>(false);

  const command = ref<Register.Command>({
    email: '',
    userName: '',
    password: '',
    repeatPassword: '',
    themeColor: ThemeColor.System, // TODO: load themeColor
  });
  const { loading, validationErrors, success, register } = useRegister(command.value);

  const form = ref<VForm>();
  const showPassword = ref<boolean>(false);
  const showRepeatPassword = ref<boolean>(false);

  const submitForm = () => {
    form.value?.validate().then(({ valid: isValid }) => {
      if (isValid) {
        register().then(() => {
          if (success.value === undefined) {
            showDialog.value = true;
          } else {
            showDialog.value = false;
            resetForm();
            console.log(command.value);
            //TODO: show success information
          }
        });
      }
    });
  };

  const resetForm = () => {
    command.value.email = '';
    command.value.userName = '';
    command.value.password = '';
    command.value.repeatPassword = '';
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
      <v-btn text="Register" variant="tonal" color="primary" v-bind="activatorProps"></v-btn>
    </template>
    <template v-slot:default="{ isActive }">
      <v-form ref="form">
        <v-card title="Register">
          <v-card-item>
            <v-col class="pa-2">
              <v-text-field
                v-model="command.email"
                label="Email"
                variant="outlined"
                type="email"
                :rules="[
                  Register.Validation.email.notEmpty,
                  Register.Validation.email.maximumLength,
                  Register.Validation.email.isEmailAddress,
                ]"
                :error-messages="validationErrors?.email"
                @input="clearValidationErrors('email')" />
              <v-text-field
                v-model="command.userName"
                class="mt-2"
                label="UserName"
                variant="outlined"
                type="text"
                :rules="[
                  Register.Validation.userName.notEmpty,
                  Register.Validation.userName.minimumLength,
                  Register.Validation.userName.maximumLength,
                  Register.Validation.userName.matchesUserNameRegex,
                ]"
                :error-messages="validationErrors?.userName"
                @input="clearValidationErrors('userName')" />
              <v-text-field
                v-model="command.password"
                class="mt-2"
                label="Password"
                variant="outlined"
                :type="showPassword ? 'text' : 'password'"
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                @click:append="showPassword = !showPassword"
                :rules="[
                  Register.Validation.password.notEmpty,
                  Register.Validation.password.minimumLength,
                  Register.Validation.password.maximumLength,
                ]"
                :error-messages="validationErrors?.password"
                @input="clearValidationErrors('password')" />
              <v-text-field
                v-model="command.repeatPassword"
                class="mt-2"
                label="Repeat Password"
                variant="outlined"
                :type="showRepeatPassword ? 'text' : 'password'"
                :append-icon="showRepeatPassword ? 'mdi-eye' : 'mdi-eye-off'"
                @click:append="showRepeatPassword = !showRepeatPassword"
                :rules="[
                  Register.Validation.repeatPassword.notEmpty,
                  Register.Validation.repeatPassword.minimumLength,
                  Register.Validation.repeatPassword.maximumLength,
                  Register.Validation.repeatPassword.equalPassword(
                    command.repeatPassword,
                    command.password
                  ),
                ]"
                :error-messages="validationErrors?.repeatPassword"
                @input="clearValidationErrors('repeatPassword')" />
            </v-col>
          </v-card-item>
          <v-card-actions>
            <v-spacer />
            <v-btn text="Register" :loading="loading" @click="submitForm" />
          </v-card-actions>
        </v-card>
      </v-form>
    </template>
  </v-dialog>
</template>
