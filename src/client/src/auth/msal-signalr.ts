import {inject, Injectable,} from "@angular/core";
import {AccountInfo, Logger} from "@azure/msal-browser";
import {firstValueFrom} from "rxjs";
import {HttpTransportType, HubConnectionBuilder} from '@microsoft/signalr';
import {MsalService} from '@azure/msal-angular';
import {IHttpConnectionOptions} from '@microsoft/signalr/src/IHttpConnectionOptions';
import {environment} from '$/env/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthenticatedHubConnectionBuilder {
  private auth = inject(MsalService);

  public withUrl(url: string): HubConnectionBuilder {
    const options: IHttpConnectionOptions = {
      accessTokenFactory: this.createAccessTokenFactory()
    };
    return new HubConnectionBuilder().withUrl(url, options);
  }

  private createAccessTokenFactory(): () => Promise<string> {
    const scopes = environment.api.scopes;
    const auth = this.auth;
    const accessTokenFactory = new AccessTokenFactory(scopes, auth);

    return () => accessTokenFactory.getAccessToken();
  }
}

class AccessTokenFactory {
  private readonly scopes : string[];
  private readonly auth: MsalService;

  public constructor(scopes: string[], auth: MsalService) {
    this.scopes = scopes;
    this.auth = auth;
  }

  private logger() : Logger {
    return this.auth.getLogger();
  }

  public async getAccessToken() : Promise<string> {
    await firstValueFrom(this.auth.initialize());

    this.logger().info("MSAL Interceptor activated");

    const account = this.getAccount();

    this.logger().info(`??? Interceptor - ${this.scopes.length} scopes`);
    this.logger().infoPii(`??? Interceptor - using [${this.scopes}] scopes`);

    const token = await this.getToken(account);

    this.logger().verbose("??? Interceptor - setting authorization headers");

    return token;
  }

  private getAccount() : AccountInfo {
    const activeAccount = this.auth.instance.getActiveAccount();
    if (activeAccount != null) {
      this.logger().info("Interceptor - active account selected");
      return activeAccount;
    }

    this.logger().info("Interceptor - no active account, fallback to first account");
    const accounts = this.auth.instance.getAllAccounts();
    return accounts[0];
  }

  private async getToken(account: AccountInfo) : Promise<string> {
    const authResult$ = this.auth.acquireTokenSilent({
      scopes: this.scopes,
      account: account
    });
    const authResult = await firstValueFrom(authResult$);
    return authResult.accessToken;
  }
}
