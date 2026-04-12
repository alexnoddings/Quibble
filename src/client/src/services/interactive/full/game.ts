import {GameState} from '$/types/game-state';
import {Signal, signal, WritableSignal} from '@angular/core';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {ApiService} from '$/services/api/api.service';
import {FullRoundInteractive, FullRoundInteractiveImpl} from '$/services/interactive/full/round';
import {GameId} from '$/types/ids';
import {RoundAddedEvent, RoundEvent} from '$/services/events/round.events';
import {forGame, GameEvent, GameStateChangedEvent, GameTitleChangedEvent} from '$/services/events/game.events';
import {UpdateGameTitleRequest} from '$/services/api/games/types/update-title';
import {UpdateGameStateRequest} from '$/services/api/games/types/update-state';
import {CreateRoundRequest} from '$/services/api/rounds/types/create';
import {FullGameData} from '$/services/api/games/types/data';
import {FullRoundData} from '$/services/api/rounds/types/data';
import {Observable} from 'rxjs';
import {FullParticipantInteractive, FullParticipantInteractiveImpl} from '$/services/interactive/full/participant';
import {ParticipantAddedEvent} from '$/services/events/participant.events';
import {FullParticipantData} from '$/services/api/participants/types/participant-data';
import {FullSubmittedAnswerInteractiveImpl} from '$/services/interactive/full/submitted-answer';

export interface FullGameInteractive {
  readonly perspective: 'Host';

  readonly id: GameId;
  readonly slug: string;
  readonly state: Signal<GameState>;
  readonly title: Signal<string>;

  readonly participants: Signal<FullParticipantInteractive[]>;
  readonly rounds: Signal<FullRoundInteractive[]>;

  updateState(newState: GameState): Promise<void>;
  updateTitle(newTitle: string): Promise<void>;
  delete() : Promise<void>;
  addRound(): Promise<void>;
}

export class FullGameInteractiveImpl extends InteractiveBase implements FullGameInteractive {
  public readonly perspective = 'Host';

  public readonly id: GameId;
  public readonly slug: string;
  public readonly state: WritableSignal<GameState>;
  public readonly title: WritableSignal<string>;

  public readonly participants: WritableSignal<FullParticipantInteractiveImpl[]>;
  public readonly rounds: WritableSignal<FullRoundInteractiveImpl[]>;

  public constructor(api: ApiService, events : GameEventBus, data: FullGameData) {
    super(api, events);
    this.id = data.id;

    this.slug = data.slug;
    this.state = signal(data.state);
    this.title = signal(data.title);

    const participants: FullParticipantInteractiveImpl[] = data.participants.map(p => new FullParticipantInteractiveImpl(api, events, this, p));
    this.participants = signal(participants);

    const rounds = data.rounds.map(r => new FullRoundInteractiveImpl(api, events, this, participants, r));
    this.rounds = signal(rounds);

    this.subscribe(events.gameStateChanged$, event => this.onStateChanged(event));
    this.subscribe(events.gameTitleChanged$, event => this.onTitleChanged(event));
    this.subscribe(events.roundAdded$, event => this.onRoundAdded(event));
    this.subscribe(events.participantAdded$, event => this.onParticipantAdded(event));
  }

  private subscribe<T extends GameEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forGame(this.id));
  }

  public async updateState(newState: GameState) : Promise<void> {
    const request: UpdateGameStateRequest = {
      state: newState,
    };
    await this.api.game.updateState(request);
  }

  private onStateChanged(event: GameStateChangedEvent): void {
    this.state.set(event.state);
  }

  public async updateTitle(newTitle: string) : Promise<void> {
    const request: UpdateGameTitleRequest = {
      title: newTitle,
    };
    await this.api.game.updateTitle(request);
  }

  private onTitleChanged(event: GameTitleChangedEvent): void {
    this.title.set(event.title);
  }

  public async delete(): Promise<void> {
    await this.api.game.delete();
  }

  public async addRound() : Promise<void> {
    const request: CreateRoundRequest = {
      title: `Round #${this.rounds().length + 1}`
    };
    await this.api.round.create(request);
  }

  private onRoundAdded(event: RoundAddedEvent): void {
    const data: FullRoundData = {
      id: event.roundId,
      order: event.order,
      state: event.state,
      title: event.title,
      description: event.description,
      // newly added rounds won't have any questions yet
      questions: []
    };
    const newRound = new FullRoundInteractiveImpl(this.api, this.events, this, this.participants(), data);
    this.rounds.update(rounds => [...rounds, newRound]);
  }

  private onParticipantAdded(event: ParticipantAddedEvent): void {
    const data: FullParticipantData = {
      id: event.participantId,
      userName: event.name,
      answers: event.answers.map(a => ({
        participantId: event.participantId,
        questionId: a.questionId,
        points: undefined,
        answer: a.answer
      }))
    };
    const newParticipant = new FullParticipantInteractiveImpl(this.api, this.events, this, data);
    this.participants.update(participants => [...participants, newParticipant]);

    const questions = this.rounds().flatMap(r => r.questions());
    for (const answer of data.answers) {
      const question = questions.find(q => q.id == answer.questionId);
      if (!question) {
        console.log("Could not find question id", answer.questionId);
        continue;
      }
      const participants = this.participants();
      const submittedAnswer = new FullSubmittedAnswerInteractiveImpl(this.api, this.events, question, participants, answer);
      question.answer.submittedAnswers.update(answers => [...answers, submittedAnswer]);
    }
  }
}
