import {HttpClient} from '@angular/common/http';
import {GameId} from '$/types/ids';
import {GameService} from '$/services/api/games/game.service';
import {RoundService} from '$/services/api/rounds/round.service';
import {QuestionService} from '$/services/api/questions/question.service';
import {ParticipantService} from '$/services/api/participants/participant.service';
import {AnswerService} from '$/services/api/answers/answer.service';

export class ApiService {
  public readonly game: GameService;
  public readonly round: RoundService;
  public readonly question: QuestionService;
  public readonly participant: ParticipantService;
  public readonly answer: AnswerService;

  constructor(http: HttpClient, gameId: GameId) {
    this.game = new GameService(http, gameId);
    this.round = new RoundService(http, gameId);
    this.question = new QuestionService(http);
    this.participant = new ParticipantService(http);
    this.answer = new AnswerService(http);
  }
}
