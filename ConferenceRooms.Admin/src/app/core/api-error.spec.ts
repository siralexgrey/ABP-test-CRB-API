import { HttpErrorResponse } from '@angular/common/http';
import { messageFromHttpError } from './api-error';

describe('messageFromHttpError', () => {
  it('joins ValidationProblemDetails errors from [ApiController]', () => {
    const err = new HttpErrorResponse({
      status: 400,
      error: { errors: { Duration: ['Duration should be more than 0'], RoomId: ['Bad'] } },
    });

    expect(messageFromHttpError(err)).toBe('Duration should be more than 0 Bad');
  });

  it('uses ProblemDetails.detail for domain errors (404 / 409)', () => {
    const err = new HttpErrorResponse({
      status: 409,
      error: { title: 'Conflict', detail: 'Booking has times overlaps' },
    });

    expect(messageFromHttpError(err)).toBe('Booking has times overlaps');
  });

  it('reports an unreachable API on status 0', () => {
    const err = new HttpErrorResponse({ status: 0 });

    expect(messageFromHttpError(err)).toContain('Cannot reach the API');
  });

  it('falls back to the status code when the body has no message', () => {
    const err = new HttpErrorResponse({ status: 500, error: null });

    expect(messageFromHttpError(err)).toBe('Request failed with status 500.');
  });
});
