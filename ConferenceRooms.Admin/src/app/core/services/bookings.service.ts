import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Booking, CreateBookingRequest } from '../models/booking';

@Injectable({ providedIn: 'root' })
export class BookingsApi {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/bookings';

  get(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.base}/${id}`);
  }

  create(body: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(this.base, body);
  }
}
