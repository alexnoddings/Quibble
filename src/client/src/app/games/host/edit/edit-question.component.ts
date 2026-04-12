import {Component, input} from '@angular/core';
import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';
import {SyncedInputNumber} from '$/app/synced-input/synced-input-number.component';
import {OcticonBinComponent} from '$/app/icons/bin';

@Component({
  selector: 'quibble-host-edit-question',
  imports: [
    SyncedInputText,
    SyncedInputNumber,
    OcticonBinComponent
  ],
  templateUrl: './edit-question.component.html',
  styleUrls: ['edit-question.component.css'],
})
export class EditQuestionComponent {
  public question = input.required<FullQuestionInteractive>();
  public number = input.required<number>();

  protected updateBody(body: string): void {
    this.question().body.updateText(body);
  }

  protected updateAnswer(answer: string): void {
    this.question().answer.updateAnswer(answer);
  }

  protected updatePoints(points: number): void {
    this.question().updatePoints(points);
  }

  protected delete(): void {
    this.question().delete();
  }

  protected readonly IdGenerator = IdGenerator;
}
