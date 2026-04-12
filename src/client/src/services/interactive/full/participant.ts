import {InteractiveBase} from '$/services/interactive/interactive-base';
import {FullGameInteractive, FullGameInteractiveImpl} from '$/services/interactive/full/game';
import {
  FullSubmittedAnswerInteractive,
  FullSubmittedAnswerInteractiveImpl
} from '$/services/interactive/full/submitted-answer';
import {computed, signal, Signal, WritableSignal} from '@angular/core';
import {ParticipantId} from '$/types/ids';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullParticipantData} from '$/services/api/participants/types/participant-data';

export interface FullParticipantInteractive {
  readonly game: FullGameInteractive;

  readonly id: ParticipantId;
  readonly userName: string;

  readonly answers: Signal<FullSubmittedAnswerInteractive[]>;
  readonly totalPoints: Signal<number>;

  // kick(): Promise<void>;
}

export class FullParticipantInteractiveImpl extends InteractiveBase implements FullParticipantInteractive {
  public readonly game: FullGameInteractiveImpl;

  public readonly id: ParticipantId;
  public readonly userName: string;

  public readonly answers: Signal<FullSubmittedAnswerInteractive[]>;
  public readonly totalPoints: Signal<number>;

  public constructor(
    api: ApiService,
    events: GameEventBus,
    game: FullGameInteractiveImpl,
    data: FullParticipantData
  ) {
    super(api, events);
    this.game = game;
    this.id = data.id;

    this.userName = data.userName;

    // const answers: FullSubmittedAnswerInteractiveImpl[] = data.answers
    //   .map(a => {
    //     const qs = game.rounds().flatMap(r => r.questions());
    //     const q = qs.find(q => q.id == a.questionId);
    //     if (q == undefined)
    //       return undefined;
    //     const ai = new FullSubmittedAnswerInteractiveImpl(api, events, this, q, a);
    //     q.answer.submittedAnswers.update(ais => [...ais, ai]);
    //     return ai;
    //   })
    //   .filter(x => !!x);

    this.answers = computed(() =>
      this.game
        .rounds()
        .flatMap(r => r.questions())
        .flatMap(q => q.answer.submittedAnswers())
        .filter(a => a.participant.id == this.id)
    );

    this.totalPoints = computed(() =>
      this.answers().reduce((score, answer) => score + (answer.points() ?? 0), 0)
    );
  }
}
