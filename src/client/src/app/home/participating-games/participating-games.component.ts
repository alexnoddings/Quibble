import {Component, inject, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink} from '@angular/router';
import {ReactiveFormsModule} from '@angular/forms';
import {GamesService} from '$/services/api/games/games.service';
import {ParticipantGameInfo} from '$/services/api/games/types/get-info';
import {Observable} from 'rxjs';

@Component({
  selector: 'quibble-participating-games',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './participating-games.component.html',
  styleUrls: []
})
export class ParticipatingGamesComponent implements OnInit {
  private gamesService: GamesService = inject(GamesService);

  games$?: Observable<ParticipantGameInfo[]>;

  ngOnInit(): void {
    this.games$ = this.gamesService.getParticipatingGames();
  }
}
