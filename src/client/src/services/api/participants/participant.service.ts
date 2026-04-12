import {HttpClient} from '@angular/common/http';
import {BaseEntityService} from '$/services/api/base.service';

export class ParticipantService extends BaseEntityService {
  constructor(http: HttpClient) {
    super(http);
  }
}
