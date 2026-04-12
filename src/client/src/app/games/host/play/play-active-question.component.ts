import {Component, input} from '@angular/core';
import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {RoundStates} from '$/types/round-state';
import {QuestionStates} from '$/types/question-state';
import {humanise} from '$/app/games/humanise';

@Component({
  selector: 'quibble-host-play-active-question',
  imports: [],
  templateUrl: './play-active-question.component.html',
  styleUrls: ['play-active-question.component.css'],
})
export class PlayActiveQuestionComponent {
  public question = input.required<FullQuestionInteractive>();

  protected unmarkedAnswerCount(): number {
    return this.question().answer.submittedAnswers().filter(a => a.points() == undefined).length;
  }

  protected readonly RoundStates = RoundStates;
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
