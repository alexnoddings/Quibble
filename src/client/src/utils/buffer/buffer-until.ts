import {noop, Observable, ObservableInput, OperatorFunction, Subscriber} from 'rxjs';
import {operate} from 'rxjs/internal/util/lift';
import {createOperatorSubscriber} from 'rxjs/internal/operators/OperatorSubscriber';
import {innerFrom} from 'rxjs/internal/observable/innerFrom';

export function bufferUntil<T>(closingNotifier: ObservableInput<any>): OperatorFunction<T, T[]> {
  return operate((source: Observable<T>, subscriber: Subscriber<T[]>) => {
    // The current buffer, or null if done buffering
    let currentBuffer: T[]|null = [];

    // Subscribe to our source
    source.subscribe(
      createOperatorSubscriber(
        subscriber,
        (value: T) => {
          if (currentBuffer == null) {
            // If there is no buffer, then emit the value immediately
            subscriber.next([value]);
          }
          else {
            // Otherwise, buffer the value for later
            currentBuffer.push(value);
          }
        },
        () => {
          if (currentBuffer != null) {
            // If there is a buffer, emit it and complete
            subscriber.next(currentBuffer);
            subscriber.complete();
          }
        }
      )
    );

    // Subscribe to the closing notifier
    innerFrom(closingNotifier).subscribe(
      createOperatorSubscriber(
        subscriber,
        () => {
          // Clone the current buffer, clear it, then emit the clone
          const b = currentBuffer;
          currentBuffer = null;
          if (b != null) {
            subscriber.next(b);
          }
        },
        noop
      )
    );

    return () => {
      // Ensure buffered values are released on finalization.
      currentBuffer = null!;
    };
  });
}
