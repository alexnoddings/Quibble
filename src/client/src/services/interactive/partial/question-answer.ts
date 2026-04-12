import {PartialSubmittedAnswerInteractive,PartialSubmittedAnswerInteractiveImpl} from '$/services/interactive/partial/submitted-answer';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {PartialQuestionInteractive, PartialQuestionInteractiveImpl} from '$/services/interactive/partial/question';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialQuestionAnswerData} from '$/services/api/questions/types/answer/answer-data';
import {forQuestion,QuestionAnswerTextChangedEvent,QuestionEvent} from '$/services/events/question.events';
import {Observable} from 'rxjs';

export interface PartialQuestionAnswerInteractive {
  readonly question: PartialQuestionInteractive;

  readonly answer: Signal<string>;
  readonly submittedAnswer: PartialSubmittedAnswerInteractive;
}

export class PartialQuestionAnswerInteractiveImpl extends InteractiveBase implements PartialQuestionAnswerInteractive {
  public readonly question: PartialQuestionInteractiveImpl;

  public readonly answer: WritableSignal<string>;
  public readonly submittedAnswer: PartialSubmittedAnswerInteractiveImpl;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    question: PartialQuestionInteractiveImpl,
    data: PartialQuestionAnswerData
  ) {
    super(api, events);
    this.question = question;

    this.answer = signal(data.answer);

    this.submittedAnswer = new PartialSubmittedAnswerInteractiveImpl(api, events, question, data.submittedAnswer);

    this.subscribe(events.questionAnswerTextChanged$, event => this.onAnswerChanged(event));
  }

  private subscribe<T extends QuestionEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forQuestion(this.question.id));
  }

  private onAnswerChanged(event: QuestionAnswerTextChangedEvent): void {
    this.answer.set(event.answerText);
  }
}
