import {ProblemDetails} from '$/services/api/problem-details';
import {catchError, map, Observable, of, OperatorFunction} from 'rxjs';
import {createProblemDetails} from '$/services/api/problem-details.factory';

export interface OkResult<T> {
  readonly isOk: true;
  readonly value: T;
}

export interface ErrorResult {
  readonly isOk: false;
  readonly error: ProblemDetails;
}

export type Result<T> = OkResult<T> | ErrorResult;

function operator<T>($: Observable<T>): Observable<Result<T>> {
  return $
    .pipe(map(value => ok(value)))
    .pipe(catchError(err => {
      const problemDetails = createProblemDetails(err)
      return of(error(problemDetails));
    }));
}

export function asResult<T>() : OperatorFunction<T, Result<T>> {
  return operator<T>;
}

export function error(problemDetails: ProblemDetails) : ErrorResult {
  return {
    isOk: false,
    error: problemDetails
  };
}

export function ok<T>(value: T) : OkResult<T> {
  return {
    isOk: true,
    value: value
  };
}
