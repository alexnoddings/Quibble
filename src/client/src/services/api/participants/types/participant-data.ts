import {ParticipantId} from '$/types/ids';
import {FullSubmittedAnswerData} from '$/services/api/participants/types/submitted-answer-data';

export interface FullParticipantData {
  readonly id: ParticipantId;
  readonly userName: string;
  readonly answers: FullSubmittedAnswerData[];
}

export interface PartialParticipantData {
  readonly id: string;
  readonly userName: string;
}
