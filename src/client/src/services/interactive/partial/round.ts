import {RoundState} from '$/types/round-state';
import {PartialQuestionInteractive, PartialQuestionInteractiveImpl} from '$/services/interactive/partial/question';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {ApiService} from '$/services/api/api.service';
import {signal, Signal, WritableSignal} from '@angular/core';
import {RoundId} from '$/types/ids';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialRoundData} from '$/services/api/rounds/types/data';
import {
  forRound, RoundDescriptionChangedEvent, RoundEvent,
  RoundOrderChangedEvent, RoundRemovedEvent, RoundStateChangedEvent, RoundTitleChangedEvent
} from '$/services/events/round.events';
import {Observable} from 'rxjs';
import {PartialGameInteractive, PartialGameInteractiveImpl} from '$/services/interactive/partial/game';
import {QuestionAddedEvent} from '$/services/events/question.events';
import {PartialQuestionData} from '$/services/api/questions/types/question-data';
import {PartialParticipantInteractiveImpl} from '$/services/interactive/partial/participant';

export interface PartialRoundInteractive {
  readonly game: PartialGameInteractive;

  readonly id: RoundId;
  readonly order: Signal<number>;
  readonly state: Signal<RoundState>;
  readonly title: Signal<string>;
  readonly description: Signal<string>;

  readonly questions: Signal<PartialQuestionInteractive[]>;
}

export class PartialRoundInteractiveImpl extends InteractiveBase implements PartialRoundInteractive {
  public readonly game: PartialGameInteractiveImpl;

  public readonly id: RoundId;
  public readonly order: WritableSignal<number>;
  public readonly state: WritableSignal<RoundState>;
  public readonly title: WritableSignal<string>;
  public readonly description: WritableSignal<string>;

  public readonly questions: WritableSignal<PartialQuestionInteractiveImpl[]>;

  public constructor(
    api: ApiService,
    events : GameEventBus,
    game: PartialGameInteractiveImpl,
    data: PartialRoundData
  ) {
    super(api, events);
    this.game = game;
    this.id = data.id;

    this.order = signal(data.order);
    this.state = signal(data.state);
    this.title = signal(data.title);
    this.description = signal(data.description);

    const questions = data.questions.map(q => new PartialQuestionInteractiveImpl(api, events, this, q));
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

  private onTitleChanged(event: RoundTitleChangedEvent): void {
    this.title.set(event.title);
  }

  private onDescriptionChanged(event: RoundDescriptionChangedEvent): void {
    this.description.set(event.description);
  }

  private onOrderChanged(event: RoundOrderChangedEvent): void {
    this.order.set(event.order);
  }

  private onStateChanged(event: RoundStateChangedEvent): void {
    this.state.set(event.state);
  }

  private onQuestionAdded(event: QuestionAddedEvent): void {
    const data: PartialQuestionData = {
      id: event.questionId,
      order: event.order,
      state: event.state,
      points: event.points,
      body: {
        text: event.bodyText
      },
      answer: {
        answer: event.answerText,
        submittedAnswer: {
          questionId: event.questionId,
          points: undefined,
          answer: '',
        }
      }
    };
    const newQuestion = new PartialQuestionInteractiveImpl(this.api, this.events, this, data);
    this.questions.update(questions => [...questions, newQuestion]);
  }

  private onRemoved(_: RoundRemovedEvent): void {
    this.game.rounds.update(rounds =>
      rounds.filter(r => r !== this)
    );
    this.dispose();
  }
}
