import {HttpClient} from '@angular/common/http';
import {BaseEntityService} from '$/services/api/base.service';
import {GameId} from '$/types/ids';
import {Result} from '$/services/api/result';
import {UpdateGameStateRequest} from '$/services/api/games/types/update-state';
import {UpdateGameTitleRequest} from '$/services/api/games/types/update-title';
import {GameData} from '$/services/api/games/types/data';

export class GameService extends BaseEntityService {
  private readonly gameId: GameId;

  constructor(http: HttpClient, gameId: GameId) {
    super(http);
    this.gameId = gameId;
  }

  async getData(): Promise<Result<GameData>> {
    const url = this.getUrl(`/games/${this.gameId}`);
    const http = this.http.get<GameData>(url);
    return await this.send(http);
  }

  async updateState(request: UpdateGameStateRequest): Promise<Result<void>> {
    const url = this.getUrl(`/games/${this.gameId}/state`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateTitle(request: UpdateGameTitleRequest): Promise<Result<void>> {
    const url = this.getUrl(`/games/${this.gameId}/title`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async delete(): Promise<Result<void>> {
    const url = this.getUrl(`/games/${this.gameId}`);
    const http = this.http.delete<void>(url);
    return await this.send(http);
  }
}
