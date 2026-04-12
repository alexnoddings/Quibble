import {Component, inject, input} from '@angular/core';

import {FullRoundInteractive} from '$/services/interactive/full/round';
import {EditQuestionComponent} from '$/app/games/host/edit/edit-question.component';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';
import {OcticonBinComponent} from '$/app/icons/bin';
import {PartialRoundInteractive} from '$/services/interactive/partial/round';
import {PlayQuestionComponent} from '$/app/games/participant/play/play-question.component';
import {QuestionStates} from '$/types/question-state';

@Component({
  selector: 'quibble-participant-play-round',
  imports: [
    PlayQuestionComponent
  ],
  templateUrl: './play-round.component.html',
  styleUrls: ['./play-round.component.css'],
})
export class PlayRoundComponent {
  public round = input.required<PartialRoundInteractive>();

  protected readonly IdGenerator = IdGenerator;
  protected readonly QuestionStates = QuestionStates;
}
