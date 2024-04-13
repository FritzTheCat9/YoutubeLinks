import router from '@/router';
import {
	MyForbiddenException,
	MyNotFoundException,
	MyServerException,
	MyUnauthorizedException,
	MyValidationException,
} from '@/shared/exceptions/CustomException';

const useExceptionHandler = () => {
	function handleExceptions(exception: Error): void {
		if (exception instanceof MyUnauthorizedException) {
			router.push('/error/unauthorized');
		} else if (exception instanceof MyForbiddenException) {
			router.push('/error/forbidden');
		} else if (exception instanceof MyNotFoundException) {
			router.push('/error/notfound');
		} else if (exception instanceof MyValidationException) {
			router.push('/error/validation');
		} else if (exception instanceof MyServerException) {
			router.push('/error/server');
		} else {
			router.push('/error/server');
		}
	}

	return { handleExceptions };
};

export default useExceptionHandler;
