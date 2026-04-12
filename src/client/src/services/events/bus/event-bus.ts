import {Observable} from "rxjs";
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
import {
  AnswerPointsChangedEvent,
  AnswerTextChangedEvent,
  AnswerTextPreviewedEvent
} from '$/services/events/answer.events';

export interface GameEventBus {
  readonly gameStateChanged$: Observable<GameStateChangedEvent>;
  readonly gameTitleChanged$: Observable<GameTitleChangedEvent>;

  readonly participantAdded$: Observable<ParticipantAddedEvent>;

  readonly roundAdded$: Observable<RoundAddedEvent>;
  readonly roundRemoved$: Observable<RoundRemovedEvent>;
  readonly roundOrderChanged$: Observable<RoundOrderChangedEvent>;
  readonly roundStateChanged$: Observable<RoundStateChangedEvent>;
  readonly roundTitleChanged$: Observable<RoundTitleChangedEvent>;
  readonly roundDescriptionChanged$: Observable<RoundDescriptionChangedEvent>;

  readonly questionAdded$: Observable<QuestionAddedEvent>;
  readonly questionRevealed$: Observable<QuestionRevealedEvent>;
  readonly questionRemoved$: Observable<QuestionRemovedEvent>;
  readonly questionOrderChanged$: Observable<QuestionOrderChangedEvent>;
  readonly questionStateChanged$: Observable<QuestionStateChangedEvent>;
  readonly questionPointsChanged$: Observable<QuestionPointsChangedEvent>;

  readonly questionBodyTextChanged$: Observable<QuestionBodyTextChangedEvent>;
  readonly questionAnswerTextChanged$: Observable<QuestionAnswerTextChangedEvent>;

  readonly answerTextChanged$: Observable<AnswerTextChangedEvent>;
  readonly answerTextPreviewed$: Observable<AnswerTextPreviewedEvent>;
  readonly answerPointsChanged$: Observable<AnswerPointsChangedEvent>;
}
