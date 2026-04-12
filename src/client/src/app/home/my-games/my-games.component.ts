import {Component, inject, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink} from '@angular/router';
import {ReactiveFormsModule} from '@angular/forms';
import {GamesService} from '$/services/api/games/games.service';
import {UserGameInfo} from '$/services/api/games/types/get-info';
import {Observable} from 'rxjs';
import {GameStates} from '$/types/game-state';

@Component({
  selector: 'quibble-my-games',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './my-games.component.html',
  styleUrls: []
})
export class MyGamesComponent implements OnInit {
  private gamesService: GamesService = inject(GamesService);

  games$?: Observable<UserGameInfo[]>;

  ngOnInit(): void {
    this.games$ = this.gamesService.getOwnedGames();
  }

  protected group(games: UserGameInfo[]){
    return {
      drafts: games.filter(g => g.state == GameStates.Draft),
      active: games.filter(g => g.state == GameStates.Open || g.state == GameStates.InProgress),
      done: games.filter(g => g.state == GameStates.Completed),
    }
  }

  protected readonly GameStates = GameStates;
}
