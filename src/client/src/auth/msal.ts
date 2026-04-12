import {
  BrowserCacheLocation,
  InteractionType,
  IPublicClientApplication,
  LogLevel,
  PublicClientApplication
} from '@azure/msal-browser';
import {environment} from '$/env/environment';
import {
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE, MSAL_INTERCEPTOR_CONFIG, MsalBroadcastService, MsalGuard,
  MsalGuardConfiguration,
  MsalInterceptor,
  MsalInterceptorConfiguration, MsalService
} from '@azure/msal-angular';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {EnvironmentProviders, makeEnvironmentProviders} from '@angular/core';

function loggerCallback(logLevel: LogLevel, message: string) {
  console.log(message);
}

function InstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.msal.clientId,
      authority: environment.msal.authority,
      redirectUri: '/auth',
      postLogoutRedirectUri: '/',
    },
    cache: {
      cacheLocation: BrowserCacheLocation.SessionStorage,
    },
    system: {
      allowPlatformBroker: false, // Disables WAM Broker
      loggerOptions: {
        loggerCallback,
        logLevel: LogLevel.Info,
        piiLoggingEnabled: false,
      },
    },
  });
}

function InterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  protectedResourceMap.set(
    environment.api.uri,
    environment.api.scopes
  );

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap,
  };
}

function GuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: ['User.Read', ...environment.api.scopes],
    },
    loginFailedRoute: '/login-failed',
  };
}

export function provideMsalAuth(): EnvironmentProviders {
  return makeEnvironmentProviders([
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: InstanceFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: GuardConfigFactory,
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: InterceptorConfigFactory,
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService,
  ]);
}
