export interface PriceSegment {
  start: string;
  end: string;
  hours: number;
  multiplier: number;
  amount: number;
}

export interface PriceBreakdown {
  rentalTotal: number;
  servicesTotal: number;
  total: number;
  segments: PriceSegment[];
}

export interface BookingServiceLine {
  roomServiceId: number;
  name: string;
  priceAtBooking: number;
}

export interface Booking {
  id: number;
  roomId: number;
  startTime: string;
  endTime: string;
  services: BookingServiceLine[];
  priceBreakdown: PriceBreakdown;
}

export interface CreateBookingRequest {
  roomId: number;
  /** Wall-clock, no timezone: "2026-09-02T11:00:00". */
  startTime: string;
  /** TimeSpan as "hh:mm:ss" (not ISO-8601). */
  duration: string;
  serviceIds?: number[];
}
