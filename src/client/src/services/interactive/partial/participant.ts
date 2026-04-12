import { InteractiveBase} from '$/services/interactive/interactive-base';
import {PartialGameInteractive, PartialGameInteractiveImpl} from '$/services/interactive/partial/game';
import {ParticipantId} from '$/types/ids';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialParticipantData} from '$/services/api/participants/types/participant-data';

export interface PartialParticipantInteractive {
  readonly game: PartialGameInteractive;

  readonly id: ParticipantId;
  readonly userName: string;
}

export class PartialParticipantInteractiveImpl extends InteractiveBase implements PartialParticipantInteractive {
  public readonly game: PartialGameInteractiveImpl;

  public readonly id: ParticipantId;
  public readonly userName: string;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    game: PartialGameInteractiveImpl,
    data: PartialParticipantData
  ) {
    super(api, events);
    this.game = game;
    this.id = data.id;

    this.userName = data.userName;
  }
}
