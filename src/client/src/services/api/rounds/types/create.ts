import {RoundId} from '$/types/ids';
import {RoundState} from '$/types/round-state';

export interface CreateRoundRequest {
  readonly title: string;
}

export interface CreateRoundResponse {
  readonly id: RoundId;
  readonly order: number;
  readonly state: RoundState;
  readonly title: string;
  readonly description: string;
}
