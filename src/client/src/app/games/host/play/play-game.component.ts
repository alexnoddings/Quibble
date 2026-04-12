import {Component, inject, input, OnInit} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {PlayGameOverviewComponent} from '$/app/games/host/play/play-game-overview.component';
import {PlayGameViewState} from '$/app/games/host/play/play-game-view-state';
import {PlayActiveQuestionComponent} from '$/app/games/host/play/play-active-question.component';
import {PlayGameParticipantsComponent} from '$/app/games/host/play/play-game-participants.component';
import {PlayNavigatorComponent} from '$/app/games/host/play/play-navigator.component';
import {PlayParticipantAnswersComponent} from '$/app/games/host/play/play-participant-answers.component';

@Component({
  selector: 'quibble-host-play-game',
  providers: [PlayGameViewState],
  imports: [PlayGameOverviewComponent, PlayActiveQuestionComponent, PlayGameParticipantsComponent, PlayNavigatorComponent, PlayParticipantAnswersComponent],
  templateUrl: './play-game.component.html',
  styleUrls: ['./play-game.component.css'],
})
export class PlayGameComponent implements OnInit {
  public game = input.required<FullGameInteractive>();

  public viewState: PlayGameViewState = inject(PlayGameViewState);

  public ngOnInit(): void {
    const game = this.game();
    const firstRound = game.rounds()[0];
    const firstQuestion = firstRound.questions()[0];
    this.viewState.focusQuestion(firstQuestion);
  }
}
