import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {signal, Signal} from '@angular/core';

export class PlayGameViewState {
  private _activeQuestion = signal<FullQuestionInteractive | undefined>(undefined);

  public get activeQuestion(): Signal<FullQuestionInteractive | undefined> {
    return this._activeQuestion;
  }

  public focusQuestion(question: FullQuestionInteractive): void {
    this._activeQuestion.set(question);
  }
}
