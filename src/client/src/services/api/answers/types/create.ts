import {QuestionId} from '$/types/ids';
import {QuestionState} from '$/types/question-state';

export interface CreateQuestionRequest {
  readonly points: number;
  readonly body: string;
  readonly answer: string;
}

export interface CreateQuestionResponse {
  readonly id: QuestionId;
  readonly order: number;
  readonly state: QuestionState;
  readonly points: number;
  readonly bodyText: string;
  readonly answerText: string;
}
