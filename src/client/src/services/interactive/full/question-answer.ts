import {
  FullSubmittedAnswerInteractive,
  FullSubmittedAnswerInteractiveImpl
} from '$/services/interactive/full/submitted-answer';
import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {FullQuestionInteractive, FullQuestionInteractiveImpl} from '$/services/interactive/full/question';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullQuestionAnswerData} from '$/services/api/questions/types/answer/answer-data';
import {
  forQuestion,
  QuestionAnswerTextChangedEvent,
  QuestionEvent
} from '$/services/events/question.events';
import {Observable} from 'rxjs';
import {UpdateQuestionAnswerTextRequest} from '$/services/api/questions/types/body/update-text';
import {FullParticipantInteractiveImpl} from '$/services/interactive/full/participant';

export interface FullQuestionAnswerInteractive {
  readonly question: FullQuestionInteractive;

  readonly answer: Signal<string>;
  readonly submittedAnswers: Signal<FullSubmittedAnswerInteractive[]>;

  updateAnswer(answer: string): Promise<void>;
}

export class FullQuestionAnswerInteractiveImpl extends InteractiveBase implements FullQuestionAnswerInteractive {
  public readonly id: string;
  public readonly question: FullQuestionInteractiveImpl;

  public readonly answer: WritableSignal<string>;
  public readonly submittedAnswers: WritableSignal<FullSubmittedAnswerInteractive[]>;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    question: FullQuestionInteractiveImpl,
    participants: FullParticipantInteractiveImpl[],
    data: FullQuestionAnswerData
  ) {
    super(api, events);
    this.id = data.id;
    this.question = question;

    this.answer = signal(data.answer);

    const submittedAnswers = data.submittedAnswers.map(sa => new FullSubmittedAnswerInteractiveImpl(api, events, question, participants, sa));
    this.submittedAnswers = signal(submittedAnswers);

    this.subscribe(events.questionAnswerTextChanged$, event => this.onAnswerChanged(event));
  }

  private subscribe<T extends QuestionEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forQuestion(this.question.id));
  }

  public async updateAnswer(answer: string): Promise<void> {
    const request: UpdateQuestionAnswerTextRequest = {
      answer: answer
    };
    await this.api.question.updateAnswerText(this.question.id, request);
  }

  private onAnswerChanged(event: QuestionAnswerTextChangedEvent): void {
    this.answer.set(event.answerText);
  }
}
