import {Component, inject, OnInit} from '@angular/core';

import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {GamesService} from '$/services/api/games/games.service';
import {GameInfo, UserGameInfo} from '$/services/api/games/types/get-info';
import {GameRelationships} from '$/types/game-relationship';

@Component({
  selector: 'quibble-join-game',
  imports: [ReactiveFormsModule],
  templateUrl: './join-game.component.html',
  styleUrl: './join-game.component.css'
})
export class JoinGameComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private gamesService: GamesService = inject(GamesService);
  private formBuilder = inject(FormBuilder);

  maxLength: number = 6;

  loading: boolean = false;
  error?: string;

  form = this.formBuilder.group({
    // Intentionally don't constrain this input to being the exact slug length or disallowing invalid characters,
    // so that users are able to enter wrong slugs and receive an error, rather than just having
    // the 'join-game' button be locked
    slug: [''],
  });

  ngOnInit(): void {
    const slug = this.route.snapshot.params['slug']?.substring(0, this.maxLength);
    if (!slug)
      return;

    this.form.setValue({slug: slug});
    this.tryLoadGame();
  }

  tryLoadGame(): void {
    if (this.loading)
      return;

    const slug = this.form.value.slug?.toUpperCase();
    if (!slug) {
      this.error = "Enter game code";
      return;
    }

    this.loading = true;
    this.form.disable();

    this.gamesService.getGameInfo(slug)
      .subscribe({
        next: g => this.onGameInfoLoaded(g),
        error: e => this.onGameInfoLoadError(e),
      })
  }

  private async onGameInfoLoaded(gameInfo: GameInfo){
    this.loading = false;
    this.error = undefined;
    this.form.enable();

    if (gameInfo.relationship == GameRelationships.None) {
      await this.gamesService.joinGame(gameInfo.id);
    }

    await this.router.navigate(['/games', gameInfo.id], { replaceUrl: true });
  }

  private onGameInfoLoadError(error: any) {
    this.loading = false;
    this.form.enable();

    if (error.status == 404) {
      this.error = "Game not found";
    }
    else {
      this.error = error.statusText ?? "Unknown error";
    }
  }
}
