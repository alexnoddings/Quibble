import {GameState} from '$/types/game-state';
import {GameId} from '$/types/ids';
import {OperatorFunction} from 'rxjs';

export interface GameEvent {
}

export interface GameTitleChangedEvent extends GameEvent {
  readonly title: string;
}

export interface GameStateChangedEvent extends GameEvent {
  readonly state: GameState;
}

export function forGame<T extends GameEvent>(id: GameId): OperatorFunction<T, T> {
  return $ => $;
}
