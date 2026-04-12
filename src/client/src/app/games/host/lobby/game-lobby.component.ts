import {Component, input, ViewChild} from '@angular/core';

import {FullGameInteractive} from '$/services/interactive/full/game';
import {GameStates} from '$/types/game-state';

@Component({
  selector: 'quibble-host-game-lobby',
  imports: [],
  templateUrl: './game-lobby.component.html',
  styleUrls: ['./game-lobby.component.css'],
})
export class HostGameLobbyComponent {
  public game = input.required<FullGameInteractive>();

  protected error?: string;

  public async publishGame() : Promise<void> {
    this.error = undefined;
    const game = this.game();
    if (game.participants().length === 0)
    {
      this.error = "It's a bit lonely in here! Invite some players to join before you start.";
      return;
    }

    await game.updateState(GameStates.InProgress);
  }

  public getSlugUrl(): string {
    const base = window.origin;
    const slug = this.game().slug;
    return `${base}/join/${slug}`;
  }

  public copyToClipboard(element: Element): void {
    const content = element.textContent?.trim();
    if (content != null)
      navigator.clipboard.writeText(content);

    this.select(element);
    // notify copied
  }

  public select(element: Element): void {
    const range = document.createRange();
    range.selectNodeContents(element);

    const selection = window.getSelection();
    if (selection == null)
      return;

    selection.removeAllRanges();
    selection.addRange(range);
  }
}
