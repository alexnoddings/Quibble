import {Component, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {EditRoundComponent} from '$/app/games/host/edit/edit-round.component';
import {GameStates} from '$/types/game-state';
import {PublishGameComponent} from '$/app/games/host/edit/publish-game.component';

@Component({
  selector: 'quibble-host-completed-game',
  imports: [],
  templateUrl: './completed-game.component.html',
  styleUrls: [],
})
export class CompletedGameComponent {
  public game = input.required<FullGameInteractive>();
}
