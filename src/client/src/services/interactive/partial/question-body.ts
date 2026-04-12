import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {PartialQuestionInteractive, PartialQuestionInteractiveImpl} from '$/services/interactive/partial/question';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {PartialQuestionBodyData} from '$/services/api/questions/types/body/body-data';
import {forQuestion,QuestionBodyTextChangedEvent,QuestionEvent} from '$/services/events/question.events';
import {Observable} from 'rxjs';

export interface PartialQuestionBodyInteractive {
  readonly question: PartialQuestionInteractive;

  readonly text: Signal<string>;
}

export class PartialQuestionBodyInteractiveImpl extends InteractiveBase implements PartialQuestionBodyInteractive {
  public readonly question: PartialQuestionInteractiveImpl;

  public readonly text: WritableSignal<string>;

  public constructor(api: ApiService, events: GameEventBus, question: PartialQuestionInteractiveImpl, data: PartialQuestionBodyData) {
    super(api, events);
    this.question = question;

    this.text = signal(data.text);

    this.subscribe(events.questionBodyTextChanged$, event => this.onTextChanged(event));
  }

  private subscribe<T extends QuestionEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forQuestion(this.question.id));
  }

  private onTextChanged(event: QuestionBodyTextChangedEvent): void {
    this.text.set(event.bodyText);
  }
}
