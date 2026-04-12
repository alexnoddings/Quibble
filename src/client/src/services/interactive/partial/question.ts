import {QuestionState, QuestionStates} from '$/types/question-state';
import {PartialQuestionBodyInteractive, PartialQuestionBodyInteractiveImpl} from '$/services/interactive/partial/question-body';
import {PartialQuestionAnswerInteractive,PartialQuestionAnswerInteractiveImpl} from '$/services/interactive/partial/question-answer';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {QuestionId} from '$/types/ids';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialQuestionData} from '$/services/api/questions/types/question-data';
import {PartialRoundInteractive, PartialRoundInteractiveImpl} from '$/services/interactive/partial/round';
import {Observable} from 'rxjs';
import {
  forQuestion,
  QuestionEvent,
  QuestionOrderChangedEvent,
  QuestionPointsChangedEvent, QuestionRemovedEvent, QuestionRevealedEvent,
  QuestionStateChangedEvent
} from '$/services/events/question.events';

export interface PartialQuestionInteractive {
  readonly round: PartialRoundInteractive;

  readonly id: QuestionId;
  readonly order: Signal<number>;
  readonly state: Signal<QuestionState>;
  readonly points: Signal<number>;

  readonly body: PartialQuestionBodyInteractive;
  readonly answer: PartialQuestionAnswerInteractive;
}

export class PartialQuestionInteractiveImpl extends InteractiveBase implements PartialQuestionInteractive {
  public readonly round: PartialRoundInteractiveImpl;

  public readonly id: QuestionId;
  public readonly order: WritableSignal<number>;
  public readonly state: WritableSignal<QuestionState>;
  public readonly points: WritableSignal<number>;

  public readonly body: PartialQuestionBodyInteractiveImpl;
  public readonly answer: PartialQuestionAnswerInteractiveImpl;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    round: PartialRoundInteractiveImpl,
    data: PartialQuestionData
  ) {
    super(api, events);
    this.round = round;
    this.id = data.id;

    this.order = signal(data.order);
    this.state = signal(data.state);
    this.points = signal(data.points);

    this.body = new PartialQuestionBodyInteractiveImpl(api, events, this, data.body);
    this.answer = new PartialQuestionAnswerInteractiveImpl(api, events, this, data.answer);

    this.subscribe(events.questionRevealed$, event => this.onRevealed(event));
    this.subscribe(events.questionOrderChanged$, event => this.onOrderChanged(event));
    this.subscribe(events.questionStateChanged$, event => this.onStateChanged(event));
    this.subscribe(events.questionPointsChanged$, event => this.onPointsChanged(event));
    this.subscribe(events.questionRemoved$, event => this.onRemoved(event));
  }

  private subscribe<T extends QuestionEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forQuestion(this.id));
  }

  private onRevealed(event: QuestionRevealedEvent): void {
    this.state.set(QuestionStates.InProgress);
    this.body.text.set(event.bodyText);
    this.answer.answer.set(event.answerText);
  }

  private onOrderChanged(event: QuestionOrderChangedEvent): void {
    this.order.set(event.order);
  }

  private onStateChanged(event: QuestionStateChangedEvent): void {
    this.state.set(event.state);
  }

  private onPointsChanged(event: QuestionPointsChangedEvent): void {
    this.points.set(event.points);
  }

  private onRemoved(_: QuestionRemovedEvent): void {
    this.round.questions.update(questions =>
      questions.filter(q => q !== this)
    );
    this.body.dispose();
    this.answer.dispose();
    this.dispose();
  }
}

