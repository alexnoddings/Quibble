import {HttpClient} from '@angular/common/http';
import {BaseEntityService} from '$/services/api/base.service';
import {Result} from '$/services/api/result';
import {ParticipantId, QuestionId} from '$/types/ids';
import {UpdateQuestionPointsRequest} from '$/services/api/questions/types/update-points';
import {
  PreviewQuestionSubmittedAnswerTextRequest,
  UpdateQuestionSubmittedAnswerTextRequest
} from '$/services/api/answers/types/answer/update-text';

export class AnswerService extends BaseEntityService {
  constructor(http: HttpClient) {
    super(http);
  }

  async updateAnswer(questionId: QuestionId, request: UpdateQuestionSubmittedAnswerTextRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/answers/@me/text`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async previewAnswer(questionId: QuestionId, request: PreviewQuestionSubmittedAnswerTextRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/answers/@me/text/preview`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updatePoints(participantId: ParticipantId, questionId: QuestionId, request: UpdateQuestionPointsRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/answers/${participantId}/points`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }
}
