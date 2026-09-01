import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiError } from '../../../core/api-error';
import { RevenueReport as RevenueReportModel } from '../../../core/models/report';
import { ReportsApi } from '../../../core/services/reports.service';

@Component({
  selector: 'app-revenue-report',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './revenue-report.html',
})
export class RevenueReport {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ReportsApi);

  readonly report = signal<RevenueReportModel | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    from: ['', Validators.required],
    to: ['', Validators.required],
  });

  run(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { from, to } = this.form.getRawValue();
    if (from >= to) {
      this.error.set('“From” must be before “To”.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.report.set(null);
    this.api.revenue(from, to).subscribe({
      next: (report) => {
        this.report.set(report);
        this.loading.set(false);
      },
      error: (err: ApiError) => {
        this.loading.set(false);
        this.error.set(err.message);
      },
    });
  }
}
