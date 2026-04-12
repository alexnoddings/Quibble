import {Component, input} from '@angular/core';
import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {PlayParticipantAnswerComponent} from '$/app/games/host/play/play-participant-answer.component';
import {FullSubmittedAnswerInteractive} from '$/services/interactive/full/submitted-answer';

@Component({
  selector: 'quibble-host-play-participant-answers',
  imports: [
    PlayParticipantAnswerComponent
  ],
  templateUrl: './play-participant-answers.component.html',
  styleUrls: ['play-participant-answers.component.css'],
})
export class PlayParticipantAnswersComponent {
  public question = input.required<FullQuestionInteractive>();

  protected get orderedAnswers() {
    return this
      .question()
      .answer
      .submittedAnswers()
      .sort(PlayParticipantAnswersComponent.sort);
  }

  private static sort(a: FullSubmittedAnswerInteractive, b: FullSubmittedAnswerInteractive) : -1 | 0 | 1 {
    const an = a.participant.userName;
    const bn = b.participant.userName;

    if (an < bn)
      return -1;

    if (an > bn)
      return 1;

    return 0;
  }
}
