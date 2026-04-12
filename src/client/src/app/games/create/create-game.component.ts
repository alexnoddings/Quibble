import {Component, inject} from '@angular/core';

import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {CreateGameResponse, GamesService} from '$/services/api/games/games.service';

@Component({
  selector: 'quibble-create-game',
  imports: [ReactiveFormsModule],
  templateUrl: './create-game.component.html',
  styleUrls: [],
})
export class CreateGameComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private gamesService: GamesService = inject(GamesService);
  private formBuilder = inject(FormBuilder);

  loading: boolean = false;
  error?: string;

  form = this.formBuilder.group({
    title: ['', [Validators.required, Validators.max(100)]],
  });

  tryCreateGame(): void {
    if (this.loading)
      return;

    const title = this.form.value.title?.trim();
    if (!title) {
      this.error = "Game must have a title";
      return;
    }

    this.loading = true;
    this.form.disable();

    this.gamesService.createGame(title)
      .subscribe({
        next: g => this.onGameCreated(g),
        error: e => this.onGameCreateError(e),
      })
  }

  async onGameCreated(game: CreateGameResponse){
    this.loading = false;
    this.error = undefined;
    this.form.enable();
    await this.router.navigate(['/games', game.id]);
  }

  onGameCreateError(error: any) {
    console.log(error);

    this.loading = false;
    this.form.enable();

    if (error.status == 400) {
      this.error = error.statusText ?? "Invalid title";
    }
    else {
      this.error = error.statusText ?? "Unknown error";
    }
  }
}
