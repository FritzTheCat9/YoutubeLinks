import {
  MyForbiddenException,
  MyNotFoundException,
  MyServerException,
  MyUnauthorizedException,
  MyValidationException,
} from '@/shared/exceptions/CustomException';
import type {
  ErrorResponse,
  ForbiddenErrorResponse,
  NotFoundErrorResponse,
  ServerErrorResponse,
  UnauthorizedErrorResponse,
  ValidationErrorResponse,
} from '@/shared/exceptions/ErrorResponse';
import { ExceptionType } from '@/shared/exceptions/ExceptionType';
import type { AxiosInstance, AxiosResponse } from 'axios';
import axios, { AxiosError } from 'axios';

export interface IApiClient {
  getReturnAxiosResponse(url: string): Promise<AxiosResponse>;
  get<TResponse>(url: string): Promise<TResponse>;
  postWithoutResponse<TRequest>(url: string, tRequest: TRequest): Promise<void>;
  post<TRequest, TResponse>(url: string, tRequest: TRequest): Promise<TResponse>;
  postReturnAxiosResponse<TRequest>(url: string, tRequest: TRequest): Promise<AxiosResponse>;
  put<TRequest>(url: string, tRequest: TRequest): Promise<void>;
  putWithoutResponse(url: string): Promise<void>;
  delete(url: string): Promise<void>;
}

export const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));

export interface ApiOptions {
  baseUrl: string;
  authScheme: string;
  languageHeader: string;
}

export const ApiOptions: ApiOptions = {
  baseUrl: 'http://localhost:5000/',
  authScheme: 'Bearer',
  languageHeader: 'Accept-Language',
};

export class ApiClient implements IApiClient {
  private readonly baseUrl: string = ApiOptions.baseUrl;
  private readonly client: AxiosInstance;
  private readonly authScheme: string = ApiOptions.authScheme;
  private readonly languageHeader: string = ApiOptions.languageHeader;

  constructor() {
    this.client = axios.create({
      baseURL: this.baseUrl,
    });
  }

  async getReturnAxiosResponse(url: string): Promise<AxiosResponse> {
    try {
      const response = this.client.get(`${this.baseUrl}${url}`);
      return response;
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async get<TResponse>(url: string): Promise<TResponse> {
    try {
      const response = await this.client.get<TResponse>(`${this.baseUrl}${url}`);
      return response.data;
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async postWithoutResponse<TRequest>(url: string, tRequest: TRequest): Promise<void> {
    try {
      await this.client.post(`${this.baseUrl}${url}`, tRequest);
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async post<TRequest, TResponse>(url: string, tRequest: TRequest): Promise<TResponse> {
    try {
      const response = await this.client.post(`${this.baseUrl}${url}`, tRequest);
      return response.data;
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async postReturnAxiosResponse<TRequest>(url: string, tRequest: TRequest): Promise<AxiosResponse> {
    try {
      return await this.client.post(`${this.baseUrl}${url}`, tRequest);
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async put<TRequest>(url: string, tRequest: TRequest): Promise<void> {
    try {
      await this.client.put<TRequest>(`${this.baseUrl}${url}`, tRequest);
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async putWithoutResponse(url: string): Promise<void> {
    try {
      await this.client.put(`${this.baseUrl}${url}`);
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  async delete(url: string): Promise<void> {
    try {
      await this.client.delete(`${this.baseUrl}${url}`);
    } catch (error) {
      this.HandleErrors(error as AxiosError);
      return Promise.reject();
    }
  }

  HandleErrors(error: AxiosError) {
    let apiError = error.response?.data as ErrorResponse;

    switch (apiError.type) {
      case ExceptionType.Validation:
        const validationErrorResponse = apiError as ValidationErrorResponse;
        throw new MyValidationException(validationErrorResponse.errors);
      case ExceptionType.Unauthorized:
        const unauthorizedErrorResponse = apiError as UnauthorizedErrorResponse;
        throw new MyUnauthorizedException();
      case ExceptionType.Forbidden:
        const forbiddenErrorResponse = apiError as ForbiddenErrorResponse;
        throw new MyForbiddenException();
      case ExceptionType.NotFound:
        const notFoundErrorResponse = apiError as NotFoundErrorResponse;
        throw new MyNotFoundException();
      case ExceptionType.Server:
      default:
        const serverErrorResponse = apiError as ServerErrorResponse;
        throw new MyServerException();
    }
  }
}

export const apiClient = new ApiClient();
