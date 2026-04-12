import {Subject} from 'rxjs';

export class BufferReady {
  private readonly source$ = new Subject<void>;
  public readonly ready$ = this.source$.asObservable();

  public ready(): void {
    this.source$.next();
    this.source$.complete();
  }
}
