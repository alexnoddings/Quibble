import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, Observable} from 'rxjs';
import {UserGameInfo, GameInfo, ParticipantGameInfo} from '$/services/api/games/types/get-info';
import {GameId} from '$/types/ids';
import {GameSlug} from '$/types/game-slug';

@Injectable({
  providedIn: 'root',
})
export class GamesService {
  private readonly http = inject(HttpClient);

  public getOwnedGames(): Observable<UserGameInfo[]> {
    return this.http.get<UserGameInfo[]>('/api/games/owned');
  }

  public getParticipatingGames(): Observable<ParticipantGameInfo[]> {
    return this.http.get<ParticipantGameInfo[]>('/api/games/participating');
  }

  public getGameInfo(slug: string): Observable<GameInfo> {
    return this.http.get<GameInfo>(`/api/games/info/${slug}`);
  }

  public async joinGame(id: GameSlug): Promise<void> {
    const $ = this.http.post<void>(`/api/games/${id}/join`, null);
    await firstValueFrom($);
  }

  public createGame(title: string): Observable<CreateGameResponse> {
    const request = {
      title
    };
    return this.http.post<CreateGameResponse>(`/api/games/`, request);
  }
}

export interface CreateGameResponse {
  id: GameId,
  slug: string,
  title: string
}
