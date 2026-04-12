const DEFAULT_PREVIEW_THROTTLE_MS = 1000;
const DEFAULT_SAVE_DEBOUNCE_MS = 1500;
const DEFAULT_SAVED_ICON_DURATION_MS = 2000;

type ControllerConfig<T> = {
  initialValue: T;
  emitPreview: (value: T) => void;
  emitSaved: (value: T) => void;
  setSavedVisible: (visible: boolean) => void;
  areEqual?: (left: T, right: T) => boolean;
  timing?: {
    previewThrottleMs?: number;
    saveDebounceMs?: number;
    savedIconDurationMs?: number;
  }
};

export class SyncedInputController<T> {
  private lastCommittedValue: T;
  private latestPreviewValue: T | null = null;
  private readonly previewTimer = new ManagedTimer();
  private readonly debounceTimer = new ManagedTimer();
  private readonly savedIconTimer = new ManagedTimer();
  private readonly areEqual: (left: T, right: T) => boolean;

  constructor(private readonly config: ControllerConfig<T>) {
    this.lastCommittedValue = config.initialValue;
    this.areEqual = config.areEqual ?? Object.is;
  }

  syncFromExternal(value: T): void {
    this.lastCommittedValue = value;
    this.clearPending();
  }

  handleValidInput(value: T): void {
    this.latestPreviewValue = value;
    this.previewTimer.scheduleIfIdle(
      () => {
        if (this.latestPreviewValue === null) {
          return;
        }

        this.config.emitPreview(this.latestPreviewValue);
      },
      this.config.timing?.previewThrottleMs ?? DEFAULT_PREVIEW_THROTTLE_MS,
    );
    this.debounceTimer.schedule(() => {
      this.tryCommit(value);
    }, this.config.timing?.saveDebounceMs ?? DEFAULT_SAVE_DEBOUNCE_MS);
  }

  clearPending(): void {
    this.latestPreviewValue = null;
    this.previewTimer.clear();
    this.debounceTimer.clear();
  }

  tryCommit(value: T): boolean {
    this.clearPending();

    if (this.areEqual(value, this.lastCommittedValue)) {
      return false;
    }

    this.lastCommittedValue = value;
    this.config.emitSaved(value);
    this.showSavedIndicator();

    return true;
  }

  destroy(): void {
    this.clearPending();
    this.savedIconTimer.clear();
  }

  private showSavedIndicator(): void {
    this.config.setSavedVisible(true);
    this.savedIconTimer.schedule(
      () => this.config.setSavedVisible(false),
      this.config.timing?.savedIconDurationMs ?? DEFAULT_SAVED_ICON_DURATION_MS,
    );
  }
}

class ManagedTimer {
  private id: ReturnType<typeof setTimeout> | null = null;

  schedule(callback: () => void, ms: number): void {
    this.clear();
    this.id = setTimeout(() => {
      this.id = null;
      callback();
    }, ms);
  }

  scheduleIfIdle(callback: () => void, ms: number): void {
    if (this.id === null) {
      this.schedule(callback, ms);
    }
  }

  clear(): void {
    if (this.id !== null) {
      clearTimeout(this.id);
      this.id = null;
    }
  }
}
