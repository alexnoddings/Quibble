import {Component, input} from '@angular/core';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';
import {PartialQuestionInteractive} from '$/services/interactive/partial/question';
import {QuestionStates} from '$/types/question-state';
import {OcticonCheckCircleComponent} from '$/app/icons/check-circle';
import {OcticonXCircleComponent} from '$/app/icons/x-circle';
import {OcticonDotCircleComponent} from '$/app/icons/dot-circle';
import {OcticonSparklesComponent} from '$/app/icons/sparkles';
import {OcticonLockComponent} from '$/app/icons/lock';
import {humanise} from '$/app/games/humanise';

@Component({
  selector: 'quibble-participant-play-question',
  imports: [SyncedInputText, OcticonCheckCircleComponent, OcticonXCircleComponent, OcticonDotCircleComponent, OcticonSparklesComponent, OcticonLockComponent],
  templateUrl: './play-question.component.html',
  styleUrls: ['play-question.component.css'],
})
export class PlayQuestionComponent {
  public question = input.required<PartialQuestionInteractive>();

  protected updateAnswer(answer: string): void {
    this.question().answer.submittedAnswer.updateAnswer(answer);
  }

  protected previewAnswer(answer: string): void {
    this.question().answer.submittedAnswer.previewAnswer(answer);
  }

  protected readonly IdGenerator = IdGenerator;
  protected readonly QuestionStates = QuestionStates;

  protected prettyPoints(points: number) {
    const floor = Math.floor(points);
    if (floor == points) {
      return points.toString();
    }

    const decimal = points - floor;
    let decimalStr;
    if (decimal == 0.25)
      decimalStr = "¼";
    if (decimal == 0.5)
      decimalStr = "½";
    if (decimal == 0.75)
      decimalStr = "¾";

    if (floor == 0)
      return decimalStr;

    return `${floor}${decimalStr}`;
  }

  protected readonly prettyPrint = humanise;
}
