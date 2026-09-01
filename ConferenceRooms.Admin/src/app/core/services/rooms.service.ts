import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateRoomRequest,
  CreateRoomServiceRequest,
  Room,
  RoomService,
  UpdateRoomRequest,
} from '../models/room';

@Injectable({ providedIn: 'root' })
export class RoomsApi {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/rooms';

  list(): Observable<Room[]> {
    return this.http.get<Room[]>(this.base);
  }

  get(id: number): Observable<Room> {
    return this.http.get<Room>(`${this.base}/${id}`);
  }

  available(start: string, end: string, minCapacity: number): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.base}/available`, {
      params: { start, end, minCapacity },
    });
  }

  create(body: CreateRoomRequest): Observable<Room> {
    return this.http.post<Room>(this.base, body);
  }

  update(id: number, body: UpdateRoomRequest): Observable<Room> {
    return this.http.put<Room>(`${this.base}/${id}`, body);
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  addService(id: number, body: CreateRoomServiceRequest): Observable<RoomService> {
    return this.http.post<RoomService>(`${this.base}/${id}/services`, body);
  }
}
