import {QuestionState, QuestionStates} from '$/types/question-state';
import {FullQuestionBodyInteractive, FullQuestionBodyInteractiveImpl} from '$/services/interactive/full/question-body';
import {
  FullQuestionAnswerInteractive,
  FullQuestionAnswerInteractiveImpl
} from '$/services/interactive/full/question-answer';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {QuestionId} from '$/types/ids';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullQuestionData} from '$/services/api/questions/types/question-data';
import {FullRoundInteractive, FullRoundInteractiveImpl} from '$/services/interactive/full/round';
import {Observable, OperatorFunction} from 'rxjs';
import {
  forQuestion,
  QuestionEvent,
  QuestionOrderChangedEvent,
  QuestionPointsChangedEvent, QuestionRemovedEvent, QuestionRevealedEvent,
  QuestionStateChangedEvent
} from '$/services/events/question.events';
import {UpdateQuestionOrderRequest} from '$/services/api/questions/types/update-order';
import {UpdateQuestionStateRequest} from '$/services/api/questions/types/update-state';
import {UpdateQuestionPointsRequest} from '$/services/api/questions/types/update-points';
import {FullParticipantInteractiveImpl} from '$/services/interactive/full/participant';

export interface FullQuestionInteractive {
  readonly round: FullRoundInteractive;

  readonly id: QuestionId;
  readonly order: Signal<number>;
  readonly state: Signal<QuestionState>;
  readonly points: Signal<number>;

  readonly body: FullQuestionBodyInteractive;
  readonly answer: FullQuestionAnswerInteractive;

  updateOrder(order: number): Promise<void>;
  updateState(state: QuestionState): Promise<void>;
  updatePoints(points: number): Promise<void>;
  delete(): Promise<void>;
}

export class FullQuestionInteractiveImpl extends InteractiveBase implements FullQuestionInteractive {
  public readonly round: FullRoundInteractiveImpl;

  public readonly id: QuestionId;
  public readonly order: WritableSignal<number>;
  public readonly state: WritableSignal<QuestionState>;
  public readonly points: WritableSignal<number>;

  public readonly body: FullQuestionBodyInteractiveImpl;
  public readonly answer: FullQuestionAnswerInteractiveImpl;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    round: FullRoundInteractiveImpl,
    participants: FullParticipantInteractiveImpl[],
    data: FullQuestionData
  ) {
    super(api, events);
    this.round = round;
    this.id = data.id;

    this.order = signal(data.order);
    this.state = signal(data.state);
    this.points = signal(data.points);

    this.body = new FullQuestionBodyInteractiveImpl(api, events, this, data.body);
    this.answer = new FullQuestionAnswerInteractiveImpl(api, events, this, participants, data.answer);

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
  }

  public async updateOrder(order: number): Promise<void> {
    const request: UpdateQuestionOrderRequest = {
      order
    };
    await this.api.question.updateOrder(this.id, request);
  }

  private onOrderChanged(event: QuestionOrderChangedEvent): void {
    this.order.set(event.order);
  }

  public async updateState(state: QuestionState): Promise<void> {
    const request: UpdateQuestionStateRequest = {
      state
    };
    await this.api.question.updateState(this.id, request);
  }

  private onStateChanged(event: QuestionStateChangedEvent): void {
    this.state.set(event.state);
  }

  public async updatePoints(points: number): Promise<void> {
    const request: UpdateQuestionPointsRequest = {
      points
    };
    await this.api.question.updatePoints(this.id, request);
  }

  private onPointsChanged(event: QuestionPointsChangedEvent): void {
    this.points.set(event.points);
  }

  public async delete() : Promise<void> {
    await this.api.question.delete(this.id);
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

