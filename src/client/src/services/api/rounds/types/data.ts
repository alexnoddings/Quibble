import {RoundId} from '$/types/ids';
import {RoundState} from '$/types/round-state';
import {FullQuestionData, PartialQuestionData} from '$/services/api/questions/types/question-data';

export interface FullRoundData {
  readonly id: RoundId;
  readonly order: number;
  readonly state: RoundState;
  readonly title: string;
  readonly description: string;

  readonly questions: FullQuestionData[];
}

export interface PartialRoundData {
  id: string;
  order: number;
  state: RoundState;
  title: string;
  description: string;

  questions: PartialQuestionData[];
}
