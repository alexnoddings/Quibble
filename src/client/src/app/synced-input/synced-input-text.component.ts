import {
  Component,
  DestroyRef,
  effect,
  inject,
  input,
  linkedSignal,
  output,
  signal,
} from '@angular/core';
import {SyncedInputController} from '$/app/synced-input/synced-input-controller';
import {OcticonCheckComponent} from '$/app/icons/check';

@Component({
  selector: 'quibble-synced-input-text',
  imports: [OcticonCheckComponent],
  templateUrl: './synced-input.component.html',
  styleUrl: './synced-input.component.css',
})
export class SyncedInputText {
  readonly id = input<string | undefined>();
  readonly placeholder = input<string | undefined>();

  readonly value = input<string>('');
  readonly preview = output<string>();
  readonly saved = output<string>();
  readonly maxLength = input<number | undefined>(undefined);

  protected readonly inputType = 'text';
  protected readonly inputMode: string | null = null;
  protected readonly currentValue = linkedSignal(() => this.value());
  protected readonly saveIconVisible = signal(false);

  private readonly controller = new SyncedInputController<string>({
    initialValue: this.value(),
    emitPreview: (value) => this.preview.emit(value),
    emitSaved: (value) => {
      this.currentValue.set(value);
      this.saved.emit(value);
    },
    setSavedVisible: (visible) => this.saveIconVisible.set(visible),
    timing: {
      previewThrottleMs: 500
    }
  });

  constructor() {
    const destroyRef = inject(DestroyRef);

    effect(() => {
      this.controller.syncFromExternal(this.value());
    });

    destroyRef.onDestroy(() => this.controller.destroy());
  }

  protected onInput(event: Event): void {
    const val = (event.target as HTMLInputElement).value;
    this.currentValue.set(val);
    this.controller.handleValidInput(val);
  }

  protected onEnter(event: Event): void {
    event.preventDefault();
    this.controller.tryCommit(this.currentValue());
  }

  protected onClipboard(event: Event): void {
    this.controller.tryCommit(this.currentValue());
  }

  protected onBlur(): void {
    this.controller.tryCommit(this.currentValue());
  }
}
