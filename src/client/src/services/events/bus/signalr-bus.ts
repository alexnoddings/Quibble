import {HubConnection} from "@microsoft/signalr";
import {Observable, Subject} from "rxjs";
import {GameStateChangedEvent, GameTitleChangedEvent} from "$/services/events/game.events";
import {ParticipantAddedEvent} from "$/services/events/participant.events";
import {
  RoundAddedEvent,
  RoundDescriptionChangedEvent,
  RoundOrderChangedEvent,
  RoundRemovedEvent,
  RoundStateChangedEvent,
  RoundTitleChangedEvent
} from "$/services/events/round.events";
import {
  QuestionAddedEvent,
  QuestionAnswerTextChangedEvent,
  QuestionBodyTextChangedEvent,
  QuestionOrderChangedEvent,
  QuestionPointsChangedEvent,
  QuestionRemovedEvent, QuestionRevealedEvent,
  QuestionStateChangedEvent
} from "$/services/events/question.events";
import {GameEventBus} from '$/services/events/bus/event-bus';
import {environment} from '$/env/environment';
import {
  AnswerPointsChangedEvent,
  AnswerTextChangedEvent,
  AnswerTextPreviewedEvent
} from '$/services/events/answer.events';

export class SignalrGameEventBus implements GameEventBus {
  private readonly hubConnection: HubConnection;

  // Game
  private readonly gameStateChanged = new Subject<GameStateChangedEvent>();
  public readonly gameStateChanged$ = this.gameStateChanged.asObservable();

  private readonly gameTitleChanged = new Subject<GameTitleChangedEvent>();
  public readonly gameTitleChanged$ = this.gameTitleChanged.asObservable();

  // Participants
  private readonly participantAdded = new Subject<ParticipantAddedEvent>();
  public readonly participantAdded$ = this.participantAdded.asObservable()

  // Rounds
  private readonly roundAdded = new Subject<RoundAddedEvent>();
  public readonly roundAdded$ = this.roundAdded.asObservable();

  private readonly roundRemoved = new Subject<RoundRemovedEvent>();
  public readonly roundRemoved$ = this.roundRemoved.asObservable();

  private readonly roundOrderChanged = new Subject<RoundOrderChangedEvent>();
  public readonly roundOrderChanged$ = this.roundOrderChanged.asObservable();

  private readonly roundStateChanged = new Subject<RoundStateChangedEvent>();
  public readonly roundStateChanged$ = this.roundStateChanged.asObservable();

  private readonly roundTitleChanged = new Subject<RoundTitleChangedEvent>();
  public readonly roundTitleChanged$ = this.roundTitleChanged.asObservable();

  private readonly roundDescriptionChanged = new Subject<RoundDescriptionChangedEvent>();
  public readonly roundDescriptionChanged$ = this.roundDescriptionChanged.asObservable();

  // Questions
  private readonly questionAdded = new Subject<QuestionAddedEvent>();
  public readonly questionAdded$ = this.questionAdded.asObservable();

  private readonly questionRevealed = new Subject<QuestionRevealedEvent>();
  public readonly questionRevealed$ = this.questionRevealed.asObservable();

  private readonly questionRemoved = new Subject<QuestionRemovedEvent>();
  public readonly questionRemoved$ = this.questionRemoved.asObservable();

  private readonly questionOrderChanged = new Subject<QuestionOrderChangedEvent>();
  public readonly questionOrderChanged$ = this.questionOrderChanged.asObservable();

  private readonly questionStateChanged = new Subject<QuestionStateChangedEvent>();
  public readonly questionStateChanged$ = this.questionStateChanged.asObservable();

  private readonly questionPointsChanged = new Subject<QuestionPointsChangedEvent>();
  public readonly questionPointsChanged$ = this.questionPointsChanged.asObservable();

  private readonly questionBodyTextChanged = new Subject<QuestionBodyTextChangedEvent>();
  public readonly questionBodyTextChanged$ = this.questionBodyTextChanged.asObservable();

  private readonly questionAnswerTextChanged = new Subject<QuestionAnswerTextChangedEvent>();
  public readonly questionAnswerTextChanged$ = this.questionAnswerTextChanged.asObservable();

  // Answers
  private readonly answerTextChanged = new Subject<AnswerTextChangedEvent>();
  public readonly answerTextChanged$ = this.answerTextChanged.asObservable();

  private readonly answerTextPreviewed = new Subject<AnswerTextPreviewedEvent>();
  public readonly answerTextPreviewed$ = this.answerTextPreviewed.asObservable();

  private readonly answerPointsChanged = new Subject<AnswerPointsChangedEvent>();
  public readonly answerPointsChanged$ = this.answerPointsChanged.asObservable();

  public constructor(hubConnection: HubConnection) {
    this.hubConnection = hubConnection;

    this.on('gameStateChanged', this.gameStateChanged);
    this.on('gameTitleChanged', this.gameTitleChanged);

    this.on('participantAdded', this.participantAdded);

    this.on('roundAdded', this.roundAdded);
    this.on('roundRemoved', this.roundRemoved);
    this.on('roundOrderChanged', this.roundOrderChanged);
    this.on('roundStateChanged', this.roundStateChanged);
    this.on('roundTitleChanged', this.roundTitleChanged);
    this.on('roundDescriptionChanged', this.roundDescriptionChanged);

    this.on('questionAdded', this.questionAdded);
    this.on('questionRevealed', this.questionRevealed);
    this.on('questionRemoved', this.questionRemoved);
    this.on('questionOrderChanged', this.questionOrderChanged);
    this.on('questionStateChanged', this.questionStateChanged);
    this.on('questionPointsChanged', this.questionPointsChanged);
    this.on('questionBodyTextChanged', this.questionBodyTextChanged);
    this.on('questionAnswerTextChanged', this.questionAnswerTextChanged);

    this.on('answerTextChanged', this.answerTextChanged);
    this.on('answerTextPreviewed', this.answerTextPreviewed);
    this.on('answerPointsChanged', this.answerPointsChanged);
  }

  private on<T>(eventName: string, subject: Subject<T>) {
    this.hubConnection.on(eventName, (event: T) => subject.next(event));

    if (!environment.production) {
      subject.subscribe(event => {
        const log = {
          $: eventName,
          ...event
        };
        console.log("[SignalR event bus]", log);
      });
    }
  }
}
