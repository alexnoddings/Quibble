import {GameId} from '$/types/ids';
import {GameState} from '$/types/game-state';
import {FullParticipantData, PartialParticipantData} from '$/services/api/participants/types/participant-data';
import {FullRoundData, PartialRoundData} from '$/services/api/rounds/types/data';

export interface FullGameData {
  readonly perspective: 'Host';

  readonly id: GameId;
  readonly slug: string;
  readonly state: GameState;
  readonly title: string;

  readonly participants: FullParticipantData[];
  readonly rounds: FullRoundData[];
}

export interface PartialGameData {
  readonly perspective: 'Participant';

  id: string;
  slug: string;
  ownerName: string;
  state: GameState;
  title: string;

  participants: PartialParticipantData[];
  rounds: PartialRoundData[];
}

export type GameData = FullGameData | PartialGameData;
