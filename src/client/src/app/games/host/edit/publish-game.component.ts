import {Component, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {EditRoundComponent} from '$/app/games/host/edit/edit-round.component';
import {GameStates} from '$/types/game-state';

@Component({
  selector: 'quibble-host-publish-game',
  imports: [],
  templateUrl: './publish-game.component.html',
  styleUrls: [],
})
export class PublishGameComponent {
  public game = input.required<FullGameInteractive>();

  public async openGame(): Promise<void> {
    await this.game().updateState(GameStates.Open);
  }

  protected readonly GameStates = GameStates;
}
