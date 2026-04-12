import {inject, Injectable} from '@angular/core';
import {FullGameInteractive, FullGameInteractiveImpl} from '$/services/interactive/full/game';
import {ApiServiceFactory} from '$/services/api/api.factory';
import {AuthenticatedHubConnectionBuilder} from '$/auth/msal-signalr';
import {BufferReady} from '$/utils/buffer/buffer-ready';
import {SignalrGameEventBus} from '$/services/events/bus/signalr-bus';
import {BufferGameEventBus} from '$/services/events/bus/buffer-bus';
import {ok, Result} from '$/services/api/result';
import {PartialGameInteractive, PartialGameInteractiveImpl} from '$/services/interactive/partial/game';

@Injectable({
  providedIn: 'root',
})
export class GameFactory {
  private readonly authenticatedHubConnectionBuilder = inject(AuthenticatedHubConnectionBuilder);
  private readonly apiServiceFactory = inject(ApiServiceFactory);

  async getGame(id: string): Promise<Result<GameInteractive>> {
    const ready = new BufferReady;

    // Setup
    const apiService = this.apiServiceFactory.get(id);
    const hubConnection = this.authenticatedHubConnectionBuilder
      .withUrl(`/api/games/${id}/events`)
      .build();

    // Create adapters
    const signalrAdapter = new SignalrGameEventBus(hubConnection);
    const bufferWrapperAdapter = signalrAdapter; // new BufferGameEventBus(ready, signalrAdapter);

    // Start hub connection BEFORE we load data.
    // This removes any race conditions with data being updated between us loading it and starting the connection.
    // This may mean we end up with events that have already been applied to the data,
    // but this isn't an issue since the event handling is idempotent.
    try {
      await hubConnection.start();
    } catch (error) {
      console.error('Error connecting to SignalR hub:', error);
      throw error;
    }

    console.log('Connected to SignalR hub');

    // Now load the api's data
    const gameResult = await apiService.game.getData();
    if (!gameResult.isOk) {
      // Loading the game responded with an error, likely that the game doesn't exist.
      try {
        await hubConnection.stop();
      } catch {
        // noop
      }
      return gameResult;
    }

    const gameData = gameResult.value;
    const perspective = gameData.perspective;
    let game;

    // Create the interactive game
    if (perspective == 'Host') {
      game = new FullGameInteractiveImpl(apiService, bufferWrapperAdapter, gameData);
    } else if (perspective == 'Participant') {
      game = new PartialGameInteractiveImpl(apiService, bufferWrapperAdapter, gameData);
    } else {
      throw Error(`Invalid game perspective '${perspective}'.`);
    }

    // Once everything is set up and ready, we can release and apply the event buffer
    ready.ready();

    // And now we're fully alive
    return ok(game);
  }
}

export type GameInteractive = FullGameInteractive | PartialGameInteractive;
