/**
 * Formats a number of hours as a .NET `TimeSpan` string (`hh:mm:ss`), which is what
 * `POST /api/bookings` expects for `duration`. Accepts fractional hours (2.5 -> "02:30:00").
 */
export function hoursToTimeSpan(hours: number): string {
  const totalSeconds = Math.round(hours * 3600);
  const h = Math.floor(totalSeconds / 3600);
  const m = Math.floor((totalSeconds % 3600) / 60);
  const s = totalSeconds % 60;
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${pad(h)}:${pad(m)}:${pad(s)}`;
}

/** Whole hours between two ISO-ish datetime-local strings, rounded to 2dp. Used to prefill a booking. */
export function hoursBetween(startIso: string, endIso: string): number {
  const ms = new Date(endIso).getTime() - new Date(startIso).getTime();
  return Math.round((ms / 3_600_000) * 100) / 100;
}
