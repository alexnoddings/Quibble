import {Component, input} from '@angular/core';
import {PartialGameInteractive} from '$/services/interactive/partial/game';

@Component({
  selector: 'quibble-participant-game-lobby',
  imports: [],
  templateUrl: './game-lobby.component.html',
  styleUrls: ['./game-lobby.component.css'],
})
export class ParticipantGameLobbyComponent {
  public game = input.required<PartialGameInteractive>();
}
