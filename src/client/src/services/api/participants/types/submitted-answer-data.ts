import {ParticipantId, QuestionId} from '$/types/ids';

export interface FullSubmittedAnswerData {
  participantId: ParticipantId;
  questionId: string;
  points?: number;
  answer: string;
}

export interface PartialSubmittedAnswerData {
  questionId: QuestionId;
  points?: number;
  answer: string;
}
