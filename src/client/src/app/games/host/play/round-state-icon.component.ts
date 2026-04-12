import {Component, input} from '@angular/core';

import {OcticonEyeClosedComponent} from '$/app/icons/eye-closed';
import {OcticonCheckComponent} from '$/app/icons/check';
import {FullRoundInteractive} from '$/services/interactive/full/round';
import {OcticonEyeComponent} from '$/app/icons/eye';
import {RoundStates} from '$/types/round-state';
import {QuestionStates} from '$/types/question-state';

@Component({
  selector: 'quibble-round-state-icon',
  imports: [
    OcticonEyeClosedComponent,
    OcticonEyeComponent,
    OcticonCheckComponent,
  ],
  templateUrl: './round-state-icon.component.html',
  styleUrls: [],
})
export class RoundStateIconComponent {
  public round = input.required<FullRoundInteractive>();
  protected readonly RoundStates = RoundStates;

  protected get hasCompletedRound(): boolean {
    return this.round().questions().every(q => q.state() == QuestionStates.Revealed);
  }
}
