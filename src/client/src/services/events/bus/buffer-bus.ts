import {BufferReady} from "$/utils/buffer/buffer-ready";
import {mergeAll, Observable, share} from "rxjs";
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
import {bufferUntil} from "$/utils/buffer/buffer-until";
import {GameEventBus} from '$/services/events/bus/event-bus';
import {
  AnswerPointsChangedEvent,
  AnswerTextChangedEvent,
  AnswerTextPreviewedEvent
} from '$/services/events/answer.events';

export class BufferGameEventBus implements GameEventBus {
  private readonly ready: BufferReady;

  public readonly gameStateChanged$: Observable<GameStateChangedEvent>;
  public readonly gameTitleChanged$: Observable<GameTitleChangedEvent>;

  public readonly participantAdded$: Observable<ParticipantAddedEvent>;

  public readonly roundAdded$: Observable<RoundAddedEvent>;
  public readonly roundRemoved$: Observable<RoundRemovedEvent>;
  public readonly roundDescriptionChanged$: Observable<RoundDescriptionChangedEvent>;
  public readonly roundOrderChanged$: Observable<RoundOrderChangedEvent>;
  public readonly roundStateChanged$: Observable<RoundStateChangedEvent>;
  public readonly roundTitleChanged$: Observable<RoundTitleChangedEvent>;

  public readonly questionAdded$: Observable<QuestionAddedEvent>;
  public readonly questionRevealed$: Observable<QuestionRevealedEvent>;
  public readonly questionRemoved$: Observable<QuestionRemovedEvent>;
  public readonly questionOrderChanged$: Observable<QuestionOrderChangedEvent>;
  public readonly questionStateChanged$: Observable<QuestionStateChangedEvent>;
  public readonly questionPointsChanged$: Observable<QuestionPointsChangedEvent>;
  public readonly questionBodyTextChanged$: Observable<QuestionBodyTextChangedEvent>;
  public readonly questionAnswerTextChanged$: Observable<QuestionAnswerTextChangedEvent>;

  public readonly answerTextChanged$: Observable<AnswerTextChangedEvent>;
  public readonly answerTextPreviewed$: Observable<AnswerTextPreviewedEvent>;
  public readonly answerPointsChanged$: Observable<AnswerPointsChangedEvent>;

  public constructor(ready: BufferReady, inner: GameEventBus) {
    this.ready = ready;

    this.gameStateChanged$ = this.buffer(inner.gameStateChanged$);
    this.gameTitleChanged$ = this.buffer(inner.gameTitleChanged$);

    this.participantAdded$ = this.buffer(inner.participantAdded$);

    this.roundAdded$ = this.buffer(inner.roundAdded$);
    this.roundRemoved$ = this.buffer(inner.roundRemoved$);
    this.roundDescriptionChanged$ = this.buffer(inner.roundDescriptionChanged$);
    this.roundOrderChanged$ = this.buffer(inner.roundOrderChanged$);
    this.roundStateChanged$ = this.buffer(inner.roundStateChanged$);
    this.roundTitleChanged$ = this.buffer(inner.roundTitleChanged$);

    this.questionAdded$ = this.buffer(inner.questionAdded$);
    this.questionRevealed$ = this.buffer(inner.questionRevealed$);
    this.questionRemoved$ = this.buffer(inner.questionRemoved$);
    this.questionOrderChanged$ = this.buffer(inner.questionOrderChanged$);
    this.questionStateChanged$ = this.buffer(inner.questionStateChanged$);
    this.questionPointsChanged$ = this.buffer(inner.questionPointsChanged$);
    this.questionBodyTextChanged$ = this.buffer(inner.questionBodyTextChanged$);
    this.questionAnswerTextChanged$ = this.buffer(inner.questionAnswerTextChanged$);

    this.answerTextChanged$ = this.buffer(inner.answerTextChanged$);
    this.answerTextPreviewed$ = this.buffer(inner.answerTextPreviewed$);
    this.answerPointsChanged$ = this.buffer(inner.answerPointsChanged$);
  }

  private buffer<T>(observable: Observable<T>): Observable<T> {
    return observable
      // buffer until ready
      .pipe(bufferUntil(this.ready.ready$))
      // buffer emits as an array, instead flatten
      .pipe(mergeAll())
      // share ensures anything subscribing after the ready$ event
      // doesn't get caught in the buffer
      .pipe(share());
  }
}
