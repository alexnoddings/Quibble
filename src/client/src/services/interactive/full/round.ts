import {RoundState} from '$/types/round-state';
import {FullQuestionInteractive, FullQuestionInteractiveImpl} from '$/services/interactive/full/question';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {ApiService} from '$/services/api/api.service';
import {signal, Signal, WritableSignal} from '@angular/core';
import {RoundId} from '$/types/ids';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullRoundData} from '$/services/api/rounds/types/data';
import {UpdateRoundTitleRequest} from '$/services/api/rounds/types/update-title';
import {UpdateRoundDescriptionRequest} from '$/services/api/rounds/types/update-description';
import {UpdateRoundOrderRequest} from '$/services/api/rounds/types/update-order';
import {
  forRound, RoundAddedEvent,
  RoundDescriptionChangedEvent, RoundEvent,
  RoundOrderChangedEvent, RoundRemovedEvent, RoundStateChangedEvent,
  RoundTitleChangedEvent
} from '$/services/events/round.events';
import {Observable} from 'rxjs';
import {FullGameInteractive, FullGameInteractiveImpl} from '$/services/interactive/full/game';
import {forQuestion, QuestionAddedEvent, QuestionEvent} from '$/services/events/question.events';
import {FullQuestionData} from '$/services/api/questions/types/question-data';
import {CreateQuestionRequest} from '$/services/api/questions/types/create';
import {FullParticipantInteractiveImpl} from '$/services/interactive/full/participant';
import {UpdateRoundStateRequest} from '$/services/api/rounds/types/update-state';

export interface FullRoundInteractive {
  readonly game: FullGameInteractive;

  readonly id: RoundId;
  readonly order: Signal<number>;
  readonly state: Signal<RoundState>;
  readonly title: Signal<string>;
  readonly description: Signal<string>;

  readonly questions: Signal<FullQuestionInteractive[]>;

  updateTitle(title: string) : Promise<void>;
  updateDescription(description: string) : Promise<void>;
  updateOrder(order: number) : Promise<void>;
  updateState(state: RoundState) : Promise<void>;
  delete() : Promise<void>;
  addQuestion(): Promise<void>;
}

export class FullRoundInteractiveImpl extends InteractiveBase implements FullRoundInteractive {
  public readonly game: FullGameInteractiveImpl;

  public readonly id: RoundId;
  public readonly order: WritableSignal<number>;
  public readonly state: WritableSignal<RoundState>;
  public readonly title: WritableSignal<string>;
  public readonly description: WritableSignal<string>;

  public readonly questions: WritableSignal<FullQuestionInteractiveImpl[]>;

  public constructor(api: ApiService, events : GameEventBus, game: FullGameInteractiveImpl, participants: FullParticipantInteractiveImpl[], data: FullRoundData) {
    super(api, events);
    this.game = game;
    this.id = data.id;

    this.order = signal(data.order);
    this.state = signal(data.state);
    this.title = signal(data.title);
    this.description = signal(data.description);

    const questions = data.questions.map(q => new FullQuestionInteractiveImpl(api, events, this, participants, q));
    this.questions = signal(questions);

    this.subscribe(events.roundTitleChanged$, event => this.onTitleChanged(event));
    this.subscribe(events.roundDescriptionChanged$, event => this.onDescriptionChanged(event));
    this.subscribe(events.roundOrderChanged$, event => this.onOrderChanged(event));
    this.subscribe(events.roundStateChanged$, event => this.onStateChanged(event));
    this.subscribe(events.roundRemoved$, event => this.onRemoved(event));
    this.subscribe(events.questionAdded$, event => this.onQuestionAdded(event));
  }

  private subscribe<T extends RoundEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forRound(this.id));
  }

  public async updateTitle(title: string) : Promise<void> {
    const request: UpdateRoundTitleRequest = {
      title
    };
    await this.api.round.updateTitle(this.id, request);
  }

  private onTitleChanged(event: RoundTitleChangedEvent): void {
    this.title.set(event.title);
  }

  public async updateDescription(description: string) : Promise<void> {
    const request: UpdateRoundDescriptionRequest = {
      description
    };
    await this.api.round.updateDescription(this.id, request);
  }

  private onDescriptionChanged(event: RoundDescriptionChangedEvent): void {
    this.description.set(event.description);
  }

  public async updateOrder(order: number) : Promise<void> {
    const request: UpdateRoundOrderRequest = {
      order
    };
    await this.api.round.updateOrder(this.id, request);
  }

  private onOrderChanged(event: RoundOrderChangedEvent): void {
    this.order.set(event.order);
  }

  public async updateState(state: RoundState) : Promise<void> {
    const request: UpdateRoundStateRequest = {
      state
    };
    await this.api.round.updateState(this.id, request);
  }

  private onStateChanged(event: RoundStateChangedEvent): void {
    this.state.set(event.state);
  }

  public async addQuestion() : Promise<void> {
    const request: CreateQuestionRequest = {
      body: `Question #${this.questions().length + 1}`,
      answer: "...",
      points: 1
    };
    await this.api.question.create(this.id, request);
  }

  private onQuestionAdded(event: QuestionAddedEvent): void {
    const data: FullQuestionData = {
      id: event.questionId,
      order: event.order,
      state: event.state,
      points: event.points,
      body: {
        text: event.bodyText
      },
      answer: {
        id: event.questionId,
        answer: event.answerText,
        // questions can only be added during draft, when there won't be any participants
        submittedAnswers: []
      }
    };
    const newQuestion = new FullQuestionInteractiveImpl(this.api, this.events, this, this.game.participants(), data);
    this.questions.update(questions => [...questions, newQuestion]);
  }

  public async delete() : Promise<void> {
    await this.api.round.delete(this.id);
  }

  private onRemoved(_: RoundRemovedEvent): void {
    this.game.rounds.update(rounds =>
      rounds.filter(r => r !== this)
    );
    this.dispose();
  }
}
