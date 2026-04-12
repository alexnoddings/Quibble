import {GameState} from '$/types/game-state';
import {GameId} from '$/types/ids';
import {GameSlug} from '$/types/game-slug';
import {GameRelationship} from '$/types/game-relationship';

export interface UserGameInfo {
  id: GameId;
  slug: GameSlug;
  state: GameState;
  title: string;
}

export interface ParticipantGameInfo {
  id: GameId;
  slug: GameSlug;
  ownerName: string;
  state: GameState;
  title: string;
}

export interface GameInfo {
  id: GameId;
  state: GameState;
  relationship: GameRelationship;
}
