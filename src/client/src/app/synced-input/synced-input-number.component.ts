import { Component, DestroyRef, effect, inject, input, linkedSignal, output, signal } from '@angular/core';
import {SyncedInputController} from '$/app/synced-input/synced-input-controller';
import {OcticonCheckComponent} from '$/app/icons/check';

@Component({
  selector: 'quibble-synced-input-number',
  imports: [OcticonCheckComponent],
  templateUrl: './synced-input.component.html',
  styleUrl: './synced-input.component.css',
})
export class SyncedInputNumber {
  readonly id = input<string | undefined>();
  readonly placeholder = input<string | undefined>();

  readonly value = input<number>(0);
  readonly preview = output<number>();
  readonly saved = output<number>();
  protected readonly maxLength = signal<number | undefined>(undefined);

  protected readonly inputType = 'number';
  protected readonly inputMode = 'decimal';
  protected readonly currentValue = linkedSignal(() => this.formatValue(this.value()));
  protected readonly saveIconVisible = signal(false);

  private readonly controller = new SyncedInputController<number>({
    initialValue: this.value(),
    emitPreview: (value) => this.preview.emit(value),
    emitSaved: (value) => {
      this.currentValue.set(this.formatValue(value));
      this.saved.emit(value);
    },
    setSavedVisible: (visible) => this.saveIconVisible.set(visible),
  });

  constructor() {
    const destroyRef = inject(DestroyRef);

    effect(() => {
      this.controller.syncFromExternal(this.value());
    });

    destroyRef.onDestroy(() => this.controller.destroy());
  }

  protected onInput(event: Event): void {
    const rawValue = (event.target as HTMLInputElement).value;
    this.currentValue.set(rawValue);

    const parsedValue = this.parseValue(rawValue);

    if (parsedValue === null) {
      this.controller.clearPending();
      return;
    }

    this.controller.handleValidInput(parsedValue);
  }

  protected onEnter(event: Event): void {
    event.preventDefault();
    this.tryCommitCurrentValue();
  }

  protected onClipboard(event: Event): void {
    this.tryCommitCurrentValue();
  }

  protected onBlur(): void {
    const parsedValue = this.parseValue(this.currentValue());

    if (parsedValue === null) {
      this.controller.clearPending();
      this.currentValue.set(this.formatValue(this.value()));
      return;
    }

    this.currentValue.set(this.formatValue(parsedValue));
    this.controller.tryCommit(parsedValue);
  }

  private tryCommitCurrentValue(): void {
    const parsedValue = this.parseValue(this.currentValue());

    if (parsedValue === null) {
      this.controller.clearPending();
      return;
    }

    this.currentValue.set(this.formatValue(parsedValue));
    this.controller.tryCommit(parsedValue);
  }

  private parseValue(rawValue: string): number | null {
    const trimmedValue = rawValue.trim();

    if (trimmedValue === '') {
      return null;
    }

    if (!/^(?:\d+\.?\d*|\.\d+)$/.test(trimmedValue)) {
      return null;
    }

    return this.roundToTwoPlaces(Number(trimmedValue));
  }

  private formatValue(value: number): string {
    return this.roundToTwoPlaces(value).toString();
  }

  private roundToTwoPlaces(value: number): number {
    return Number(value.toFixed(2));
  }
}
