import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {GameId} from '$/types/ids';
import {ApiService} from '$/services/api/api.service';

@Injectable({
  providedIn: 'root',
})
export class ApiServiceFactory {
  private readonly http = inject(HttpClient);

  get(gameId: GameId): ApiService {
    return new ApiService(this.http, gameId);
  }
}
