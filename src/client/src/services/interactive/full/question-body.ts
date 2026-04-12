import {EventHandler, InteractiveBase} from '$/services/interactive/interactive-base';
import {FullQuestionInteractive, FullQuestionInteractiveImpl} from '$/services/interactive/full/question';
import {signal, Signal, WritableSignal} from '@angular/core';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullQuestionBodyData} from '$/services/api/questions/types/body/body-data';
import {
  forQuestion,
  QuestionBodyTextChangedEvent,
  QuestionEvent
} from '$/services/events/question.events';
import {Observable} from 'rxjs';
import {UpdateQuestionBodyTextRequest} from '$/services/api/questions/types/answer/update-text';

export interface FullQuestionBodyInteractive {
  readonly question: FullQuestionInteractive;

  readonly text: Signal<string>;

  updateText(text: string): Promise<void>;
}

export class FullQuestionBodyInteractiveImpl extends InteractiveBase implements FullQuestionBodyInteractive {
  public readonly question: FullQuestionInteractiveImpl;

  public readonly text: WritableSignal<string>;

  public constructor(api: ApiService, events: GameEventBus, question: FullQuestionInteractiveImpl, data: FullQuestionBodyData) {
    super(api, events);
    this.question = question;

    this.text = signal(data.text);

    this.subscribe(events.questionBodyTextChanged$, event => this.onTextChanged(event));
  }

  private subscribe<T extends QuestionEvent>($: Observable<T>, handler: EventHandler<T>): void {
    return this.subscribeCore($, handler, forQuestion(this.question.id));
  }

  public async updateText(text: string): Promise<void> {
    const request: UpdateQuestionBodyTextRequest = {
      text
    };
    await this.api.question.updateBodyText(this.question.id, request);
  }

  private onTextChanged(event: QuestionBodyTextChangedEvent): void {
    this.text.set(event.bodyText);
  }
}
