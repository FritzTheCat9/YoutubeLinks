import type { AxiosInstance, AxiosResponse } from 'axios';
import axios from 'axios';

export interface IApiClient {
	get(url: string): Promise<AxiosResponse>;
	getWithResponse<TResponse>(url: string): Promise<TResponse>;
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

	async get(url: string): Promise<AxiosResponse> {
		return await this.client.get(`${this.baseUrl}${url}`);
	}

	async getWithResponse<TResponse>(url: string): Promise<TResponse> {
		const response = await this.client.get<TResponse>(`${this.baseUrl}${url}`);
		return response.data;
	}

	async postWithoutResponse<TRequest>(url: string, tRequest: TRequest): Promise<void> {
		await this.client.post(`${this.baseUrl}${url}`, tRequest);
	}

	async post<TRequest, TResponse>(url: string, tRequest: TRequest): Promise<TResponse> {
		const response = await this.client.post(`${this.baseUrl}${url}`, tRequest);
		return response.data;
	}

	async postReturnAxiosResponse<TRequest>(url: string, tRequest: TRequest): Promise<AxiosResponse> {
		return await this.client.post(`${this.baseUrl}${url}`, tRequest);
	}

	async put<TRequest>(url: string, tRequest: TRequest): Promise<void> {
		await this.client.put<TRequest>(`${this.baseUrl}${url}`, tRequest);
	}

	async putWithoutResponse(url: string): Promise<void> {
		await this.client.put(`${this.baseUrl}${url}`);
	}

	async delete(url: string): Promise<void> {
		await this.client.delete(`${this.baseUrl}${url}`);
	}
}

export const apiClient = new ApiClient();
