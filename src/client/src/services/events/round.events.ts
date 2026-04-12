import {RoundId} from '$/types/ids';
import {RoundState} from '$/types/round-state';
import {Observable, OperatorFunction} from 'rxjs';
import {filter} from 'rxjs/operators';

export interface RoundEvent {
  readonly roundId: RoundId;
}

export interface RoundAddedEvent extends RoundEvent {
  readonly order: number;
  readonly state: RoundState;
  readonly title: string;
  readonly description: string;
}

export interface RoundRemovedEvent extends RoundEvent {
}

export interface RoundOrderChangedEvent extends RoundEvent {
  readonly order: number;
}

export interface RoundStateChangedEvent extends RoundEvent {
  readonly state: RoundState;
}

export interface RoundTitleChangedEvent extends RoundEvent {
  readonly title: string;
}

export interface RoundDescriptionChangedEvent extends RoundEvent {
  readonly description: string;
}

function roundGuard<T extends RoundEvent>(id: RoundId, $: Observable<T>): Observable<T> {
  return $.pipe(filter(x => x.roundId == id))
}

export function forRound<T extends RoundEvent>(id: RoundId): OperatorFunction<T, T> {
  return $ => roundGuard(id, $);
}
