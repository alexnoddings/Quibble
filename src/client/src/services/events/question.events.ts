import {QuestionId, RoundId} from '$/types/ids';
import {QuestionState} from '$/types/question-state';
import {Observable, OperatorFunction} from 'rxjs';
import {filter} from 'rxjs/operators';
import {RoundEvent} from '$/services/events/round.events';

export interface QuestionEvent {
  readonly questionId: QuestionId;
}

export interface QuestionAddedEvent extends RoundEvent, QuestionEvent {
  readonly order: number;
  readonly state: QuestionState;
  readonly points: number;

  readonly bodyText: string;
  readonly answerText: string;
}

export interface QuestionRevealedEvent extends QuestionEvent {
  readonly bodyText: string;
  readonly answerText: string;
}

export interface QuestionRemovedEvent extends QuestionEvent {
}

export interface QuestionOrderChangedEvent extends QuestionEvent {
  readonly order: number;
}

export interface QuestionStateChangedEvent extends QuestionEvent {
  readonly state: QuestionState;
}

export interface QuestionPointsChangedEvent extends QuestionEvent {
  readonly points: number;
}

export interface QuestionBodyTextChangedEvent extends QuestionEvent {
  readonly bodyText: string;
}

export interface QuestionAnswerTextChangedEvent extends QuestionEvent {
  readonly answerText: string;
}

function questionGuard<T extends QuestionEvent>(id: QuestionId, $: Observable<T>): Observable<T> {
  return $.pipe(filter(x => x.questionId == id))
}

export function forQuestion<T extends QuestionEvent>(id: QuestionId): OperatorFunction<T, T> {
  return $ => questionGuard(id, $);
}
