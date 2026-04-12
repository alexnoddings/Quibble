import {
  FullSubmittedAnswerData,
  PartialSubmittedAnswerData
} from '$/services/api/participants/types/submitted-answer-data';

export interface FullQuestionAnswerData {
  answer: string;
  submittedAnswers: FullSubmittedAnswerData[];
}

export interface PartialQuestionAnswerData {
  answer: string;
  submittedAnswer: PartialSubmittedAnswerData;
}
