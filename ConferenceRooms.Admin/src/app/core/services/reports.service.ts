import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RevenueReport } from '../models/report';

@Injectable({ providedIn: 'root' })
export class ReportsApi {
  private readonly http = inject(HttpClient);

  revenue(from: string, to: string): Observable<RevenueReport> {
    return this.http.get<RevenueReport>('/api/reports/revenue', { params: { from, to } });
  }
}
