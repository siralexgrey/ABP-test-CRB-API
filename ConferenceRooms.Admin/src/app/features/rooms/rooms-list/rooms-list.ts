import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiError } from '../../../core/api-error';
import { Room } from '../../../core/models/room';
import { RoomsApi } from '../../../core/services/rooms.service';

@Component({
  selector: 'app-rooms-list',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './rooms-list.html',
})
export class RoomsList {
  private readonly api = inject(RoomsApi);

  readonly rooms = signal<Room[] | null>(null);
  readonly error = signal<string | null>(null);
  readonly pendingDelete = signal<number | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.error.set(null);
    this.rooms.set(null);
    this.pendingDelete.set(null);
    this.api.list().subscribe({
      next: (rooms) => this.rooms.set(rooms),
      error: (err: ApiError) => this.error.set(err.message),
    });
  }

  remove(room: Room): void {
    this.api.remove(room.id).subscribe({
      next: () => this.load(),
      error: (err: ApiError) => {
        this.pendingDelete.set(null);
        this.error.set(`Could not delete "${room.name}": ${err.message}`);
      },
    });
  }
}
