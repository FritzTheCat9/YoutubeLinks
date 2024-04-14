import router from '@/router';
import {
	MyForbiddenException,
	MyNotFoundException,
	MyServerException,
	MyUnauthorizedException,
	MyValidationException,
} from '@/shared/exceptions/CustomException';
import { RouteName } from '../router/index';

const useExceptionHandler = () => {
	function handleExceptions(exception: Error): void {
		if (exception instanceof MyUnauthorizedException) {
			router.push({ name: RouteName.UNAUTHORIZED });
		} else if (exception instanceof MyForbiddenException) {
			router.push({ name: RouteName.FORBIDDEN });
		} else if (exception instanceof MyNotFoundException) {
			router.push({ name: RouteName.NOT_FOUND });
		} else if (exception instanceof MyValidationException) {
			router.push({ name: RouteName.VALIDATION_ERROR });
		} else if (exception instanceof MyServerException) {
			router.push({ name: RouteName.SERVER_ERROR });
		} else {
			router.push({ name: RouteName.SERVER_ERROR });
		}
	}

	return { handleExceptions };
};

export default useExceptionHandler;
