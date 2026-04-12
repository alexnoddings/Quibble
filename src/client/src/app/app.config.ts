import {ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection} from '@angular/core';
import {provideHttpClient} from '@angular/common/http';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {withInterceptorsFromDi, withFetch} from '@angular/common/http';
import {provideNoopAnimations} from '@angular/platform-browser/animations';
import {provideMsalAuth} from '$/auth/msal';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideNoopAnimations(),
    provideHttpClient(
      withInterceptorsFromDi(),
      withFetch()
    ),
    provideMsalAuth()
  ],
};
