import {Component, input} from '@angular/core';
import {PartialGameInteractive} from '$/services/interactive/partial/game';
import {ParticipantGameLobbyComponent} from '$/app/games/participant/lobby/game-lobby.component';
import {GameStates} from '$/types/game-state';
import {PlayGameComponent} from '$/app/games/participant/play/play-game.component';

@Component({
  selector: 'quibble-game-participant-view',
  imports: [
    ParticipantGameLobbyComponent,
    PlayGameComponent
  ],
  templateUrl: './game-participant-view.component.html',
  styleUrls: [],
})
export class GameParticipantViewComponent {
  game = input.required<PartialGameInteractive>();
  protected readonly GameStates = GameStates;
}
