import {Component, inject, input} from '@angular/core';

import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {PlayGameViewState} from '$/app/games/host/play/play-game-view-state';
import {PlayParticipantAnswerComponent} from '$/app/games/host/play/play-participant-answer.component';
import {OcticonArrowLeftComponent} from '$/app/icons/arrow-left';
import {OcticonArrowRightComponent} from '$/app/icons/arrow-right';
import {OcticonEyeClosedComponent} from '$/app/icons/eye-closed';
import {OcticonPencilComponent} from '$/app/icons/pencil';
import {OcticonSearchComponent} from '$/app/icons/search';
import {OcticonSearchCheckComponent} from '$/app/icons/search-check';
import {OcticonCheckComponent} from '$/app/icons/check';
import {OcticonCommentDiscussionComponent} from '$/app/icons/comment-discussion';
import {QuestionStates} from '$/types/question-state';

@Component({
  selector: 'quibble-question-state-icon',
  imports: [
    OcticonEyeClosedComponent,
    OcticonSearchComponent,
    OcticonSearchCheckComponent,
    OcticonCheckComponent,
    OcticonCommentDiscussionComponent
  ],
  templateUrl: './question-state-icon.component.html',
  styleUrls: [],
})
export class QuestionStateIconComponent {
  public question = input.required<FullQuestionInteractive>();
  protected readonly QuestionStates = QuestionStates;

  protected get hasMarkedEveryAnswer(): boolean {
    return this.question().answer.submittedAnswers().every(a => a.points() != undefined);
  }
}
