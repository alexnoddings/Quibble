import {GameState} from '$/types/game-state';
import {Signal, signal, WritableSignal} from '@angular/core';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {ApiService} from '$/services/api/api.service';
import {PartialRoundInteractive, PartialRoundInteractiveImpl} from '$/services/interactive/partial/round';
import {GameId} from '$/types/ids';
import {RoundAddedEvent} from '$/services/events/round.events';
import {forGame, GameEvent, GameStateChangedEvent, GameTitleChangedEvent} from '$/services/events/game.events';
import {PartialGameData} from '$/services/api/games/types/data';
import {PartialRoundData} from '$/services/api/rounds/types/data';
import {Observable} from 'rxjs';
import {PartialParticipantInteractive, PartialParticipantInteractiveImpl} from '$/services/interactive/partial/participant';
import {ParticipantAddedEvent} from '$/services/events/participant.events';
import {PartialParticipantData} from '$/services/api/participants/types/participant-data';

export interface PartialGameInteractive {
  readonly perspective: 'Participant';

  readonly id: GameId;
  readonly slug: string;
  readonly ownerName: string;
  readonly state: Signal<GameState>;
  readonly title: Signal<string>;

  readonly participants: Signal<PartialParticipantInteractive[]>;
  readonly rounds: Signal<PartialRoundInteractive[]>;
}

export class PartialGameInteractiveImpl extends InteractiveBase implements PartialGameInteractive {
  public readonly perspective = 'Participant';

  public readonly id: GameId;
  public readonly slug: string;
  public readonly ownerName: string;
  public readonly state: WritableSignal<GameState>;
  public readonly title: WritableSignal<string>;

  public readonly participants: WritableSignal<PartialParticipantInteractiveImpl[]>;
  public readonly rounds: WritableSignal<PartialRoundInteractiveImpl[]>;

  public constructor(api: ApiService, events : GameEventBus, data: PartialGameData) {
    super(api, events);
    this.id = data.id;

    this.slug = data.slug;
    this.ownerName = data.ownerName;
    this.state = signal(data.state);
    this.title = signal(data.title);

    const participants: PartialParticipantInteractiveImpl[] = data.participants.map(p => new PartialParticipantInteractiveImpl(api, events, this, p));
    this.participants = signal(participants);

    const rounds = data.rounds.map(r => new PartialRoundInteractiveImpl(api, events, this, r));
    this.rounds = signal(rounds);

    this.subscribe(events.gameStateChanged$, event => this.onStateChanged(event));
    this.subscribe(events.gameTitleChanged$, event => this.onTitleChanged(event));
    this.subscribe(events.roundAdded$, event => this.onRoundAdded(event));
    this.subscribe(events.participantAdded$, event => this.onParticipantAdded(event));
  }

  private subscribe<T extends GameEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forGame(this.id));
  }

  private onStateChanged(event: GameStateChangedEvent): void {
    this.state.set(event.state);
  }

  private onTitleChanged(event: GameTitleChangedEvent): void {
    this.title.set(event.title);
  }

  private onRoundAdded(event: RoundAddedEvent): void {
    const data: PartialRoundData = {
      id: event.roundId,
      order: event.order,
      state: event.state,
      title: event.title,
      description: event.description,
      // newly added rounds won't have any questions yet
      questions: []
    };
    const newRound = new PartialRoundInteractiveImpl(this.api, this.events, this, data);
    this.rounds.update(rounds => [...rounds, newRound]);
  }

  private onParticipantAdded(event: ParticipantAddedEvent): void {
    const data: PartialParticipantData = {
      id: event.participantId,
      userName: event.name
    };
    const newParticipant = new PartialParticipantInteractiveImpl(this.api, this.events, this, data);
    this.participants.update(participants => [...participants, newParticipant]);
  }
}
