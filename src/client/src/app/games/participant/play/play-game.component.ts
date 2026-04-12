import {Component, inject, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {EditRoundComponent} from '$/app/games/host/edit/edit-round.component';
import {PublishGameComponent} from '$/app/games/host/edit/publish-game.component';
import {EditGameOverviewComponent} from '$/app/games/host/edit/edit-game-overview.component';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';
import {PartialGameInteractive} from '$/services/interactive/partial/game';
import {PlayRoundComponent} from '$/app/games/participant/play/play-round.component';
import {RoundStates} from '$/types/round-state';

@Component({
  selector: 'quibble-participant-play-game',
  imports: [PlayRoundComponent],
  templateUrl: './play-game.component.html',
  styleUrls: ['./play-game.component.css'],
})
export class PlayGameComponent {
  public game = input.required<PartialGameInteractive>();
  protected readonly RoundStates = RoundStates;
}
