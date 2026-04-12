import {Component, inject} from '@angular/core';

import {RouterLink} from '@angular/router';
import {ReactiveFormsModule} from '@angular/forms';
import {MsalService} from '@azure/msal-angular';

@Component({
  selector: 'quibble-header',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  authService: MsalService = inject(MsalService);
}
