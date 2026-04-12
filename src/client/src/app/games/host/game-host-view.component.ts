import {Component, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {EditGameComponent} from "$/app/games/host/edit/edit-game.component";
import {GameStates} from '$/types/game-state';
import {HostGameLobbyComponent} from '$/app/games/host/lobby/game-lobby.component';
import {PlayGameComponent} from '$/app/games/host/play/play-game.component';
import {CompletedGameComponent} from '$/app/games/host/completed/completed-game.component';

@Component({
  selector: 'quibble-game-host-view',
  imports: [EditGameComponent, HostGameLobbyComponent, PlayGameComponent, CompletedGameComponent],
  templateUrl: './game-host-view.component.html',
  styleUrls: [],
})
export class GameHostViewComponent {
  game = input.required<FullGameInteractive>();
  protected readonly GameStates = GameStates;
}
