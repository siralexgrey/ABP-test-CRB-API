import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'rooms' },
  {
    path: 'rooms',
    loadComponent: () => import('./features/rooms/rooms-list/rooms-list').then((m) => m.RoomsList),
  },
  {
    path: 'rooms/new',
    loadComponent: () => import('./features/rooms/room-form/room-form').then((m) => m.RoomForm),
  },
  {
    path: 'rooms/:id',
    loadComponent: () => import('./features/rooms/room-form/room-form').then((m) => m.RoomForm),
  },
  {
    path: 'availability',
    loadComponent: () =>
      import('./features/availability/availability-search/availability-search').then(
        (m) => m.AvailabilitySearch,
      ),
  },
  {
    path: 'bookings/new',
    loadComponent: () =>
      import('./features/bookings/booking-create/booking-create').then((m) => m.BookingCreate),
  },
  {
    path: 'reports/revenue',
    loadComponent: () =>
      import('./features/reports/revenue-report/revenue-report').then((m) => m.RevenueReport),
  },
  { path: '**', redirectTo: 'rooms' },
];
