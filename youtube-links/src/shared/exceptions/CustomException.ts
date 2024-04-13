import { ExceptionType } from './ExceptionType';

export abstract class CustomException extends Error {
	public type: ExceptionType;

	constructor(message: string, type: ExceptionType) {
		super(message);
		this.type = type;
	}
}

export class MyValidationException extends CustomException {
	private static readonly errorMessage = 'Validation Error';
	public errors: Record<string, string[]>;

	constructor(errors: Record<string, string[]>) {
		super(MyValidationException.errorMessage, ExceptionType.Validation);
		this.errors = errors;
	}
}

export class MyServerException extends CustomException {
	private static readonly errorMessage = 'Server Error';

	constructor() {
		super(MyServerException.errorMessage, ExceptionType.Server);
	}
}

export class MyUnauthorizedException extends CustomException {
	private static readonly errorMessage = 'Unauthorized Error';

	constructor() {
		super(MyUnauthorizedException.errorMessage, ExceptionType.Unauthorized);
	}
}

export class MyForbiddenException extends CustomException {
	private static readonly errorMessage = 'Forbidden Error';

	constructor() {
		super(MyForbiddenException.errorMessage, ExceptionType.Forbidden);
	}
}

export class MyNotFoundException extends CustomException {
	private static readonly errorMessage = 'Not Found Error';

	constructor() {
		super(MyNotFoundException.errorMessage, ExceptionType.NotFound);
	}
}
