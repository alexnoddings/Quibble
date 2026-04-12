import {Component, inject, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {EditRoundComponent} from '$/app/games/host/edit/edit-round.component';
import {PublishGameComponent} from '$/app/games/host/edit/publish-game.component';
import {EditGameOverviewComponent} from '$/app/games/host/edit/edit-game-overview.component';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';

@Component({
  selector: 'quibble-host-edit-game',
  imports: [EditRoundComponent, PublishGameComponent, EditGameOverviewComponent, SyncedInputText],
  templateUrl: './edit-game.component.html',
  styleUrls: ['./edit-game.component.css'],
})
export class EditGameComponent {
  public game = input.required<FullGameInteractive>();

  protected updateTitle(newTitle: string): void {
    this.game().updateTitle(newTitle);
  }
}
