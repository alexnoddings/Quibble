import {ParticipantId, QuestionId} from '$/types/ids';
import {Observable, OperatorFunction} from 'rxjs';
import {filter} from 'rxjs/operators';

export interface AnswerEvent {
  readonly participantId: ParticipantId;
  readonly questionId: QuestionId;
}

export interface AnswerTextChangedEvent extends AnswerEvent {
  readonly answer: string;
}

export interface AnswerTextPreviewedEvent extends AnswerEvent {
  readonly answer: string;
}

export interface AnswerPointsChangedEvent extends AnswerEvent {
  readonly points: number;
}

function answerGuard<T extends AnswerEvent>(participantId: ParticipantId, questionId: QuestionId, $: Observable<T>): Observable<T> {
  return $.pipe(filter(x => x.participantId == participantId && x.questionId == questionId));
}

export function forAnswer<T extends AnswerEvent>(participantId: ParticipantId, questionId: QuestionId): OperatorFunction<T, T> {
  return $ => answerGuard(participantId, questionId, $);
}

function participantAnswerGuard<T extends AnswerEvent>(questionId: QuestionId, $: Observable<T>): Observable<T> {
  return $.pipe(filter(x => x.questionId == questionId));
}

export function forParticipantAnswer<T extends AnswerEvent>(questionId: QuestionId): OperatorFunction<T, T> {
  return $ => participantAnswerGuard(questionId, $);
}
