export namespace UpdateLink {
  export interface Command {
    id: number;
    url: string;
    title: string;
    downloaded: boolean;
  }
}
