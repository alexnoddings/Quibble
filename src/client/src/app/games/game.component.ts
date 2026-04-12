import {Component, inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {ErrorComponent} from '$/app/error/error.component';
import {GameFactory} from '$/services/interactive/game.factory';
import {GameHostViewComponent} from '$/app/games/host/game-host-view.component';
import {GameParticipantViewComponent} from '$/app/games/participant/game-participant-view.component';

@Component({
  selector: 'quibble-game',
  imports: [CommonModule, ErrorComponent, GameHostViewComponent, GameParticipantViewComponent],
  templateUrl: './game.component.html',
  styleUrls: [],
})
export class GameComponent {
  private route = inject(ActivatedRoute);
  private gameFactory = inject(GameFactory);

  private id = this.route.snapshot.params['id'];
  public gameResult$ = this.gameFactory.getGame(this.id);
}
