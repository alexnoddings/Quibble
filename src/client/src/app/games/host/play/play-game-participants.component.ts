import {Component, computed, inject, input, Signal} from '@angular/core';

import {FullGameInteractive, FullGameInteractiveImpl} from '$/services/interactive/full/game';
import {PlayGameViewState} from '$/app/games/host/play/play-game-view-state';
import {FullParticipantInteractive} from '$/services/interactive/full/participant';
import {ParticipantId} from '$/types/ids';
import {FullSubmittedAnswerInteractive} from '$/services/interactive/full/submitted-answer';
import {InteractiveBase} from '$/services/interactive/interactive-base';
import {ApiService} from '$/services/api/api.service';
import {GameEventBus} from '$/services/events/bus/event-bus';
import {FullParticipantData} from '$/services/api/participants/types/participant-data';
import {OcticonTrophyComponent} from '$/app/icons/trophy';
import {humanise} from '$/app/games/humanise';

export type ScoreTier = 'First' | 'Second' | 'Third' | 'Other';

export class ParticipantScoreGroup {
  public readonly tier: ScoreTier;
  public readonly score: number;
  public readonly participants: FullParticipantInteractive[] = [];

  public constructor(
    tier: ScoreTier,
    score: number
  ) {
    this.tier = tier;
    this.score = score;
  }
}

@Component({
  selector: 'quibble-host-play-game-participants',
  imports: [
    OcticonTrophyComponent
  ],
  templateUrl: './play-game-participants.component.html',
  styleUrls: ['./play-game-participants.component.css'],
})
export class PlayGameParticipantsComponent {
  public game = input.required<FullGameInteractive>();

  public viewState: PlayGameViewState = inject(PlayGameViewState);

  public get orderedGroupedParticipants(): Signal<ParticipantScoreGroup[]> {
    return computed(() => {
      const participants = this.game().participants();
      // Order by descending score
      const ordered = participants.sort((a, b) => b.totalPoints() - a.totalPoints());

      let count = 0;
      const groups: ParticipantScoreGroup[] = [];
      for (const participant of ordered) {
        const score = participant.totalPoints();
        let group = groups.find(g => g.score == score);
        if (!group) {
          const tier = PlayGameParticipantsComponent.mapToTier(count);
          group = new ParticipantScoreGroup(tier, score);
          groups.push(group);
        }
        group.participants.push(participant);
        count++;
      }

      return groups;
    });
  }

  private static mapToTier(count: number): ScoreTier {
    if (count == 0) {
      return 'First';
    }

    if (count == 1) {
      return 'Second';
    }

    if (count == 2) {
      return 'Third';
    }

    return 'Other';
  }

  // This really sucks, we should be grouping into a class which tracks the key
  // We should also probably be sorting by something stable first (username? participant id?)
  private static groupBy<T>(arr: T[], keyFn: (item: T) => number) {
    const grouped: T[][] = [];
    arr.forEach((i: T) => {
      const key = keyFn(i);
      let group = grouped[key];
      if (!group){
        group = [];
        grouped[key] = group;
      }
      group.push(i);
    });
    return grouped;
  }

  protected readonly humanise = humanise;
}
