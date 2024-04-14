import type { PagedList } from '@/shared/abstractions/PagedList';
import type { CreatePlaylist } from '@/shared/features/playlists/commands/CreatePlaylist';
import type { ExportPlaylist } from '@/shared/features/playlists/commands/ExportPlaylist';
import type { ImportPlaylist } from '@/shared/features/playlists/commands/ImportPlaylist';
import type { ResetLinksDownloadedFlag } from '@/shared/features/playlists/commands/ResetLinksDownloadedFlag';
import type { UpdatePlaylist } from '@/shared/features/playlists/commands/UpdatePlaylist';
import type { GetAllPublicPlaylists } from '@/shared/features/playlists/queries/GetAllPublicPlaylists';
import type { GetAllUserPlaylists } from '@/shared/features/playlists/queries/GetAllUserPlaylists';
import type { PlaylistDto } from '@/shared/features/playlists/responses/PlaylistDto';
import { apiClient, type IApiClient } from './ApiClient';
import type { AxiosResponse } from 'axios';

interface IPlaylistApiClient {
  getAllUserPlaylists(query: GetAllUserPlaylists.Query): Promise<PagedList<PlaylistDto>>;
  getAllPublicPlaylists(query: GetAllPublicPlaylists.Query): Promise<PagedList<PlaylistDto>>;
  getPlaylist(id: number): Promise<PlaylistDto>;
  createPlaylist(command: CreatePlaylist.Command): Promise<void>;
  updatePlaylist(command: UpdatePlaylist.Command): Promise<void>;
  deletePlaylist(id: number): Promise<void>;
  exportPlaylist(command: ExportPlaylist.Command): Promise<AxiosResponse>;
  importPlaylistFromJson(command: ImportPlaylist.Command): Promise<void>;
  resetLinksDownloadedFlag(command: ResetLinksDownloadedFlag.Command): Promise<void>;
}

class PlaylistApiClient implements IPlaylistApiClient {
  private readonly apiClient: IApiClient;
  private readonly url: string = 'api/playlists';

  constructor() {
    this.apiClient = apiClient;
  }

  async getAllUserPlaylists(query: GetAllUserPlaylists.Query): Promise<PagedList<PlaylistDto>> {
    return await this.apiClient.post<GetAllUserPlaylists.Query, PagedList<PlaylistDto>>(
      `${this.url}/all`,
      query
    );
  }

  async getAllPublicPlaylists(query: GetAllPublicPlaylists.Query): Promise<PagedList<PlaylistDto>> {
    return await this.apiClient.post<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>(
      `${this.url}/allPublic`,
      query
    );
  }

  async getPlaylist(id: number): Promise<PlaylistDto> {
    return await this.apiClient.get<PlaylistDto>(`${this.url}/${id}`);
  }

  async createPlaylist(command: CreatePlaylist.Command): Promise<void> {
    await this.apiClient.postWithoutResponse(this.url, command);
  }

  async updatePlaylist(command: UpdatePlaylist.Command): Promise<void> {
    await this.apiClient.put(`${this.url}/${command.id}`, command);
  }

  async deletePlaylist(id: number): Promise<void> {
    await this.apiClient.delete(`${this.url}/${id}`);
  }

  async exportPlaylist(command: ExportPlaylist.Command): Promise<AxiosResponse> {
    return await this.apiClient.postReturnAxiosResponse(`${this.url}/export`, command);
  }

  async importPlaylistFromJson(command: ImportPlaylist.Command): Promise<void> {
    await this.apiClient.postWithoutResponse(`${this.url}/import`, command);
  }

  async resetLinksDownloadedFlag(command: ResetLinksDownloadedFlag.Command): Promise<void> {
    await this.apiClient.postWithoutResponse(`${this.url}/resetDownloadedFlag`, command);
  }
}

export const playlistApiClient = new PlaylistApiClient();
