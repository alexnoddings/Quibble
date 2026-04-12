import {QuestionState} from '$/types/question-state';
import {FullQuestionBodyData, PartialQuestionBodyData} from '$/services/api/questions/types/body/body-data';
import {FullQuestionAnswerData, PartialQuestionAnswerData} from '$/services/api/questions/types/answer/answer-data';
import {QuestionId} from '$/types/ids';

export interface FullQuestionData {
  readonly id: QuestionId;
  readonly order: number;
  readonly state: QuestionState;
  readonly points: number;

  readonly body: FullQuestionBodyData;
  readonly answer: FullQuestionAnswerData;
}

export interface PartialQuestionData {
  readonly id: string;
  readonly order: number;
  readonly state: QuestionState;
  readonly points: number;

  readonly body: PartialQuestionBodyData;
  readonly answer: PartialQuestionAnswerData;
}
