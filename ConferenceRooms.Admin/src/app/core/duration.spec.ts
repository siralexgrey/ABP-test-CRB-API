import { hoursBetween, hoursToTimeSpan } from './duration';

describe('hoursToTimeSpan', () => {
  it('formats whole hours', () => {
    expect(hoursToTimeSpan(2)).toBe('02:00:00');
  });

  it('formats fractional hours', () => {
    expect(hoursToTimeSpan(2.5)).toBe('02:30:00');
  });

  it('formats the 24h upper bound the API still accepts', () => {
    expect(hoursToTimeSpan(24)).toBe('24:00:00');
  });

  it('rounds sub-second noise', () => {
    expect(hoursToTimeSpan(1 / 3)).toBe('00:20:00');
  });
});

describe('hoursBetween', () => {
  it('returns the gap in hours', () => {
    expect(hoursBetween('2026-09-02T11:00', '2026-09-02T13:30')).toBe(2.5);
  });
});
