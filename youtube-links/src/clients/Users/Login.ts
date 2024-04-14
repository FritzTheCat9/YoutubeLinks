import { ref } from 'vue';
import { userApiClient } from '../UserApiClient';
import { MyValidationException } from '@/shared/exceptions/CustomException';
import useExceptionHandler from '../ExceptionHandler';
import type { Login } from '@/shared/features/users/commands/Login';
import type { JwtDto } from '@/shared/features/users/responses/JwtDto';

const useLogin = (command: Login.Command) => {
	const jwtDto = ref<JwtDto>();
	const loading = ref(false);
	const validationErrors = ref<Record<string, string[]>>();

	const { handleExceptions } = useExceptionHandler();

	const login = async (): Promise<void> => {
		validationErrors.value = undefined;
		jwtDto.value = undefined;

		try {
			loading.value = true;
			jwtDto.value = await userApiClient.Login(command);
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

	return { jwtDto, loading, validationErrors, login };
};

export default useLogin;
