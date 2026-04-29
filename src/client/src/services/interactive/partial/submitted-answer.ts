import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {Signal, signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialQuestionInteractive, PartialQuestionInteractiveImpl} from '$/services/interactive/partial/question';
import {PartialSubmittedAnswerData} from '$/services/api/participants/types/submitted-answer-data';
import {Observable} from 'rxjs';
import {
  AnswerEvent,
  AnswerPointsChangedEvent,
  AnswerTextChangedEvent,
  forParticipantAnswer
} from '$/services/events/answer.events';
import {
  PreviewQuestionSubmittedAnswerTextRequest,
  UpdateQuestionSubmittedAnswerTextRequest
} from '$/services/api/answers/types/answer/update-text';

export interface PartialSubmittedAnswerInteractive {
  readonly question: PartialQuestionInteractive;

  readonly points: Signal<number | undefined>;
  readonly answer: Signal<string>;

  updateAnswer(answer: string): Promise<void>
  previewAnswer(answer: string): Promise<void>
}

export class PartialSubmittedAnswerInteractiveImpl extends InteractiveBase implements PartialSubmittedAnswerInteractive {
  public readonly question: PartialQuestionInteractiveImpl;

  public readonly points: WritableSignal<number | undefined>;
  public readonly answer: WritableSignal<string>;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    question: PartialQuestionInteractiveImpl,
    data: PartialSubmittedAnswerData
  ) {
    super(api, events);

    this.question = question;

    this.points = signal(data.points);
    this.answer = signal(data.answer);

    this.subscribe(events.answerTextChanged$, event => this.onTextChanged(event));
    this.subscribe(events.answerPointsChanged$, event => this.onPointsChanged(event));
  }

  private subscribe<T extends AnswerEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forParticipantAnswer(this.question.id));
  }

  public async updateAnswer(answer: string): Promise<void> {
    const request: UpdateQuestionSubmittedAnswerTextRequest = {
      answer: answer,
    };
    await this.api.answer.updateAnswer(this.question.id, request);
  }

  public async previewAnswer(answer: string): Promise<void> {
    const request: PreviewQuestionSubmittedAnswerTextRequest = {
      answer: answer,
    };
    await this.api.answer.previewAnswer(this.question.id, request);
  }

  private onTextChanged(event: AnswerTextChangedEvent): void {
    this.answer.set(event.answer);
  }

  private onPointsChanged(event: AnswerPointsChangedEvent): void {
    this.points.set(event.points);
  }
}

