import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {Signal, signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullParticipantInteractive, FullParticipantInteractiveImpl} from '$/services/interactive/full/participant';
import {FullQuestionInteractive, FullQuestionInteractiveImpl} from '$/services/interactive/full/question';
import {FullSubmittedAnswerData} from '$/services/api/participants/types/submitted-answer-data';
import {UpdateQuestionPointsRequest} from '$/services/api/questions/types/update-points';
import {QuestionPointsChangedEvent} from '$/services/events/question.events';
import {Observable} from 'rxjs';
import {
  AnswerEvent,
  AnswerPointsChangedEvent,
  AnswerTextChangedEvent, AnswerTextPreviewedEvent,
  forAnswer
} from '$/services/events/answer.events';

export interface FullSubmittedAnswerInteractive {
  readonly participant: FullParticipantInteractive;
  readonly question: FullQuestionInteractive;

  readonly points: Signal<number | undefined>;
  readonly answer: Signal<string>;

  updatePoints(points: number): Promise<void>
}

export class FullSubmittedAnswerInteractiveImpl extends InteractiveBase implements FullSubmittedAnswerInteractive {
  public readonly participant: FullParticipantInteractiveImpl;
  public readonly question: FullQuestionInteractiveImpl;

  public readonly points: WritableSignal<number | undefined>;
  public readonly answer: WritableSignal<string>;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    question: FullQuestionInteractiveImpl,
    participants: FullParticipantInteractiveImpl[],
    data: FullSubmittedAnswerData
  ) {
    super(api, events);

    this.participant = participants.find(p => p.id == data.participantId)!;
    this.question = question;

    this.points = signal(data.points);
    this.answer = signal(data.answer);

    this.subscribe(events.answerTextChanged$, event => this.onAnswerTextChanged(event));
    this.subscribe(events.answerTextPreviewed$, event => this.onAnswerTextPreviewed(event));
    this.subscribe(events.answerPointsChanged$, event => this.onPointsChanged(event));
  }

  private subscribe<T extends AnswerEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forAnswer(this.participant.id, this.question.id));
  }

  private onAnswerTextChanged(event: AnswerTextChangedEvent): void {
    this.answer.set(event.answer);
  }

  private onAnswerTextPreviewed(event: AnswerTextPreviewedEvent): void {
    this.answer.set(event.answer);
  }

  public async updatePoints(points: number): Promise<void> {
    const request: UpdateQuestionPointsRequest = {
      points
    };
    await this.api.answer.updatePoints(this.participant.id, this.question.id, request);
  }

  private onPointsChanged(event: AnswerPointsChangedEvent): void {
    this.points.set(event.points);
  }
}

