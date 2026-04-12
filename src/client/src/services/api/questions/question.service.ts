import {HttpClient} from '@angular/common/http';
import {BaseEntityService} from '$/services/api/base.service';
import {Result} from '$/services/api/result';
import {QuestionId, RoundId} from '$/types/ids';
import {CreateQuestionRequest, CreateQuestionResponse} from '$/services/api/questions/types/create';
import {UpdateQuestionPointsRequest} from '$/services/api/questions/types/update-points';
import {UpdateQuestionStateRequest} from '$/services/api/questions/types/update-state';
import {UpdateQuestionBodyTextRequest} from '$/services/api/questions/types/answer/update-text';
import {UpdateQuestionAnswerTextRequest} from '$/services/api/questions/types/body/update-text';
import {UpdateQuestionOrderRequest} from '$/services/api/questions/types/update-order';

export class QuestionService extends BaseEntityService {
  constructor(http: HttpClient) {
    super(http);
  }

  async create(roundId: RoundId, request: CreateQuestionRequest) : Promise<Result<CreateQuestionResponse>> {
    const url = this.getUrl(`/rounds/${roundId}/questions`);
    const http = this.http.post<CreateQuestionResponse>(url, request);
    return await this.send(http);
  }

  async updateOrder(questionId: QuestionId, request: UpdateQuestionOrderRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/order`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateState(questionId: QuestionId, request: UpdateQuestionStateRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/state`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updatePoints(questionId: QuestionId, request: UpdateQuestionPointsRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/points`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateBodyText(questionId: QuestionId, request: UpdateQuestionBodyTextRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/body/text`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async updateAnswerText(questionId: QuestionId, request: UpdateQuestionAnswerTextRequest): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}/answer/text`);
    const http = this.http.put<void>(url, request);
    return await this.send(http);
  }

  async delete(questionId: QuestionId): Promise<Result<void>> {
    const url = this.getUrl(`/questions/${questionId}`);
    const http = this.http.delete<void>(url);
    return await this.send(http);
  }
}
