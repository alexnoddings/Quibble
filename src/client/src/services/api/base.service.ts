import {HttpClient} from "@angular/common/http";
import {firstValueFrom, Observable, timeout} from "rxjs";
import {asResult, Result} from '$/services/api/result';

export class BaseEntityService {
  protected readonly http: HttpClient;
  private readonly apiBase: string;

  constructor(http: HttpClient) {
    this.http = http;
    this.apiBase = `/api`;
  }

  protected getUrl(relativeUrl: string): string {
    if (!relativeUrl.startsWith('/'))
      relativeUrl = '/' + relativeUrl;

    return this.apiBase + relativeUrl;
  }

  protected async send<T>(http: Observable<T>): Promise<Result<T>> {
    const result$ =
      http
        .pipe(timeout(5_000))
        .pipe(asResult());

    return await firstValueFrom(result$);
  }
}
