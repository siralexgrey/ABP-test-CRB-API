import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiError } from '../../../core/api-error';
import { hoursBetween } from '../../../core/duration';
import { Room } from '../../../core/models/room';
import { RoomsApi } from '../../../core/services/rooms.service';

@Component({
  selector: 'app-availability-search',
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './availability-search.html',
})
export class AvailabilitySearch {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(RoomsApi);

  readonly results = signal<Room[] | null>(null);
  readonly searching = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    start: ['', Validators.required],
    end: ['', Validators.required],
    minCapacity: [10, [Validators.required, Validators.min(1)]],
  });

  windowHours(): number | null {
    const { start, end } = this.form.getRawValue();
    return start && end && end > start ? hoursBetween(start, end) : null;
  }

  search(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { start, end, minCapacity } = this.form.getRawValue();
    if (start >= end) {
      this.error.set('Start must be before end.');
      return;
    }

    this.searching.set(true);
    this.error.set(null);
    this.results.set(null);
    this.api.available(start, end, minCapacity).subscribe({
      next: (rooms) => {
        this.results.set(rooms);
        this.searching.set(false);
      },
      error: (err: ApiError) => {
        this.searching.set(false);
        this.error.set(err.message);
      },
    });
  }
}
