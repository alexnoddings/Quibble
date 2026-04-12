import {Component, inject, input} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {IdGenerator} from '$/app/games/id-generator';

@Component({
  selector: 'quibble-host-edit-game-overview',
  imports: [],
  templateUrl: './edit-game-overview.component.html',
  styleUrls: ['./edit-game-overview.component.css'],
})
export class EditGameOverviewComponent {
  public game = input.required<FullGameInteractive>();

  protected addRound(): void {
    this.game().addRound();
  }

  protected getRoundCountLabel(): string {
    const c = this.game().rounds().length;
    return this.pluralise("round", c);
  }

  protected getQuestionCountLabel(): string {
    const c = this.game().rounds().reduce((c, r) => c + r.questions().length, 0);
    return this.pluralise("question", c);
  }

  private pluralise(text: string, number: number) {
    if (number === 0)
      return `No ${text}s`;

    if (number === 1)
      return `1 ${text}`;

    return `${number} ${text}s`;
  }
}
