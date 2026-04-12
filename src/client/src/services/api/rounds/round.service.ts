import {HttpClient} from '@angular/common/http';
import {BaseEntityService} from '$/services/api/base.service';
import {CreateRoundRequest, CreateRoundResponse} from '$/services/api/rounds/types/create';
import {Result} from '$/services/api/result';
import {UpdateRoundTitleRequest} from '$/services/api/rounds/types/update-title';
import {GameId, RoundId} from '$/types/ids';
import {UpdateRoundDescriptionRequest} from '$/services/api/rounds/types/update-description';
import {UpdateRoundOrderRequest} from '$/services/api/rounds/types/update-order';
import {UpdateRoundStateRequest} from '$/services/api/rounds/types/update-state';

export class RoundService extends BaseEntityService {
  private readonly gameId: GameId;

  constructor(http: HttpClient, gameId: GameId) {
    super(http);
    this.gameId = gameId;
  }

  async create(request: CreateRoundRequest) : Promise<Result<CreateRoundResponse>> {
    const url = this.getUrl(`/games/${this.gameId}/rounds`);
    const http = this.http.post<CreateRoundResponse>(url, request);
    return await this.send(http);
  }

  async updateTitle(roundId: RoundId, request: UpdateRoundTitleRequest): Promise<Result<void>> {
    const url = this.getUrl(`/rounds/${roundId}/title`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateDescription(roundId: RoundId, request: UpdateRoundDescriptionRequest): Promise<Result<void>> {
    const url = this.getUrl(`/rounds/${roundId}/description`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateOrder(roundId: RoundId, request: UpdateRoundOrderRequest): Promise<Result<void>> {
    const url = this.getUrl(`/rounds/${roundId}/order`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateState(roundId: RoundId, request: UpdateRoundStateRequest): Promise<Result<void>> {
    const url = this.getUrl(`/rounds/${roundId}/state`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async delete(roundId: RoundId): Promise<Result<void>> {
    const url = this.getUrl(`/rounds/${roundId}`);
    const http = this.http.delete<void>(url);
    return await this.send(http);
  }
}
