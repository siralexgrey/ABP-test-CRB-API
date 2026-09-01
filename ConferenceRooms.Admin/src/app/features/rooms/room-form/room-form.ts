import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiError } from '../../../core/api-error';
import { Room } from '../../../core/models/room';
import { RoomsApi } from '../../../core/services/rooms.service';

@Component({
  selector: 'app-room-form',
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './room-form.html',
})
export class RoomForm {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(RoomsApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly roomId = signal<number | null>(null);
  readonly isEdit = computed(() => this.roomId() !== null);

  readonly room = signal<Room | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly savedOnce = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    capacity: [10, [Validators.required, Validators.min(1)]],
    basePricePerHour: [1000, [Validators.required, Validators.min(0)]],
  });

  readonly serviceForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam !== null) {
      const id = Number(idParam);
      this.roomId.set(id);
      this.loadRoom(id);
    }
  }

  private loadRoom(id: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.get(id).subscribe({
      next: (room) => {
        this.room.set(room);
        this.form.patchValue({
          name: room.name,
          capacity: room.capacity,
          basePricePerHour: room.basePricePerHour,
        });
        this.loading.set(false);
      },
      error: (err: ApiError) => {
        this.loading.set(false);
        this.error.set(err.status === 404 ? `Room #${id} was not found.` : err.message);
      },
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);

    const value = this.form.getRawValue();
    const request = this.isEdit() ? this.api.update(this.roomId()!, value) : this.api.create(value);

    request.subscribe({
      next: (room) => {
        this.saving.set(false);
        if (this.isEdit()) {
          this.room.set(room);
          this.savedOnce.set(true);
        } else {
          void this.router.navigate(['/rooms', room.id]);
        }
      },
      error: (err: ApiError) => {
        this.saving.set(false);
        this.error.set(err.message);
      },
    });
  }

  addService(): void {
    const id = this.roomId();
    if (id === null || this.serviceForm.invalid) {
      this.serviceForm.markAllAsTouched();
      return;
    }
    this.error.set(null);
    this.api.addService(id, this.serviceForm.getRawValue()).subscribe({
      next: () => {
        this.serviceForm.reset({ name: '', price: 0 });
        this.loadRoom(id);
      },
      error: (err: ApiError) => this.error.set(err.message),
    });
  }
}
