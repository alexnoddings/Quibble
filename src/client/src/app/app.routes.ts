import {Routes} from '@angular/router';
import {HomeComponent} from '$/app/home/home.component';
import {LoginFailedComponent} from '$/app/login-failed/login-failed.component';
import {GameComponent} from '$/app/games/game.component';
import {NotFoundComponent} from '$/app/not-found/not-found.component';
import {MsalGuard, MsalRedirectComponent} from '@azure/msal-angular';
import {CreateGameComponent} from '$/app/games/create/create-game.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'join',
    component: HomeComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'join/:slug',
    component: HomeComponent,
    canActivate: [MsalGuard]
  },
  /* Games */
  {
    path: 'create-game',
    component: CreateGameComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'games/:id',
    component: GameComponent,
    canActivate: [MsalGuard]
  },
  /* Auth */
  {
    path: 'login-failed',
    component: LoginFailedComponent
  },
  {
    path: 'auth',
    component: MsalRedirectComponent
  },
  /* Misc */
  {
    path: '**',
    component: NotFoundComponent,
    canActivate: [MsalGuard]
  },
];
