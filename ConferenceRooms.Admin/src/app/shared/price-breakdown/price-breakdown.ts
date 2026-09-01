import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { PriceBreakdown } from '../../core/models/booking';

/** Renders the API's segment-by-segment `PriceBreakdown` as a table. Shared by booking create + lookup. */
@Component({
  selector: 'app-price-breakdown',
  imports: [DatePipe, DecimalPipe],
  templateUrl: './price-breakdown.html',
})
export class PriceBreakdownView {
  readonly breakdown = input.required<PriceBreakdown>();
}
