import {Component, Injectable} from '@angular/core';
import {JoinGameComponent} from '$/app/home/join/join-game.component';
import {MyGamesComponent} from '$/app/home/my-games/my-games.component';
import {ParticipatingGamesComponent} from '$/app/home/participating-games/participating-games.component';

@Injectable()
@Component({
  selector: 'quibble-home',
  imports: [JoinGameComponent, MyGamesComponent, ParticipatingGamesComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
}
