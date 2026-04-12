import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import {SyncedInputNumber} from './synced-input-number.component';

@Component({
  selector: 'quibble-synced-input-number-test-host',
  imports: [SyncedInputNumber],
  template: `
    <quibble-synced-input-number
      [value]="value()"
      (preview)="previews.push($event)"
      (saved)="savedValues.push($event)"
    />
  `,
})
class TestHost {
  readonly value = signal(12.34);
  readonly previews: number[] = [];
  readonly savedValues: number[] = [];
}

function createTestHost(): ComponentFixture<TestHost> {
  const fixture = TestBed.createComponent(TestHost);
  fixture.detectChanges();

  return fixture;
}

function getInput(fixture: ComponentFixture<TestHost>): HTMLInputElement {
  return fixture.nativeElement.querySelector('input')!;
}

function typeValue(fixture: ComponentFixture<TestHost>, value: string): void {
  const input = getInput(fixture);
  input.value = value;
  input.dispatchEvent(new Event('input', { bubbles: true }));
  fixture.detectChanges();
}

function pressEnter(fixture: ComponentFixture<TestHost>): void {
  const input = getInput(fixture);
  input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true }));
  fixture.detectChanges();
}

function blurInput(fixture: ComponentFixture<TestHost>): void {
  const input = getInput(fixture);
  input.dispatchEvent(new Event('change', { bubbles: true }));
  input.dispatchEvent(new Event('blur', { bubbles: true }));
  fixture.detectChanges();
}

describe('SyncedNumberInput', () => {
  beforeEach(async () => {
    vi.useFakeTimers();

    await TestBed.configureTestingModule({
      imports: [TestHost],
    }).compileComponents();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('should render the parent-provided number', () => {
    // Arrange
    const fixture = createTestHost();

    // Assert
    expect(getInput(fixture).value).toBe('12.34');
  });

  it('should emit rounded preview values for valid numeric input', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '12.345');
    vi.advanceTimersByTime(1000);

    // Assert
    expect(host.previews).toEqual([12.35]);
  });

  it('should emit rounded saved values on Enter', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '9.999');
    pressEnter(fixture);

    // Assert
    expect(host.savedValues).toEqual([10]);
    expect(getInput(fixture).value).toBe('10');
  });

  it('should not emit preview for invalid input', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '12.3');
    typeValue(fixture, '12.3.4');
    vi.advanceTimersByTime(1000);

    // Assert
    expect(host.previews).toEqual([]);
  });

  it('should not emit saved for invalid input on Enter', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, 'abc');
    pressEnter(fixture);

    // Assert
    expect(host.savedValues).toEqual([]);
    expect(getInput(fixture).value).toBe('abc');
  });

  it('should not emit saved for invalid input after debounce', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '12..4');
    vi.advanceTimersByTime(1500);

    // Assert
    expect(host.savedValues).toEqual([]);
  });

  it('should reset invalid input to the latest bound value on blur', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;
    host.value.set(7.5);
    fixture.detectChanges();

    // Act
    typeValue(fixture, '7.5.1');
    blurInput(fixture);

    // Assert
    expect(host.savedValues).toEqual([]);
    expect(getInput(fixture).value).toBe('7.5');
  });

  it('should reset empty input to the latest bound value on blur', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '');
    blurInput(fixture);

    // Assert
    expect(host.savedValues).toEqual([]);
    expect(getInput(fixture).value).toBe('12.34');
  });

  it('should save on blur when the current numeric value is valid', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '4.567');
    blurInput(fixture);

    // Assert
    expect(host.savedValues).toEqual([4.57]);
    expect(getInput(fixture).value).toBe('4.57');
  });

  it('should cancel a pending preview when saved fires', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '3.456');
    pressEnter(fixture);
    vi.advanceTimersByTime(1000);

    // Assert
    expect(host.savedValues).toEqual([3.46]);
    expect(host.previews).toEqual([]);
  });

  it('should reset the displayed buffer and clear pending timers on parent updates', () => {
    // Arrange
    const fixture = createTestHost();
    const host = fixture.componentInstance;

    // Act
    typeValue(fixture, '99.99');
    host.value.set(5.5);
    fixture.detectChanges();
    vi.advanceTimersByTime(1500);

    // Assert
    expect(getInput(fixture).value).toBe('5.5');
    expect(host.previews).toEqual([]);
    expect(host.savedValues).toEqual([]);
  });
});
