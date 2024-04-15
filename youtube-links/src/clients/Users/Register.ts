import { ref } from 'vue';
import { userApiClient } from '../UserApiClient';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import type { Register } from '@/shared/features/users/commands/Register';

const useRegister = (command: Register.Command) => {
  const loading = ref(false);
  const validationErrors = ref<Record<string, string[]>>();
  const success = ref(false);

  const { handleExceptions } = useExceptionHandler();

  const register = async (): Promise<void> => {
    validationErrors.value = undefined;
    success.value = false;

    try {
      loading.value = true;
      await userApiClient.Register(command);
      success.value = true;
    } catch (ex) {
      if (ex instanceof MyValidationException) {
        validationErrors.value = (ex as MyValidationException).errors;
      } else {
        handleExceptions(ex as Error);
      }
    } finally {
      loading.value = false;
    }
  };

  return { loading, validationErrors, success, register };
};

export default useRegister;
