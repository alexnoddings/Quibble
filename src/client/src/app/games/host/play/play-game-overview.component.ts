import {Component, inject, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {PlayGameViewState} from '$/app/games/host/play/play-game-view-state';
import {RoundStateIconComponent} from '$/app/games/host/play/round-state-icon.component';
import {QuestionStateIconComponent} from '$/app/games/host/play/question-state-icon.component';

@Component({
  selector: 'quibble-host-play-game-overview',
  imports: [
    RoundStateIconComponent,
    QuestionStateIconComponent
  ],
  templateUrl: './play-game-overview.component.html',
  styleUrls: ['./play-game-overview.component.css'],
})
export class PlayGameOverviewComponent {
  public game = input.required<FullGameInteractive>();

  public viewState: PlayGameViewState = inject(PlayGameViewState);
}
