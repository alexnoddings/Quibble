import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import {SyncedInputText} from '$/app/synced-input/synced-input-text.component';

@Component({
  selector: 'quibble-synced-input-text-test-host',
  imports: [SyncedInputText],
  template: `
    <quibble-synced-input-text
      [value]="value()"
      (preview)="previews.push($event)"
      (saved)="changes.push($event)"
    />
  `,
})
class TestHost {
  readonly value = signal('');
  readonly previews: string[] = [];
  readonly changes: string[] = [];
}

function createTestHost(): ComponentFixture<TestHost> {
  const fixture = TestBed.createComponent(TestHost);
  fixture.detectChanges();

  return fixture;
}

function getInput(fixture: ComponentFixture<TestHost>): HTMLInputElement {
  return fixture.nativeElement.querySelector('input')!;
}

function typeValue(fixture: ComponentFixture<TestHost>, val: string): void {
  const input = getInput(fixture);
  input.value = val;
  input.dispatchEvent(new Event('input', { bubbles: true }));
  fixture.detectChanges();
}

function pressEnter(fixture: ComponentFixture<TestHost>): void {
  const input = getInput(fixture);
  const event = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true });
  input.dispatchEvent(event);
  fixture.detectChanges();
}

function blurInput(fixture: ComponentFixture<TestHost>): void {
  const input = getInput(fixture);
  // Simulate real browser behaviour: native change fires before blur when value was modified
  input.dispatchEvent(new Event('change', { bubbles: true }));
  input.dispatchEvent(new Event('blur', { bubbles: true }));
  fixture.detectChanges();
}

function getSavedIcon(fixture: ComponentFixture<TestHost>): HTMLElement | null {
  return fixture.nativeElement.querySelector('.saved-icon');
}

describe('SyncedInput', () => {
  beforeEach(async () => {
    vi.useFakeTimers();

    await TestBed.configureTestingModule({
      imports: [TestHost],
    }).compileComponents();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('should create the component', () => {
    // Arrange & Act
    const fixture = createTestHost();
    const input = getInput(fixture);

    // Assert
    expect(input).toBeTruthy();
  });

  it('should render the parent-provided value', () => {
    // Arrange
    const fixture = createTestHost();
    fixture.componentInstance.value.set('initial');

    // Act
    fixture.detectChanges();

    // Assert
    expect(getInput(fixture).value).toBe('initial');
  });

  it('should emit preview on trailing edge of 1s throttle', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'a');
    typeValue(fixture, 'ab');
    typeValue(fixture, 'abc');

    // Assert — no preview yet (trailing edge)
    expect(host.previews).toEqual([]);

    // Act — advance past 1s throttle window
    vi.advanceTimersByTime(1000);

    // Assert — single trailing preview with latest value
    expect(host.previews).toEqual(['abc']);
  });

  it('should NOT emit preview immediately (no leading edge)', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'x');

    // Assert — no preview yet
    expect(host.previews).toEqual([]);

    // Act — advance past throttle
    vi.advanceTimersByTime(1000);

    // Assert
    expect(host.previews).toEqual(['x']);
  });

  it('should emit change after 1.5s of no typing', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'hello');
    vi.advanceTimersByTime(1500);
    fixture.detectChanges();

    // Assert
    expect(host.changes).toEqual(['hello']);
  });

  it('should emit change on Enter', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'enter-value');
    pressEnter(fixture);

    // Assert
    expect(host.changes).toEqual(['enter-value']);
  });

  it('should emit change on blur', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'blur-value');
    blurInput(fixture);

    // Assert
    expect(host.changes).toEqual(['blur-value']);
  });

  it('should NOT emit a second change when debounce fires after Enter', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'dedup');
    pressEnter(fixture);
    expect(host.changes).toEqual(['dedup']);

    // Act — advance past debounce timer
    vi.advanceTimersByTime(1500);

    // Assert — still only one change
    expect(host.changes).toEqual(['dedup']);
  });

  it('should NOT emit a second change when debounce fires after blur', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'dedup-blur');
    blurInput(fixture);
    expect(host.changes).toEqual(['dedup-blur']);

    // Act — advance past debounce timer
    vi.advanceTimersByTime(1500);

    // Assert
    expect(host.changes).toEqual(['dedup-blur']);
  });

  it('should NOT emit a second change when blur fires after debounce', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'dedup-timer');
    vi.advanceTimersByTime(1500);
    fixture.detectChanges();
    expect(host.changes).toEqual(['dedup-timer']);

    // Act — now blur
    blurInput(fixture);

    // Assert
    expect(host.changes).toEqual(['dedup-timer']);
  });

  it('should NOT emit change if value is unchanged', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act — type and commit via Enter
    typeValue(fixture, 'same');
    pressEnter(fixture);
    expect(host.changes).toEqual(['same']);

    // Act — press Enter again with same value
    pressEnter(fixture);

    // Assert — still only one change
    expect(host.changes).toEqual(['same']);
  });

  it('should show saved icon after change event', () => {
    // Arrange
    const fixture = createTestHost();

    // Act
    typeValue(fixture, 'saved');
    pressEnter(fixture);
    fixture.detectChanges();

    // Assert
    expect(getSavedIcon(fixture)).toBeTruthy();
    expect(getSavedIcon(fixture)!.textContent).toContain('✓');
  });

  it('should hide saved icon after 2 seconds', () => {
    // Arrange
    const fixture = createTestHost();

    // Act
    typeValue(fixture, 'temp');
    pressEnter(fixture);
    fixture.detectChanges();
    expect(getSavedIcon(fixture)).toBeTruthy();

    // Act — advance 2s
    vi.advanceTimersByTime(2000);
    fixture.detectChanges();

    // Assert
    expect(getSavedIcon(fixture)).toBeNull();
  });

  it('should reset saved icon timer on subsequent change', () => {
    // Arrange
    const fixture = createTestHost();

    // Act — first change
    typeValue(fixture, 'first');
    pressEnter(fixture);
    fixture.detectChanges();
    expect(getSavedIcon(fixture)).toBeTruthy();

    // Act — wait 1s, then second change
    vi.advanceTimersByTime(1000);
    typeValue(fixture, 'second');
    pressEnter(fixture);
    fixture.detectChanges();
    expect(getSavedIcon(fixture)).toBeTruthy();

    // Act — advance 1.5s (would be past 2s from first, but not from second)
    vi.advanceTimersByTime(1500);
    fixture.detectChanges();

    // Assert — icon still visible (reset by second change)
    expect(getSavedIcon(fixture)).toBeTruthy();

    // Act — advance remaining 0.5s
    vi.advanceTimersByTime(500);
    fixture.detectChanges();

    // Assert — now gone
    expect(getSavedIcon(fixture)).toBeNull();
  });

  it('should NOT emit events when parent sets value programmatically', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    host.value.set('programmatic');
    fixture.detectChanges();
    vi.advanceTimersByTime(1500);

    // Assert
    expect(host.previews).toEqual([]);
    expect(host.changes).toEqual([]);
  });

  it('should prevent default on Enter key', () => {
    // Arrange
    const fixture = createTestHost();
    const input = getInput(fixture);
    typeValue(fixture, 'prevent');

    // Act
    const event = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true });
    const preventSpy = vi.spyOn(event, 'preventDefault');
    input.dispatchEvent(event);
    fixture.detectChanges();

    // Assert
    expect(preventSpy).toHaveBeenCalled();
  });

  it('should emit a new change after value changes again post-commit', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act — first change
    typeValue(fixture, 'v1');
    pressEnter(fixture);
    expect(host.changes).toEqual(['v1']);

    // Act — second change with different value
    typeValue(fixture, 'v2');
    pressEnter(fixture);

    // Assert
    expect(host.changes).toEqual(['v1', 'v2']);
  });

  it('should not leak native change event as [object Event]', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act — type a value and blur (which dispatches native change + blur)
    typeValue(fixture, 'leak-test');
    blurInput(fixture);

    // Assert — only one change with the string value, no Event objects
    expect(host.changes).toEqual(['leak-test']);
    expect(host.changes.every((c) => typeof c === 'string')).toBe(true);
  });

  it('should cancel pending preview when saved fires', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act — type to schedule a preview, then immediately commit via Enter
    typeValue(fixture, 'cancel-preview');
    pressEnter(fixture);

    // Act — advance past the 1s throttle window
    vi.advanceTimersByTime(1000);

    // Assert — saved fired, but no preview since saved supersedes it
    expect(host.changes).toEqual(['cancel-preview']);
    expect(host.previews).toEqual([]);
  });
});
