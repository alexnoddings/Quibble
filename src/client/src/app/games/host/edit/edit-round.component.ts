import {Component, inject, input} from '@angular/core';

import {FullRoundInteractive} from '$/services/interactive/full/round';
import {EditQuestionComponent} from '$/app/games/host/edit/edit-question.component';
import {IdGenerator} from '$/app/games/id-generator';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';
import {OcticonBinComponent} from '$/app/icons/bin';

@Component({
  selector: 'quibble-host-edit-round',
  imports: [EditQuestionComponent, SyncedInputText, OcticonBinComponent],
  templateUrl: './edit-round.component.html',
  styleUrls: ['./edit-round.component.css'],
})
export class EditRoundComponent {
  public round = input.required<FullRoundInteractive>();
  public number = input.required<number>();

  protected updateTitle(newTitle: string): void {
    this.round().updateTitle(newTitle);
  }

  protected addQuestion(): void {
    this.round().addQuestion();
  }

  protected delete(): void {
    this.round().delete();
  }

  protected readonly IdGenerator = IdGenerator;
}
