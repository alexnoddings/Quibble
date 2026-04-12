import {ParticipantId, RoundId} from '$/types/ids';
import {Observable, OperatorFunction} from 'rxjs';
import {filter} from 'rxjs/operators';
import {QuestionEvent} from '$/services/events/question.events';
import {RoundEvent} from '$/services/events/round.events';

export interface ParticipantEvent {
  readonly participantId: ParticipantId;
}

export interface ParticipantAddedAnswer {
  readonly questionId: string;
  readonly answer: string;
}

export interface ParticipantAddedEvent extends ParticipantEvent {
  readonly name: string;
  readonly answers: ParticipantAddedAnswer[];
}

function participantGuard<T extends ParticipantEvent>(id: ParticipantId, $: Observable<T>): Observable<T> {
  return $.pipe(filter(x => x.participantId == id))
}

export function forParticipant<T extends ParticipantEvent>(id: ParticipantId): OperatorFunction<T, T> {
  return $ => participantGuard(id, $);
}
