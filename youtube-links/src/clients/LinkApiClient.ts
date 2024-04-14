import type { PagedList } from '@/shared/abstractions/PagedList';
import type { CreateLink } from '@/shared/features/links/commands/CreateLink';
import type { DownloadLink } from '@/shared/features/links/commands/DownloadLink';
import type { DownloadSingleLink } from '@/shared/features/links/commands/DownloadSingleLink';
import type { UpdateLink } from '@/shared/features/links/commands/UpdateLink';
import type { GetAllLinks } from '@/shared/features/links/queries/GetAllLinks';
import type { GetAllPaginatedLinks } from '@/shared/features/links/queries/GetAllPaginatedLinks';
import type { LinkDto } from '@/shared/features/links/responses/LinkDto';
import { apiClient, type IApiClient } from './ApiClient';
import type { AxiosResponse } from 'axios';

interface ILinkApiClient {
  getAllPaginatedLinks(query: GetAllPaginatedLinks.Query): Promise<PagedList<LinkDto>>;
  getAllLinks(query: GetAllLinks.Query): Promise<GetAllLinks.LinkInfoDto[]>;
  getLink(id: number): Promise<LinkDto>;
  createLink(command: CreateLink.Command): Promise<void>;
  updateLink(command: UpdateLink.Command): Promise<void>;
  deleteLink(id: number): Promise<void>;
  downloadLink(command: DownloadLink.Command): Promise<AxiosResponse>;
  downloadSingleLink(command: DownloadSingleLink.Command): Promise<AxiosResponse>;
}

class LinkApiClient implements ILinkApiClient {
  private readonly apiClient: IApiClient;
  private readonly url: string = 'api/links';

  constructor() {
    this.apiClient = apiClient;
  }

  async getAllPaginatedLinks(query: GetAllPaginatedLinks.Query): Promise<PagedList<LinkDto>> {
    return await this.apiClient.post<GetAllPaginatedLinks.Query, PagedList<LinkDto>>(
      `${this.url}/allPaginated`,
      query
    );
  }

  async getAllLinks(query: GetAllLinks.Query): Promise<GetAllLinks.LinkInfoDto[]> {
    return await this.apiClient.post<GetAllLinks.Query, GetAllLinks.LinkInfoDto[]>(
      `${this.url}/all`,
      query
    );
  }

  async getLink(id: number): Promise<LinkDto> {
    return await this.apiClient.get<LinkDto>(`${this.url}/${id}`);
  }

  async createLink(command: CreateLink.Command): Promise<void> {
    await this.apiClient.postWithoutResponse(this.url, command);
  }

  async updateLink(command: UpdateLink.Command): Promise<void> {
    await this.apiClient.put(`${this.url}/${command.id}`, command);
  }

  async deleteLink(id: number): Promise<void> {
    await this.apiClient.delete(`${this.url}/${id}`);
  }

  async downloadLink(command: DownloadLink.Command): Promise<AxiosResponse> {
    return await this.apiClient.postReturnAxiosResponse(`${this.url}/download`, command);
  }

  async downloadSingleLink(command: DownloadSingleLink.Command): Promise<AxiosResponse> {
    return await this.apiClient.postReturnAxiosResponse(`${this.url}/downloadSingle`, command);
  }
}

export const linkApiClient = new LinkApiClient();
