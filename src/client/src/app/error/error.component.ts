import {Component, input} from '@angular/core';

import {ProblemDetails} from '$/services/api/problem-details';

@Component({
  selector: 'quibble-error',
  imports: [],
  templateUrl: './error.component.html',
  styleUrl: './error.component.css'
})
export class ErrorComponent {
  error = input.required<ProblemDetails>();
}
