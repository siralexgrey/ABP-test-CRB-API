import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError, messageFromHttpError } from './api-error';

/** Turns every failed response into an {@link ApiError} carrying a display-ready message. */
export const apiErrorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        return throwError(() => new ApiError(messageFromHttpError(err), err.status));
      }
      return throwError(() => err);
    }),
  );
