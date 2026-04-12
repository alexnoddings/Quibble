import {Observable, OperatorFunction, Subscription} from "rxjs";
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';

export class InteractiveBase {
  private readonly subscriptions = new Subscription;
  protected readonly api: ApiService;
  protected readonly events : GameEventBus;

  constructor(api: ApiService, events : GameEventBus) {
    this.api = api;
    this.events = events;
  }

  protected subscribeCore<T>($: Observable<T>, handler: EventHandler<T>, pipe: OperatorFunction<T,T>): void {
    const subscription = $
      .pipe(pipe)
      .subscribe(handler);
    this.subscriptions.add(subscription);
  }

  public dispose(): void {
    this.subscriptions.unsubscribe();
  }
}

export type EventHandler<T> = (value: T) => void;
