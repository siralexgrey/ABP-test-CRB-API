import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiError } from '../../../core/api-error';
import { hoursBetween, hoursToTimeSpan } from '../../../core/duration';
import { Booking } from '../../../core/models/booking';
import { Room } from '../../../core/models/room';
import { BookingsApi } from '../../../core/services/bookings.service';
import { RoomsApi } from '../../../core/services/rooms.service';
import { PriceBreakdownView } from '../../../shared/price-breakdown/price-breakdown';

@Component({
  selector: 'app-booking-create',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe, PriceBreakdownView],
  templateUrl: './booking-create.html',
})
export class BookingCreate {
  private readonly fb = inject(FormBuilder);
  private readonly roomsApi = inject(RoomsApi);
  private readonly bookingsApi = inject(BookingsApi);
  private readonly route = inject(ActivatedRoute);

  readonly rooms = signal<Room[]>([]);
  readonly selectedServiceIds = signal<ReadonlySet<number>>(new Set());
  readonly created = signal<Booking | null>(null);
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  readonly lookup = signal<Booking | null>(null);
  readonly lookupError = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    roomId: [0, [Validators.required, Validators.min(1)]],
    startTime: ['', Validators.required],
    durationHours: [2, [Validators.required, Validators.min(0.25), Validators.max(24)]],
  });

  readonly lookupForm = this.fb.nonNullable.group({
    id: [1, [Validators.required, Validators.min(1)]],
  });

  private readonly roomId = toSignal(this.form.controls.roomId.valueChanges, {
    initialValue: this.form.controls.roomId.value,
  });

  readonly selectedRoom = computed(
    () => this.rooms().find((room) => room.id === this.roomId()) ?? null,
  );

  constructor() {
    // Services are per-room: whenever the room changes, drop ticks it no longer offers.
    effect(() => {
      const offered = new Set(this.selectedRoom()?.services.map((service) => service.id) ?? []);
      this.selectedServiceIds.update(
        (current) => new Set([...current].filter((id) => offered.has(id))),
      );
    });

    this.roomsApi.list().subscribe({
      next: (rooms) => {
        this.rooms.set(rooms);
        this.applyQueryParams();
      },
      error: (err: ApiError) => this.error.set(err.message),
    });
  }

  private applyQueryParams(): void {
    const params = this.route.snapshot.queryParamMap;
    const roomId = Number(params.get('roomId'));
    const start = params.get('start');
    const end = params.get('end');

    if (roomId && this.rooms().some((room) => room.id === roomId)) {
      this.form.controls.roomId.setValue(roomId);
    }
    if (start) {
      this.form.controls.startTime.setValue(start);
    }
    if (start && end) {
      const hours = hoursBetween(start, end);
      if (hours > 0) {
        this.form.controls.durationHours.setValue(hours);
      }
    }
  }

  isServiceSelected(id: number): boolean {
    return this.selectedServiceIds().has(id);
  }

  toggleService(id: number, checked: boolean): void {
    const next = new Set(this.selectedServiceIds());
    if (checked) {
      next.add(id);
    } else {
      next.delete(id);
    }
    this.selectedServiceIds.set(next);
  }

  previewDuration(): string {
    return hoursToTimeSpan(this.form.controls.durationHours.value);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { roomId, startTime, durationHours } = this.form.getRawValue();

    this.submitting.set(true);
    this.error.set(null);
    this.created.set(null);
    this.bookingsApi
      .create({
        roomId,
        startTime: `${startTime}:00`,
        duration: hoursToTimeSpan(durationHours),
        serviceIds: [...this.selectedServiceIds()],
      })
      .subscribe({
        next: (booking) => {
          this.created.set(booking);
          this.submitting.set(false);
        },
        error: (err: ApiError) => {
          this.submitting.set(false);
          this.error.set(err.message);
        },
      });
  }

  runLookup(): void {
    if (this.lookupForm.invalid) {
      return;
    }
    const id = this.lookupForm.getRawValue().id;

    this.lookupError.set(null);
    this.lookup.set(null);
    this.bookingsApi.get(id).subscribe({
      next: (booking) => this.lookup.set(booking),
      error: (err: ApiError) =>
        this.lookupError.set(err.status === 404 ? `No booking #${id}.` : err.message),
    });
  }
}
