import { HttpErrorResponse } from '@angular/common/http';

/** Normalised transport error surfaced to components by {@link apiErrorInterceptor}. */
export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

/** Shape of RFC 7807 responses the API returns (both ProblemDetails and ValidationProblemDetails). */
interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

export function messageFromHttpError(err: HttpErrorResponse): string {
  if (err.status === 0) {
    return 'Cannot reach the API. Make sure it is running (see README) and the dev proxy target matches.';
  }

  const problem = (err.error ?? {}) as ProblemDetails;

  if (problem.errors) {
    const messages = Object.values(problem.errors).flat();
    if (messages.length > 0) {
      return messages.join(' ');
    }
  }
  if (problem.detail) {
    return problem.detail;
  }
  if (problem.title) {
    return problem.title;
  }

  return `Request failed with status ${err.status}.`;
}
