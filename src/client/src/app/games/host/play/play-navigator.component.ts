import {Component, inject, input} from '@angular/core';

import {FullQuestionInteractive} from '$/services/interactive/full/question';
import {PlayGameViewState} from '$/app/games/host/play/play-game-view-state';
import {PlayParticipantAnswerComponent} from '$/app/games/host/play/play-participant-answer.component';
import {OcticonArrowLeftComponent} from '$/app/icons/arrow-left';
import {OcticonArrowRightComponent} from '$/app/icons/arrow-right';
import {QuestionStateIconComponent} from '$/app/games/host/play/question-state-icon.component';
import {RoundStateIconComponent} from '$/app/games/host/play/round-state-icon.component';

@Component({
  selector: 'quibble-host-play-navigator',
  imports: [OcticonArrowLeftComponent, OcticonArrowRightComponent, QuestionStateIconComponent, RoundStateIconComponent],
  templateUrl: './play-navigator.component.html',
  styleUrls: ['play-navigator.component.css'],
})
export class PlayNavigatorComponent {
  public question = input.required<FullQuestionInteractive>();

  public viewState: PlayGameViewState = inject(PlayGameViewState);

  public canMoveToPreviousRound(): boolean {
    const question = this.question();
    const round = question.round;
    return canMoveToPrevious(round, round.game.rounds());
  }

  public canMoveToNextRound(): boolean {
    const question = this.question();
    const round = question.round;
    return canMoveToNext(round, round.game.rounds());
  }

  public moveToPreviousRound(): void {
    if (!this.canMoveToPreviousRound())
      return;

    const question = this.question();
    const round = question.round;
    const game = round.game;
    const rounds = game.rounds();

    const currentRoundIndex = rounds.indexOf(round);
    const previousRoundFirstQuestion = rounds[currentRoundIndex - 1].questions()[0];

    this.viewState.focusQuestion(previousRoundFirstQuestion);
  }

  public moveToNextRound(): void {
    if (!this.canMoveToNextRound())
      return;

    const question = this.question();
    const round = question.round;
    const game = round.game;
    const rounds = game.rounds();

    const currentRoundIndex = rounds.indexOf(round);
    const nextRoundFirstQuestion = rounds[currentRoundIndex + 1].questions()[0];

    this.viewState.focusQuestion(nextRoundFirstQuestion);
  }

  private canMoveToPreviousQuestionInRound(): boolean {
    const question = this.question();
    return canMoveToPrevious(question, question.round.questions());
  }

  public canMoveToPreviousQuestion(): boolean {
    return this.canMoveToPreviousQuestionInRound() || this.canMoveToPreviousRound();
  }

  private canMoveToNextQuestionInRound(): boolean {
    const question = this.question();
    return canMoveToNext(question, question.round.questions());
  }

  public canMoveToNextQuestion(): boolean {
    return this.canMoveToNextQuestionInRound() || this.canMoveToNextRound();
  }

  public moveToPreviousQuestion(): void {
    // Move to $round, $question-1
    if (this.canMoveToPreviousQuestionInRound()) {
      const question = this.question();
      const round = question.round;
      const questionIndex = round.questions().indexOf(question);

      this.viewState.focusQuestion(round.questions()[questionIndex - 1]);
      return;
    }

    // Move to $round-1, $lastQuestion
    if (this.canMoveToPreviousRound()) {
      const question = this.question();
      const round = question.round;
      const game = round.game;
      const rounds = game.rounds();
      const roundIndex = rounds.indexOf(round);

      const previousRoundQuestions = rounds[roundIndex - 1].questions();
      const previousQuestion = previousRoundQuestions[previousRoundQuestions.length - 1];
      this.viewState.focusQuestion(previousQuestion);
      return;
    }
  }

  public moveToNextQuestion(): void {
    // Move to $round, $question+1
    if (this.canMoveToNextQuestionInRound()) {
      const question = this.question();
      const round = question.round;
      const questionIndex = round.questions().indexOf(question);

      this.viewState.focusQuestion(round.questions()[questionIndex + 1]);
      return;
    }

    // Move to $round+1, 1
    if (this.canMoveToNextRound()) {
      const question = this.question();
      const round = question.round;
      const game = round.game;
      const rounds = game.rounds();
      const roundIndex = rounds.indexOf(round);

      const nextRoundQuestions = rounds[roundIndex + 1].questions();
      const nextQuestion = nextRoundQuestions[0];
      this.viewState.focusQuestion(nextQuestion);
      return;
    }
  }
}

function canMoveToPrevious<T>(value: T, values: T[]): boolean {
  const currentIndex = values.indexOf(value);
  return currentIndex > 0;
}

function canMoveToNext<T>(value: T, values: T[]): boolean {
  const currentIndex = values.indexOf(value);
  const valuesCount = values.length;
  return (currentIndex + 1) < valuesCount;
}
