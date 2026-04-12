import {Component, ElementRef, input, viewChild} from '@angular/core';

import {FullSubmittedAnswerInteractive} from '$/services/interactive/full/submitted-answer';
import {OcticonCheckCircleComponent} from '$/app/icons/check-circle';
import {OcticonXCircleComponent} from '$/app/icons/x-circle';
import {OcticonCheckCircleFilledComponent} from '$/app/icons/check-circle-filled';
import {OcticonXCircleFilledComponent} from '$/app/icons/x-circle-filled';
import {QuestionStates} from '$/types/question-state';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputNumber} from '$/app/synced-input/synced-input-number.component';
import {OcticonSkipFillComponent} from '$/app/icons/skip-fill';
import {OcticonSparklesComponent} from '$/app/icons/sparkles';
import {OcticonSkipComponent} from '$/app/icons/skip';
import {humanise} from '$/app/games/humanise';

@Component({
  selector: 'quibble-host-play-participant-answer',
  imports: [
    OcticonCheckCircleComponent,
    OcticonXCircleComponent,
    OcticonCheckCircleFilledComponent,
    OcticonXCircleFilledComponent,
    SyncedInputNumber,
    OcticonSkipFillComponent,
    OcticonSparklesComponent,
    OcticonSkipComponent
  ],
  templateUrl: './play-participant-answer.component.html',
  styleUrls: ['play-participant-answer.component.css'],
})
export class PlayParticipantAnswerComponent {
  public answer = input.required<FullSubmittedAnswerInteractive>();

  public getAnswerVariant(score: number | undefined, outOf: number) : string {
    if (score == undefined) {
      return "answer--unmarked";
    }

    if (score == 0) {
      return "answer--no-points";
    }

    if (score < outOf) {
      return "answer--partial-points";
    }

    if (score == outOf) {
      return "answer--full-points"
    }

    return "answer--extra-points";
  }

  public setPoints(points: number) {
    this.answer().updatePoints(points);
  }

  pointsDialog = viewChild<ElementRef<HTMLDialogElement>>("pointsDialog");
  openPointsDialogButton = viewChild<ElementRef<HTMLButtonElement>>("openPointsDialogButton");

  protected openPointsDialog(): void {
    const button = this.openPointsDialogButton()?.nativeElement;
    if (!button)
      return;

    const dialog = this.pointsDialog()?.nativeElement;
    if (!dialog)
      return;

    const l = button.offsetLeft + (button.offsetWidth / 2);
    const t = button.offsetTop;

    dialog.style = `left: ${l}px; top: ${t}px;`;
    dialog.showPopover();
  }

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

  protected readonly QuestionStates = QuestionStates;
  protected readonly IdGenerator = IdGenerator;
  protected readonly prettyPrint = humanise;
}
